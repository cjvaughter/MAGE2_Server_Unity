using System;
using System.Collections;
using System.Collections.Generic;

namespace Legacy
{
    public static class Game
    {
        static long _lastTime;
        static bool _running;

        public static IGameRules Rules { get; private set; }
        static TimeSpan UtcOffset { get; set; }
        public static DateTime Now { get { return DateTime.UtcNow + UtcOffset; } }

        public static long CurrentTime { get; private set; }

        static Game()
        {
            UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        }

        public static void Start()
        {
            CurrentTime = Now.TimeOfDay.Ticks;
            _lastTime = CurrentTime;

            Rules = new TestMode();
            HUDPanelBehavior.Initialize("Legacy", 0);
            Coordinator.Start();
            Database.Create();
            Logger.Log(LogEvents.ServerInitialized);
            _running = true;
        }

        public static IEnumerator Stop()
        {
            if (_running)
            {
                Logger.Log(LogEvents.GameQuit);
                Coordinator.Stop();
                Players.ResetPlayers(); //To kill effects
                Players.Clear();
                _running = false;
            }
            yield return null;
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

            ushort payload = 0;
            bool supported = true;

            if (p != null)
            {
                p.Heartbeat = CurrentTime;
            }

            switch ((LegacyMsgFunc)msg.Data[0])
            {
                case LegacyMsgFunc.Heartbeat:
                    if (p == null)
                    {
                        p = Players.Add(msg);
                    }
                    else if (!p.Connected)
                    {
                        p.Connected = true;
                        p.Heartbeat = CurrentTime;
                        Logger.Log(LogEvents.Reconnected, p);
                    }
                    break;
                case LegacyMsgFunc.Stat:
                    switch((Stat)msg.Data[1])
                    {
                        case Stat.PlayerID:
                            payload = p.ID;
                            break;
                        case Stat.TeamID:
                            payload = (ushort)p.Team;
                            break;
                        case Stat.PlayerLevel:
                            payload = (ushort)p.Level;
                            break;
                        case Stat.CurrentHP:
                            payload = (ushort)p.Health;
                            break;
                        case Stat.MaxHP:
                            payload = (ushort)p.MaxHealth;
                            break;
                        default:
                            supported = false;
                            break;
                    }
                    if(supported)
                        Coordinator.SendMessage(p.Address, (byte)LegacyMsgFunc.Stat, msg.Data[1], (byte)(payload >> 8), (byte)payload);
                    break;
                case LegacyMsgFunc.NormalizedStat:
                    switch((SubFunc)msg.Data[1])
                    {
                        case SubFunc.Health:
                            Coordinator.SendMessage(p.Address, (byte)LegacyMsgFunc.NormalizedStat, 0, 0, p.HealthNormalized);
                            break;
                        default:
                            Logger.Log(LogEvents.InvalidMessage);
                            break;
                    }
                    break;
                case LegacyMsgFunc.StatSet:
                    supported = false;
                    break;
                case LegacyMsgFunc.StatClear:
                    supported = false;
                    break;
                case LegacyMsgFunc.StatSetBit:
                    supported = false;
                    break;
                case LegacyMsgFunc.StatClearBit:
                    supported = false;
                    break;
                case LegacyMsgFunc.MIRP:
                    switch ((SubFunc)msg.Data[1])
                    {
                        case SubFunc.Damage:
                            p.Health -= msg.Data[3];
                            Coordinator.SendMessage(p.Address, (byte)LegacyMsgFunc.NormalizedStat, 0, 0, p.HealthNormalized);
                            break;
                        case SubFunc.Heal:
                            p.Health += msg.Data[3];
                            Coordinator.SendMessage(p.Address, (byte)LegacyMsgFunc.NormalizedStat, 0, 0, p.HealthNormalized);
                            break;
                    }
                    break;
                default:
                    Logger.Log(LogEvents.InvalidMessage);
                    break;
            }
        }

        private static void UpdateState()
        {
            if (CurrentTime - _lastTime < TimeSpan.TicksPerMillisecond) return;

            Players.VerifyHeartbeats();
        }
    }
}
