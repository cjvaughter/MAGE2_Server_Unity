using System;
using System.Collections;
using System.Collections.Generic;

public enum GameState : byte
{
    Waiting,    // for players to connect
    Ready,      // for round to start
    Active,     // during a round
    Timeout,    // break between rounds
    Finished,   // player won the round
    Complete,   // game over
}

public static class Game
{
    public static AnnouncerBehavior Announcer;

    const int TimeoutPeriod = 3000;
    const int ReadyPeriod = 7000;
    const int FinishedPeriod = 2000;

    static long _lastTime;
    static int _countdown;
    static bool _running;

    static TimeSpan UtcOffset { get; set; }
    public static DateTime Now { get { return DateTime.UtcNow + UtcOffset; } }

    public static long CurrentTime { get; private set; }
    public static int TimeRemaining { get; private set; }
    public static GameState State { get; private set; }
    public static GameType Type { get; private set; }
    public static IGameRules Rules { get; private set; }
    public static int Round { get; private set; }
    public static int Rounds { get; private set; }
    public static int PlayerCount { get; private set; }
    public static int RoundLength { get; private set; }

    static Game()
    {
        UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
    }

    public static void Start()
    {
        CurrentTime = Now.TimeOfDay.Ticks;
        _lastTime = CurrentTime;
        TimeRemaining = 0;
        _countdown = 0;

        State = GameState.Waiting;
        Round = 0;
        Rounds = Settings.Rounds;
        PlayerCount = Settings.PlayerCount;
        RoundLength = Settings.RoundLength * 1000;

        Type = (GameType)Enum.Parse(typeof(GameType), Settings.GameType.Replace(" ", ""), true);
        switch (Type)
        {
            case GameType.FreeForAll:
                Rules = new FreeForAll();
                break;
            case GameType.TeamBattle:
                Rules = new TeamBattle();
                break;
            case GameType.TestMode:
                Rules = new TestMode();
                State = GameState.Active;
                Rounds = 0;
                break;
        }

        Logger.Begin(Type, PlayerCount, Rounds, RoundLength);
        HUDPanelBehavior.Initialize(Settings.GameType, Rounds);
        Coordinator.Start();
        Database.Create();
        Devices.Load();
        Logger.Log(LogEvents.ServerInitialized);
        _running = true;
    }

    public static IEnumerator Stop()
    {
        if (_running)
        {
            Coordinator.SendMessage(Coordinator.Broadcast, (byte)MsgFunc.Disconnect);

            if (State == GameState.Complete)
                Wrapup();
            else
                Logger.Log(LogEvents.GameQuit);

            Logger.Save();
            Coordinator.Stop();
            Players.ResetPlayers(); //To kill effects
            Players.Clear();
            _running = false;
        }
        yield return null;
    }

    public static void Pause()
    {
        if(State != GameState.Waiting) Logger.Log(LogEvents.GamePaused);
        Coordinator.Pause();
        Announcer.Pause();
    }

    public static void Unpause()
    {
        CurrentTime = Now.TimeOfDay.Ticks;
        _lastTime = CurrentTime;
        Logger.Log(LogEvents.GameUnpaused);
        Players.ResetHeartbeats();
        Coordinator.Unpause();
        Announcer.Unpause();
    }

    public static void Run()
    {
        if (_running)
        {
            CurrentTime = Now.TimeOfDay.Ticks;
            ProcessMessages();
            UpdateState();
        }
    }

    private static void ProcessMessages()
    {
        if (!Coordinator.HasMessage()) return;
        MAGEMsg msg = Coordinator.GetMessage();
        Player p = Players.Get(msg.Address);

        if(p == null)
        {
            if ((MsgFunc)msg.Data[0] != MsgFunc.Connect)
            {
                Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Disconnect);
                return;
            }
        }
        else
        {
            p.Heartbeat = CurrentTime;
        }

