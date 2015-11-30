using System;
using System.IO;
using UnityEngine.UI;

namespace Legacy
{
    public enum LogEvents : byte
    {
        //Game events
        ServerInitialized,
        InvalidPlayer,
        InvalidDevice,
        InvalidMessage,
        GameQuit,

        //Player events
        Connected,
        Reconnected,
        LostConnection,
        Died,

        //PvP events
        WasHit,
    }

    public static class Logger
    {
        public static ScrollRect LogScroll;
        public static Text LogPanel;

        const int Logwidth = 40;
        private static string _log = "";
        private static int _lineCount = 0;

        public static void Initialize()
        {
            _log = "";
            LogPanel.text = "";
            _lineCount = 0;
        }

        //Game events
        public static void Log(LogEvents e, int round = 0)
        {
            switch (e)
            {
                case LogEvents.ServerInitialized:
                    Log("Server initialized", false);
                    break;
                case LogEvents.InvalidMessage:
                    Log("Unknown message received");
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
                case LogEvents.Died:
                    Log(entity.Name + " died");
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
            }
        }

        public static void Log(string data, bool newline = true)
        {
            DateTime now = new DateTime(Game.CurrentTime);
            string log = now.ToString("[HH:mm:ss.fff] ") + data;
            if (newline) log = "\r\n" + log;
            _log += log;
            if (_lineCount >= 100)
            {
                LogPanel.text = LogPanel.text.Substring(LogPanel.text.IndexOf("\r\n") + 2);
                _lineCount--;
            }
            LogPanel.text += log;
            //LogScroll.verticalNormalizedPosition = 0;
            LogScroll.verticalScrollbar.value = 0;
            _lineCount++;
        }
    }
}
