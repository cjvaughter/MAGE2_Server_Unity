using System;
using System.Collections.Generic;
using System.Linq;

namespace Legacy
{
    public static class Players
    {
        const long HeartbeatTimeout = TimeSpan.TicksPerSecond * 300;

        public static List<Player> PlayerList = new List<Player>();

        public static Player Add(MAGEMsg msg)
        {
            ushort playerID = (ushort)msg.Address;
            Player p = Database.GetPlayer(playerID);
            if (p != null)
            {
                p.Address = msg.Address;
                p.Connected = true;
                PlayerList.Add(p);
                p.CreatePanel();
                Logger.Log(LogEvents.Connected, p);
                return p;
            }
            if (p == null) Logger.Log(LogEvents.InvalidPlayer, playerID);
            return null;
        }

        public static void Remove(Player p) { PlayerList.Remove(p); }
        public static void Remove(ushort id) { PlayerList.RemoveAll(p => p.ID == id); }
        public static bool Exists(ushort id) { return PlayerList.Exists(p => p.ID == id); }
        public static bool Exists(ulong address) { return PlayerList.Exists(p => p.Address == address); }
        public static Player Get(ushort id) { return PlayerList.Find(p => p.ID == id); }
        public static Player Get(ulong address) { return PlayerList.Find(p => p.Address == address); }
        public static void Clear() { PlayerList.Clear(); }
        public static int Count { get { return PlayerList.FindAll(p => p.GetType() == typeof(Player)).Count; } }

        public static void VerifyHeartbeats()
        {
            foreach (Player p in PlayerList.Where(p => p.Connected && Game.CurrentTime - p.Heartbeat > HeartbeatTimeout))
            {
                p.Connected = false;
                Logger.Log(LogEvents.LostConnection, p);
            }
        }

        public static void ResetPlayers()
        {
            foreach (Player p in PlayerList)
            {
                p.Health = p.MaxHealth;
                p.State = EntityState.Alive;
                p.Restore();
            }
        }
    }
}
