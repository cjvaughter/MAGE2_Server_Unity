using System;
using System.Threading;
using UnityEngine;

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
    public static OverlayBehavior Announcer;

    const int TimeoutPeriod = 3000;
    const int ReadyPeriod = 7000;

    static long _lastTime;
    static int _timeRemaining;
    static int _countdown;

    public static long CurrentTime;
    public static GameState State { get; private set; }
    public static GameType Type { get; private set; }
    public static IGameRules Rules { get; private set; }
    public static int Round { get; private set; }
    public static int Rounds { get; private set; }
    public static bool Timed { get; private set; }
    public static int PlayerCount { get; private set; }
    public static int RoundLength { get; private set; }

    public static void Start()
    {
        CurrentTime = DateTime.Now.TimeOfDay.Ticks;
        _lastTime = CurrentTime;
        _timeRemaining = 0;
        _countdown = 0;
        State = GameState.Waiting;
        Type = (GameType)Enum.Parse(typeof(GameType), Settings.GameType.Replace(" ", ""), true);
        switch (Type)
        {
            case GameType.TestMode:
                Rules = new TestMode();
                State = GameState.Active;
                break;
            case GameType.FreeForAll:
                Rules = new FreeForAll();
                break;
            case GameType.TeamBattle:
                Rules = new TeamBattle();
                Teams.Add(new Team());
                Teams.Add(new Team());
                break;
        }
        Round = 0;
        Rounds = Settings.Rounds;
        Timed = Settings.Timed;
        PlayerCount = Settings.PlayerCount;
        RoundLength = Settings.RoundLength * 1000;
        
        Logger.Initialize(Type, PlayerCount, Rounds, RoundLength);
        HUDPanelBehavior.Initialize(Settings.GameType, Rounds);
        Coordinator.Start();
        Database.Connect();
        Logger.Log(LogEvents.ServerInitialized);
    }

    public static void Stop()
    {
        if (State == GameState.Complete)
            Wrapup();
        else
            Logger.Log(LogEvents.GameQuit);

        Coordinator.Stop();
        Logger.Save();
        Players.Clear();
        Database.Disconnect();
    }

    public static void Run()
    {
        CurrentTime = DateTime.Now.TimeOfDay.Ticks;
        ProcessMessages();
        UpdateState();
    }

    private static void ProcessMessages()
    {
        if (!Coordinator.HasMessage()) return;
        MAGEMsg msg = Coordinator.GetMessage();
            
        switch ((MsgFunc)msg.Data[0])
        {
            case MsgFunc.Heartbeat:
                Players.Get(msg.Address).Heartbeat = CurrentTime;
                break;
            case MsgFunc.Connect:
                if (State == GameState.Waiting)
                {
                    if (Players.Add(msg))
                    {
                        //_wnd.BeginInvoke(new Action(() => _wnd.AddPlayer(Players.Get(msg.Address))));
                        Player p = Players.Get(msg.Address);
                        Coordinator.SendMessage(new MAGEMsg(msg.Address, new[] { (byte)MsgFunc.Connect, (byte)p.Team }));
                    }
                }
                else
                {
                    if (Players.Exists(msg.Address))
                    {
                        Player p = Players.Get(msg.Address);
                        p.Connected = true;
                        Logger.Log(LogEvents.Reconnected, p);
                        //send response
                    }
                    else
                    {
                        Logger.Log(LogEvents.ConnectDuringGame);
                    }
                }
                break;
            case MsgFunc.SentSpell:
                Players.Get(msg.Address).Misses++;
                break;
            case MsgFunc.RecievedSpell:
                Spell.Process(Players.Get(msg.Address), msg.Data);
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
                _timeRemaining -= TimeSpan.FromTicks(CurrentTime - _lastTime).Milliseconds;
            }
            _lastTime = CurrentTime;

            if (_timeRemaining <= 5000 && _countdown == 5)
            {
                if(State == GameState.Active) Announcer.Speak(Phrase.Five);
                if (State == GameState.Ready) Announcer.Speak(Phrase.Ready);
                _countdown--;
            }
            else if (_timeRemaining <= 4000 && _countdown == 4)
            {
                if (State == GameState.Active) Announcer.Speak(Phrase.Four);
                _countdown--;
            }
            else if (_timeRemaining <= 3000 && _countdown == 3)
            {
                Announcer.Speak(Phrase.Three);
                _countdown--;
            }
            else if (_timeRemaining <= 2000 && _countdown == 2)
            {
                Announcer.Speak(Phrase.Two);
                _countdown--;
            }
            else if (_timeRemaining <= 1000 && _countdown == 1)
            {
                Announcer.Speak(Phrase.One);
                _countdown--;
            }

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                switch (State)
                {
                    case GameState.Waiting:
                        if (Rules.CanStart)
                        {
                            State = GameState.Timeout;
                            _timeRemaining = TimeoutPeriod;
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
                            _timeRemaining = TimeoutPeriod;
                            Announcer.Speak(Phrase.Time);
                        }
                        break;
                    case GameState.Timeout:
                        State = GameState.Ready;
                        _timeRemaining = ReadyPeriod;
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
                        State = GameState.Active;
                        _timeRemaining = RoundLength;
                        Logger.Log(LogEvents.RoundBegan, Round);
                        Announcer.Speak(Phrase.Go);
                        _countdown = 5;
                        break;
                }
            }

            //Players.VerifyHeartbeats(_currentTime);
            Spell.ClearExpired(CurrentTime);

            if (Rules.GameIsOver && State != GameState.Waiting) // Did somebody win?
                State = GameState.Complete;

            UpdateScreen();
        }
    }

    private static void UpdateScreen()
    {
        int time = (State == GameState.Active) ? _timeRemaining : 0;
        HUDPanelBehavior.UpdatePanel(time, Round);
    }

    private static void Wrapup()
    {
        //Announce winner(s), do levelups, save to db, play again?, exit
    }
}
