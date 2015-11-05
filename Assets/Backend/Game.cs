using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public enum GameState : byte
{
    Waiting,    // for players to connect
    Ready,      // for round to start
    Active,     // during a round
    Timeout,    // break between rounds
    Complete,   // game over
}

public static class Game
{
    public static AnnouncerBehavior Announcer;

    const int TimeoutPeriod = 3000;
    const int ReadyPeriod = 7000;

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
    public static bool Timed { get; private set; }
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
        Timed = true;
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
                Teams.Add(new Team());
                Teams.Add(new Team());
                break;
            case GameType.TestMode:
                Rules = new TestMode();
                State = GameState.Active;
                Rounds = 0;
                Timed = false;
                break;
        }

        Logger.Begin(Type, PlayerCount, Rounds, RoundLength);
        HUDPanelBehavior.Initialize(Settings.GameType, Rounds);
        Coordinator.Start();
        Database.Create();
        Database.Connect();
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
            Players.Clear();
            Database.Disconnect();
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
            Coordinator.Receive();
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
                Logger.Log("Heartbeat");
                if(!p.Connected)
                {
                    p.Connected = true;
                    Logger.Log(LogEvents.Reconnected, p);
                }
                break;
            case MsgFunc.Connect:
                if ((State == GameState.Waiting || Rules.ConnectAnytime) && p == null)
                {
                    if ((p = Players.Add(msg)) != null)
                    {
                        Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Connect, (byte)p.Team, (byte)MsgFunc.State, (byte)EntityState.Dead, (byte)MsgFunc.Update);
                    }
                }
                else
                {
                    if (p != null)
                    {
                        p.Connected = true;
                        p.Heartbeat = CurrentTime;
                        Logger.Log(LogEvents.Reconnected, p);
                        Coordinator.SendMessage(msg.Address, (byte)MsgFunc.Connect, (byte)p.Team, (byte)MsgFunc.State, (byte)p.State, (byte)MsgFunc.Health, (byte)p.Health, (byte)MsgFunc.Update);
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
            default:
                Logger.Log(LogEvents.InvalidMessage);
                break;
        }
    }

    private static void UpdateState()
    {
        if (CurrentTime - _lastTime >= TimeSpan.TicksPerMillisecond)
        {
            if (Timed && State != GameState.Waiting)
            {
                TimeRemaining -= TimeSpan.FromTicks(CurrentTime - _lastTime).Milliseconds;
            }
            _lastTime = CurrentTime;

            if (TimeRemaining <= 5000 && _countdown == 5)
            {
                if(State == GameState.Active) Announcer.Speak(Phrase.Five);
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

            if (Timed && TimeRemaining <= 0)
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
                            switch(Type)
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
                            State = GameState.Complete;
                            Logger.Log(LogEvents.GameEnded);
                            Announcer.Speak(Phrase.Game);
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
                        Coordinator.SendMessage(Coordinator.Broadcast, (byte)MsgFunc.State, (byte)EntityState.Alive, (byte)MsgFunc.Health, 100, (byte)MsgFunc.Update);
                        State = GameState.Active;
                        TimeRemaining = RoundLength;
                        Logger.Log(LogEvents.RoundBegan, Round);
                        Announcer.Speak(Phrase.Go);
                        _countdown = 5;
                        break;
                }
            }

            Players.VerifyHeartbeats();
            Players.ClearExpiredEffects();
            Spell.ClearExpired();

            if (Rules.GameIsOver && State != GameState.Waiting) // Did somebody win?
                State = GameState.Complete;
        }
    }

    private static void Wrapup()
    {
        //Announce winner(s), do levelups, save to db, play again?, exit
    }
}