        switch ((MsgFunc)msg.Data[0])
        {
            case MsgFunc.Heartbeat:
                if(!p.Connected)
                {
                    p.Connected = true;
                    Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Connect, (byte)p.Team);
                    Coordinator.UpdatePlayer(p);
                    Logger.Log(LogEvents.Reconnected, p);
                }
                break;
            case MsgFunc.Connect:
                if ((State == GameState.Waiting || Rules.ConnectAnytime) && p == null)
                {
                    if ((p = Players.Add(msg)) != null)
                    {
                        if(Type == GameType.TestMode)
                            Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Connect, (byte)Colors.Red, (byte)MsgFunc.State, (byte)EntityState.Alive, (byte)MsgFunc.Health, 100, (byte)MsgFunc.Effect, (byte)Colors.NoColor, (byte)MsgFunc.Update);
                        else
                            Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Connect, (byte)p.Team, (byte)MsgFunc.State, (byte)EntityState.Dead, (byte)MsgFunc.Health, 0, (byte)MsgFunc.Effect, (byte)Colors.NoColor, (byte)MsgFunc.Update);
                    }
                }
                else
                {
                    if (p != null)
                    {
                        p.Connected = true;
                        p.Heartbeat = CurrentTime;
                        Logger.Log(LogEvents.Reconnected, p);
                        Coordinator.SendMessage((byte)MsgFunc.Connect, (byte)p.Team);
                        Coordinator.UpdatePlayer(p);
                    }
                    else
                    {
                        Logger.Log(LogEvents.ConnectDuringGame);
                        Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Disconnect);
                    }
                }
                break;
            case MsgFunc.Spell_TX:
                if (State != GameState.Active) break;
                Spell.Add(p, msg.Data);
                break;
            case MsgFunc.Spell_RX:
                if (State != GameState.Active) break;
                Spell.Process(p, msg.Data);
                break;
            case MsgFunc.ChangeWeapon:
                ushort deviceID = (ushort)((msg.Data[1] << 8) | msg.Data[2]);
                Device d = Devices.Get(deviceID);
                if (d == null)
                {
                    Logger.Log(LogEvents.InvalidDevice, deviceID);
                }
                else if(p.Device != d)
                {
                    p.Device = d;
                    Logger.Log(LogEvents.ChangedWeapon, p);
                }
                break;
            case MsgFunc.Device_RX:
                if (State != GameState.Active) break;
                Spell.ForceProcess(p, msg.Data);
                break;
            default:
                Logger.Log(LogEvents.InvalidMessage);
                break;
        }
    }

    private static void UpdateState()
    {
        if (CurrentTime - _lastTime < TimeSpan.TicksPerMillisecond) return;

        if (Rules.Timed)
        {
            DoTiming();

            if (TimeRemaining <= 0)
            {
                TimeRemaining = 0;
                switch (State)
                {
                    case GameState.Waiting:
                        if (Rules.CanStart)
                        {
                            State = GameState.Timeout;
                            TimeRemaining = TimeoutPeriod;
                            Logger.Log(LogEvents.GameBegan);
                            switch (Type)
                            {
                                case GameType.FreeForAll:
                                    Announcer.Speak(Phrase.FreeForAll);
                                    break;
                                case GameType.TeamBattle:
                                    Announcer.Speak(Phrase.TeamBattle);
                                    break;
                            }
                        }
                        break;
                    case GameState.Active:
                        Coordinator.SendMessage(Coordinator.Broadcast, (byte)MsgFunc.State, (byte)EntityState.Dead, (byte)MsgFunc.Update);
                        Players.ResetPlayers();
                        Logger.Log(LogEvents.RoundEnded, Round);
                        if (Round == Rounds)
                        {
                            //State = GameState.Complete;
                            //Logger.Log(LogEvents.GameEnded);
                            //Announcer.Speak(Phrase.Game);
                            State = GameState.Timeout;
                            TimeRemaining = TimeoutPeriod;
                            Announcer.Speak(Phrase.Time);
                            Round = 0;
                        }
                        else
                        {
                            State = GameState.Timeout;
                            TimeRemaining = TimeoutPeriod;
                            Announcer.Speak(Phrase.Time);
                        }
                        break;
                    case GameState.Timeout:
                        State = GameState.Ready;
                        TimeRemaining = ReadyPeriod;
                        Round++;
                        if (Round == Rounds) Announcer.Speak(Phrase.FinalRound);
                        else
                        {
                            if (Round == 1) Announcer.Speak(Phrase.Round1);
                            if (Round == 2) Announcer.Speak(Phrase.Round2);
                            if (Round == 3) Announcer.Speak(Phrase.Round3);
                            if (Round == 4) Announcer.Speak(Phrase.Round4);
                        }
                        _countdown = 5;
                        break;
                    case GameState.Ready:
                        Coordinator.SendMessage(Coordinator.Broadcast, (byte)MsgFunc.State, (byte)EntityState.Alive, (byte)MsgFunc.Health, 100, (byte)MsgFunc.Effect, (byte)Colors.NoColor, (byte)MsgFunc.Update);
                        State = GameState.Active;
                        TimeRemaining = RoundLength;
                        Logger.Log(LogEvents.RoundBegan, Round);
                        Announcer.Speak(Phrase.Go);
                        _countdown = 5;
                        break;
                    case GameState.Finished:
                        Players.ResetPlayers();
                        State = GameState.Timeout;
                        TimeRemaining = TimeoutPeriod - FinishedPeriod;
                        break;
                }
            }
        }

        Players.VerifyHeartbeats();
        Players.ClearExpiredEffects();
        Spell.ClearExpired();

        if (Rules.GameIsOver && !(State == GameState.Waiting || State == GameState.Finished)) // Did somebody win?
        {
            Coordinator.SendMessage(Coordinator.Broadcast, (byte)MsgFunc.State, (byte)EntityState.Dead, (byte)MsgFunc.Update);
            Logger.Log(LogEvents.RoundEnded, Round);


            Players.ResetPlayers();
            Announcer.Speak(Phrase.Finished);
            State = GameState.Timeout;
            TimeRemaining = TimeoutPeriod;
            Round = 0;
            /*
            _countdown = 0;            
            if (Round == Rounds)
            {
                Players.ResetPlayers();
                State = GameState.Complete;
                Logger.Log(LogEvents.GameEnded);
                Announcer.Speak(Phrase.Game);
            }
            else
            {
                State = GameState.Finished;
                TimeRemaining = FinishedPeriod;
                Announcer.Speak(Phrase.Finished);
                //log round winner for xp bonus and to determine overall winner
            }
            */
        }
    }

    private static void DoTiming()
    {
        if (State != GameState.Waiting)
        {
            TimeRemaining -= TimeSpan.FromTicks(CurrentTime - _lastTime).Milliseconds;
        }
        _lastTime = CurrentTime;

        if (TimeRemaining <= 5000 && _countdown == 5)
        {
            if (State == GameState.Active) Announcer.Speak(Phrase.Five);
            if (State == GameState.Ready) Announcer.Speak(Phrase.Ready);
            _countdown--;
        }
        else if (TimeRemaining <= 4000 && _countdown == 4)
        {
            if (State == GameState.Active) Announcer.Speak(Phrase.Four);
            _countdown--;
        }
        else if (TimeRemaining <= 3000 && _countdown == 3)
        {
            Announcer.Speak(Phrase.Three);
            _countdown--;
        }
        else if (TimeRemaining <= 2000 && _countdown == 2)
        {
            Announcer.Speak(Phrase.Two);
            _countdown--;
        }
        else if (TimeRemaining <= 1000 && _countdown == 1)
        {
            Announcer.Speak(Phrase.One);
            _countdown--;
        }
    }

    private static void Wrapup()
    {
        //Announce winner(s), do levelups, save to db, play again?, exit
    }
}
