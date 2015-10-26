using System;
using UnityEngine;
using UnityEngine.UI;
/*
public enum GameState : byte
{
    Waiting,    // for players to connect
    Ready,      // for round to start
    Active,     // during a round
    Timeout,    // break between rounds
    Complete,   // game over
}
*/
public class GameController : MonoBehaviour
{
    public GameObject overlay;

    void Start()
    {
        Game.Announcer = overlay.GetComponent<OverlayBehavior>();
        Game.Start();
    }

    void OnApplicationQuit()
    {
        Game.Stop();
    }

    void FixedUpdate()
    {
        Game.Run();
    }
    
    
    /*
    const int TimeoutPeriod = 5000;
    const int ReadyPeriod = 5000;
    const long UpdatePeriod = TimeSpan.TicksPerMillisecond * 50;

    long _lastTime;
    long _lastWindowUpdate;
    int _timeRemaining;
    int _countdown;

    public long CurrentTime;
    public GameState State { get; private set; }
    //public GameType Type { get; private set; }
    //public IGameRules Rules { get; private set; }
    public int Round { get; private set; }
    public int Rounds { get; private set; }
    public bool Timed { get; private set; }
    public int PlayerCount { get; private set; }
    public int RoundLength { get; private set; }

    public GameObject Overlay;
    private OverlayBehavior Announcer;

    void Start()
    {
        Announcer = Overlay.GetComponent<OverlayBehavior>();

        Round = 0;
        _timeRemaining = 0;
        _countdown = 0;
        _lastTime = 0;
        CurrentTime = 0;
        //PlayerCount = Settings.Default.PlayerCount;
        RoundLength = 10 * 1000;
        Rounds = 1;
        Timed = true;
        State = GameState.Waiting;
        //Type = (GameType)Enum.Parse(typeof(GameType), Settings.Default.GameType, true);
        /*
        switch (Type)
        {
            case GameType.TestMode:
                Rules = new TestMode();
                State = GameState.Active;
                break;
            case GameType.Deathmatch:
                Rules = new Deathmatch();
                break;
            case GameType.TeamDeathmatch:
                Rules = new TeamDeathmatch();
                break;
        }
        */
        //Logger.Initialize(Type, PlayerCount, Rounds, RoundLength);
        //Coordinator.Start();

        //_lastTime = DateTime.Now.TimeOfDay.Ticks;

        //Database.Connect();
        //CurrentTime = DateTime.Now.TimeOfDay.Ticks;
/*
        Logger.Log(LogEvents.ServerInitialized);
    }

    void Exit()
    {
        // if manual exit //Logger.Log(LogEvents.GameQuit);

        //Coordinator.Stop();
        //Logger.Save();
        //Players.Clear();
        //Database.Disconnect();
    }

    void FixedUpdate()
    {
        if (State != GameState.Complete)
        {
            CurrentTime = DateTime.Now.TimeOfDay.Ticks;
            ProcessMessages();
            DoTiming();
        }
        else
        {
            Wrapup();
        }
    }



    private void ProcessMessages()
    {
        /*
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
                        _wnd.BeginInvoke(new Action(() => _wnd.AddPlayer(Players.Get(msg.Address))));
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

    private void DoTiming()
    {
        if (CurrentTime - _lastTime >= TimeSpan.TicksPerMillisecond)
        {
            if (Timed && State != GameState.Waiting)
            {
                _timeRemaining -= TimeSpan.FromTicks(CurrentTime - _lastTime).Milliseconds;
            }
            _lastTime = CurrentTime;

            if (State == GameState.Ready)
            {
                //Announcer.CountFromThree(_countdown);
            }
            if (State == GameState.Active)
            {
                //Announcer.CountFromFive(_countdown);
            }


            if (_timeRemaining < _countdown * 1000)
            {
                Announcer.ChangeSprite(_countdown--);
            }

            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                switch (State)
                {
                    case GameState.Waiting:
                        //if (Rules.CanStart)
                        {
                            State = GameState.Ready;
                            _countdown = 3;
                            _timeRemaining = ReadyPeriod;
                            //Logger.Log(LogEvents.GameBegan);
                        }
                        break;
                    case GameState.Active:
                        //Logger.Log(LogEvents.RoundEnded, Round);
                        if (Round == Rounds)
                        {
                            State = GameState.Complete;
                            Announcer.ChangeSprite(8);
                            //Logger.Log(LogEvents.GameEnded);
                            //Announcer.Speak(Phrase.Finished, true);
                        }
                        else
                        {
                            State = GameState.Timeout;
                            Announcer.ChangeSprite(7);
                            _timeRemaining = TimeoutPeriod;
                            //Announcer.Speak(Phrase.Time);
                        }
                        break;
                    case GameState.Timeout:
                        State = GameState.Ready;
                        Announcer.ChangeSprite(6);
                        _countdown = 3;
                        _timeRemaining = ReadyPeriod;
                        //Announcer.Speak(Phrase.Ready);
                        break;
                    case GameState.Ready:
                        Round++;
                        State = GameState.Active;
                        _countdown = 5;
                        Announcer.ChangeSprite(0);
                        _timeRemaining = RoundLength;
                        //Logger.Log(LogEvents.RoundBegan, Round);
                        //Announcer.Speak(Phrase.Go);
                        break;
                }
            }

            //Players.VerifyHeartbeats(_currentTime);
            //Spell.ClearExpired(CurrentTime);

            //if (Rules.GameIsOver && State != GameState.Waiting) // Did somebody win?
            //{
            //_countdown = 0;
            //State = GameState.Complete;
            //UpdateScreen();
            //Announcer.Speak(Phrase.Finished, true);
            //}

            UpdateScreen();
        }
    }

    private static void Wrapup()
    {
        //Announcer.Speak(Phrase.TheWinnerIs, true);
        //Announcer.Speak(Phrase.BlueTeam, true);
        //Announce winner(s), do levelups, save to db, play again?, exit
    }

    private void UpdateScreen()
    {
        HUDPanelBehavior.UpdatePanel(_timeRemaining, Round);

        /*
        foreach (PlayerPanel c in Controls.OfType<PlayerPanel>())
        {
            c.UpdateData();
        }

        if (Logger.HasData)
        {
            tbLog.AppendText(Logger.Data);
        }
        
    }*/
}
