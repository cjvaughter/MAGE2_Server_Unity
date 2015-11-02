using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public enum LogEvents : byte
{
    //Game events
    ServerInitialized,
    GameBegan,
    GameEnded,
    GamePaused,
    GameUnpaused,
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
    public static ScrollRect LogScroll;
    public static Text LogPanel;

    const int Logwidth = 40;
    private static string _log = "";

    public static void Initialize()
    {
        _log = "";
        LogPanel.text = "";
    }

    public static void Begin(GameType type, int players, int rounds, int milliseconds)
    {
        int seconds = milliseconds / 1000;
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
                Log("Server initialized", false);
                break;
            case LogEvents.GameBegan:
                Log("Game started");
                break;
            case LogEvents.GameEnded:
                Log("Game over");
                break;
            case LogEvents.GamePaused:
                Log("Game paused");
                break;
            case LogEvents.GameUnpaused:
                Log("Game resumed");
                break;
            case LogEvents.RoundBegan:
                Log("Round " + round);
                break;
            case LogEvents.RoundEnded:
                Log("Round " + round + " ended");
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

    //Connecr Errors
    public static void Log(LogEvents e, ushort id)
    {
        switch (e)
        {
            case LogEvents.InvalidPlayer:
                Log("Unknown player " + id.ToString("X4") + " - connection refused");
                break;
            case LogEvents.InvalidDevice:
                Log("Unknown device " + id.ToString("X4") + " - connection refused");
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

    public static void Log(string data, bool newline = true)
    {
        DateTime now = new DateTime(Game.CurrentTime);
        string log = now.ToString("[HH:mm:ss.fff] ") + data;
        if (newline) log = "\r\n" + log;
        _log += log;
        LogPanel.text += log;
        LogScroll.verticalNormalizedPosition = 0;
    }

    public static void Save()
    {
        Directory.CreateDirectory(".\\Logs");
        int count = 1;
        string path = ".\\Logs\\MAGE2_" + Game.Now.ToString("MM.dd.yy") + "_1.log";
        while (File.Exists(path))
        {
            path = ".\\Logs\\MAGE2_" + Game.Now.ToString("MM.dd.yy") + "_" + count++ + ".log";
        }
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.Write(_log);
        }
    }
}

