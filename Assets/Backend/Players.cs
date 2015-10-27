using System;
using System.Collections.Generic;
using System.Linq;

public static class Players
{
    const long HeartbeatTimeout = TimeSpan.TicksPerSecond * 6;

    public static List<Player> PlayerList = new List<Player>();

    public static bool Add(MAGEMsg msg)
    {
        Player p = Database.GetPlayer((ushort)((msg.Data[1]<<8) + msg.Data[2]));
        Device d = Database.GetDevice((ushort)((msg.Data[3]<<8) + msg.Data[4]));
        if (p != null && d != null)
        {
            p.Device = d;
            p.Address = msg.Address;
            p.Connected = true;
            p.CreatePanel();
            PlayerList.Add(p);
            Logger.Log(LogEvents.Connected, p);
            return true;
        }
        if (p == null) Logger.Log(LogEvents.InvalidPlayer);
        if (d == null) Logger.Log(LogEvents.InvalidDevice);
        return false;
    }

    public static void Remove(Player p) { PlayerList.Remove(p); }
    public static void Remove(ushort id) { PlayerList.RemoveAll(p => p.ID == id); }
    public static bool Exists(ushort id) { return PlayerList.Exists(p => p.ID == id); }
    public static bool Exists(ulong address) { return PlayerList.Exists(p => p.Address == address); }
    public static Player Get(ushort id) { return PlayerList.Find(p => p.ID == id); }
    public static Player Get(ulong address) { return PlayerList.Find(p => p.Address == address); }
    public static void Clear() { PlayerList.Clear(); }
    public static int Count { get { return PlayerList.FindAll(p => p.GetType() == typeof(Player)).Count; } }
    public static int Remaining { get { return PlayerList.Count(p => p.State == EntityState.Alive); } }
    public static Player Survivor { get { return Remaining == 1 ? PlayerList.FirstOrDefault(p => p.State == EntityState.Alive) : null; } }

    public static void VerifyHeartbeats(long time)
    {
        foreach (Player p in PlayerList.Where(p => p.Connected && time - p.Heartbeat > HeartbeatTimeout))
        {
            p.Connected = false;
            Logger.Log(LogEvents.LostConnection, p);
        }
    }
}
