using System;
using System.Collections.Generic;
using System.Linq;

public static class Players
{
    const long HeartbeatTimeout = TimeSpan.TicksPerSecond * 6;

    public static List<Player> PlayerList = new List<Player>();

    public static Player Add(MAGEMsg msg)
    {
        ushort playerID = (ushort)((msg.Data[1] << 8) | msg.Data[2]);
        ushort deviceID = (ushort)((msg.Data[3] << 8) | msg.Data[4]);
        Player p = Database.GetPlayer(playerID);
        Device d = Devices.Get(deviceID);
        if (p != null && d != null)
        {
            p.Device = d;
            p.Address = msg.Address;
            p.Connected = true;
            if (!Game.Rules.TeamBased)
            {
                p.Team = Colors.Red;
            }
            PlayerList.Add(p);
            Teams.Add(p.Team);
            p.CreatePanel();
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
    public static int Remaining { get { return PlayerList.Count(p => p.State != EntityState.Dead); } }
    public static Player Survivor { get { return Remaining == 1 ? PlayerList.FirstOrDefault(p => p.State != EntityState.Dead) : null; } }

    public static void VerifyHeartbeats()
    {
        foreach (Player p in PlayerList.Where(p => p.Connected && Game.CurrentTime - p.Heartbeat > HeartbeatTimeout))
        {
            if (p.Address < 0xAAAA) p.Heartbeat = Game.CurrentTime;
            else
            {
                p.Connected = false;
                Logger.Log(LogEvents.LostConnection, p);
            }
        }

        foreach (Player p in PlayerList.Where(p => !p.Connected && Game.CurrentTime - p.Heartbeat > HeartbeatTimeout))
        {
            p.Heartbeat = Game.CurrentTime;
            Coordinator.SendMessage(p.Address, (byte)MsgFunc.Connect, (byte)p.Team);
            Coordinator.UpdatePlayer(p);
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
            if(p.State == EntityState.Damaged || p.State == EntityState.Healed) p.State = EntityState.Alive;

            if (!(p.State == EntityState.Alive || p.State == EntityState.Stunned))
            {
                p.KillEffect();
                Coordinator.UpdatePlayer(p);
            }
            else if(!p.ActiveEffect.SecondaryComplete)
            {
                switch(p.ActiveEffect.SecondaryEffect)
                {
                    case SpellEffect.Repeat:
                        if(p.ActiveEffect.RepeatCount < p.ActiveEffect.SecondaryValue)
                        {
                            p.ActiveEffect.RepeatCount++;
                            p.Health -= p.ActiveEffect.PrimaryValue;
                            if (p.Health > 0)
                            {
                                p.ActiveEffect.ExpireTime = 1;
                                Coordinator.SendMessage(p.Address, (byte)MsgFunc.Health, (byte)((float)p.Health / p.MaxHealth * 100), (byte)MsgFunc.State, (byte)p.State, (byte)MsgFunc.Update);
                            }
                            else
                            {
                                p.KillEffect();
                                Coordinator.UpdatePlayer(p);
                            }
                        }
                        else
                            p.ActiveEffect.SecondaryComplete = true;
                        break;
                    case SpellEffect.Stun:
                        p.State = EntityState.Stunned;
                        p.UpdateBorder();
                        p.ActiveEffect.ExpireTime = p.ActiveEffect.SecondaryValue;
                        p.ActiveEffect.SecondaryComplete = true;
                        break;
                    default:
                        p.ActiveEffect.SecondaryComplete = true;
                        break;
                }
            }
            else if(!p.ActiveEffect.TemporaryComplete)
            {
                p.State = EntityState.Alive;
                p.UpdateBorder();
                p.ActiveEffect.ExpireTime = p.ActiveEffect.TemporaryLength;
                p.ActiveEffect.TemporaryComplete = true;
            }
            else
            {
                p.KillEffect();
                p.State = EntityState.Alive;
                Coordinator.UpdatePlayer(p);
            }
        }
    }

    public static void ResetPlayers()
    {
        foreach (Player p in PlayerList)
        {
            p.Health = p.MaxHealth;
            p.State = EntityState.Alive;
            p.Restore();
            p.KillEffect();
        }
    }
}
