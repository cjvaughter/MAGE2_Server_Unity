using System;
using System.Collections.Generic;
using System.IO;

public enum LogEvents : byte
{
    //Game events
    ServerInitialized,
    GameBegan,
    GameEnded,
    RoundBegan,
    RoundEnded,
    InvalidPlayer,
    InvalidDevice,
    InvalidMessage,
    ConnectDuringGame,
    GameQuit,

    //Player events
    Connected,
    Reconnected,
    LostConnection,
    LeveledUp,
    Died,
    Won,

    //PvP events
    WasHit,
    Resisted,
}

public static class Logger
{
    const int Logwidth = 40;
    private static string _log = "";
    private static Queue<string> _logData = new Queue<string>();

    public static void Initialize(GameType type, int players, int rounds, int milliseconds)
    {
        int seconds = milliseconds / 1000;
        _log = "";
        _logData = new Queue<string>();
        _log += "*------------MAGE 2 EVENT LOG------------*\r\n";
        _log += "*                                        *\r\n";
        string game = type.ToString();
        int spaces = (Logwidth - game.Length)/2;
        _log += "*" + new string(' ', spaces) + game + new string(' ', Logwidth - game.Length - spaces) + "*\r\n";
        _log += "* " + players.ToString().PadLeft(3, ' ') + " Players - " + rounds.ToString().PadLeft(3, ' ') + " Rounds - " + seconds.ToString().PadLeft(3, ' ') + " Seconds *\r\n";
        _log += "*                                        *\r\n";
        _log += "*----------------------------------------*\r\n\r\n";
    }

    //Game events
    public static void Log(LogEvents e, int round = 0)
    {
        switch (e)
        {
            case LogEvents.ServerInitialized:
                Log("Server initialized");
                break;
            case LogEvents.GameBegan:
                Log("Game started");
                break;
            case LogEvents.GameEnded:
                Log("Game over");
                break;
            case LogEvents.RoundBegan:
                Log("Round " + round);
                break;
            case LogEvents.RoundEnded:
                Log("Round " + round + " ended");
                break;
            case LogEvents.InvalidPlayer:
                Log("Unknown player - connection refused");
                break;
            case LogEvents.InvalidDevice:
                Log("Unknown device - connection refused");
                break;
            case LogEvents.InvalidMessage:
                Log("Unknown message received");
                break;
            case LogEvents.ConnectDuringGame:
                Log("Player attempted to connect during game - connection refused");
                break;
            case LogEvents.GameQuit:
                Log("Game exited");
                break;
        }
    }

    //Player events
    public static void Log(LogEvents e, Entity entity)
    {
        switch (e)
        {
            case LogEvents.Connected:
                Log(entity.Name + " joined the game");
                break;
            case LogEvents.Reconnected:
                Log("Recovered connection with " + entity.Name);
                break;
            case LogEvents.LostConnection:
                Log("Lost connection with " + entity.Name);
                break;
            case LogEvents.LeveledUp:
                Log(entity.Name + " leveled up");
                break;
            case LogEvents.Died:
                Log(entity.Name + " died");
                break;
            case LogEvents.Won:
                Log(entity.Name + " won the game");
                break;
        }
    }

    //PvP events
    public static void Log(LogEvents e, Entity attacker, Entity defender, SpellType spell)
    {
        switch (e)
        {
            case LogEvents.WasHit:
                Log(attacker.Name + " hit " + defender.Name + " with " + spell);
                break;
            case LogEvents.Resisted:
                Log(defender.Name + " resisted " + spell + " from " + attacker.Name);
                break;
        }
    }

    public static void Log(string data)
    {
        DateTime now = new DateTime(Game.CurrentTime);
        string log = now.ToString("[HH:mm:ss.fff] ") + data + "\r\n";
        _log += log;
        _logData.Enqueue(log);
    }

    public static void Save()
    {
        Directory.CreateDirectory(".\\Logs");
        int count = 1;
        string path = ".\\Logs\\MAGE2_" + DateTime.Now.ToString("MM.dd.yy") + "_1.log";
        while (File.Exists(path))
        {
            path = ".\\Logs\\MAGE2_" + DateTime.Now.ToString("MM.dd.yy") + "_" + count++ + ".log";
        }
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.Write(_log);
        }
    }

    public static bool HasData
    {
        get
        {
            return _logData.Count > 0;
        }
    }

    public static string Data
    {
        get
        {
            return _logData.Dequeue();
        }
    }
}

