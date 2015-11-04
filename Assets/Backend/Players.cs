using System;
using System.Collections.Generic;
using System.Linq;

public static class Players
{
    const long HeartbeatTimeout = TimeSpan.TicksPerSecond * 6;

    public static List<Player> PlayerList = new List<Player>();

    public static Player Add(MAGEMsg msg)
    {
        ushort playerID = (ushort)((msg.Data[1] << 8) + msg.Data[2]);
        ushort deviceID = (ushort)((msg.Data[3] << 8) + msg.Data[4]);
        Player p = Database.GetPlayer(playerID);
        Device d = Database.GetDevice(deviceID);
        if (p != null && d != null)
        {
            p.Device = d;
            p.Address = msg.Address;
            p.Connected = true;
            p.CreatePanel();
            PlayerList.Add(p);
            Logger.Log(LogEvents.Connected, p);
            return p;
        }
        if (p == null) Logger.Log(LogEvents.InvalidPlayer, playerID);
        if (d == null) Logger.Log(LogEvents.InvalidDevice, deviceID);
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
    public static int Remaining { get { return PlayerList.Count(p => p.State == EntityState.Alive); } }
    public static Player Survivor { get { return Remaining == 1 ? PlayerList.FirstOrDefault(p => p.State == EntityState.Alive) : null; } }

    public static void VerifyHeartbeats()
    {
        foreach (Player p in PlayerList.Where(p => p.Connected && Game.CurrentTime - p.Heartbeat > HeartbeatTimeout))
        {
            p.Connected = false;
            Logger.Log(LogEvents.LostConnection, p);
        }
    }

    public static void ResetHeartbeats()
    {
        foreach (Player p in PlayerList.Where(p => p.Connected))
        {
            p.Heartbeat = Game.CurrentTime;
        }
    }

    public static void ClearExpiredEffects()
    {
        foreach (Player p in PlayerList.Where(p => p.ActiveEffect != null && Game.CurrentTime >= p.ActiveEffect.ExpireTime))
        {
            p.ActiveEffect = null;
            p.State = EntityState.Alive;
            Coordinator.SendMessage(p.Address, (byte)MsgFunc.State, (byte)EntityState.Alive);
        }
    }

    public static void ResetPlayers()
    {
        foreach (Player p in PlayerList)
        {
            p.State = EntityState.Alive;
            p.Health = p.MaxHealth;
        }
    }
}
