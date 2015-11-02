using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public enum SpellType : byte
{
    GenericDamage,      //Just damage
    GenericDamageTeam,
    GenericStun,        //Just stun
    GenericStunTeam,
    GenericHeal,        //Just heal
    GenericHealTeam,
    Fire,               //Damage, over a period
    FireTeam,
    Water,              //
    WaterTeam,
    Earth,              //
    EarthTeam,
    Wind,                //
    WindTeam,
    Ice,                //Damage, also stun
    IceTeam,
    Rock,               //Damage, stun, 
    RockTeam,
    Thunder,           //Damage, stun, lowered strength over a period, 
    ThunderTeam,
    Poison,             //Damage, lowers strength and defense for a period
    PoisonTeam,
    Psychic,            //Damage, sets luck to 0 for a period
    PsychicTeam,
    Ghost,              //Damage, sets strength to 0 for a period
    GhostTeam,
    Shadow,             //Damage, sets defense to 0 for a period
    ShadowTeam,
    Light,              //Damage, sets strength, defense, and luck to 0 for a period
    LightTeam,
    //Add new spells here
    NumberOfSpells
}
/*
public enum SpellEffect
{
    Health,
    Stun,
    Strength,
    Defense,
    Luck,
}
*/
[SuppressMessage("ReSharper", "RedundantExplicitArraySize")]
public static class Spell
{
    /*
    public SpellType Type { get; set; }
    public SpellEffect Primary { get; set; }
    public SpellEffect Secondary { get; set; }
    public SpellEffect Temporary { get; set; }
    public bool Overridable { get; set; }
    public long Length { get; set; }*/

    public const long SpellTimeout = TimeSpan.TicksPerSecond*2;
    public const int MaxChance = 100;
    static List<IRPacket> SpellQueue = new List<IRPacket>();
    static Random Chance = new Random();

    public static readonly int[,] DamageMatrix = new int[,]
    {
            //Nor, Fir, Wat, Ear, Air, Ice, Roc, Ele, Poi, Psy, Gho, Sha, Lig 
/*Normal*/  {100, 100, 100, 100, 100, 100,  50, 100, 100, 100,   0, 100, 100},
/*Fire*/    {100,  50,  50, 100, 100, 200,  50, 100, 100, 100, 100, 100, 100},
/*Water*/   {100, 200,  50, 200, 100, 100, 200, 100, 100, 100, 100, 100, 100},
/*Earth*/   {100, 200, 100, 100,   0, 100, 200, 200, 200, 100, 100, 100, 100},
/*Air*/     {100, 100, 100, 100, 100, 100,  50,  50, 100, 100, 100, 100, 100},
/*Ice*/     {100,  50,  50, 200, 200,  50, 100, 100, 100, 100, 100, 100, 100},
/*Rock*/    {100, 200, 100,  50, 200, 200, 100, 100, 100, 100, 100, 100, 100},
/*Electric*/{100, 100, 200,   0, 200, 100, 100,  50, 100, 100, 100, 100, 100},
/*Poison*/  {100, 100, 100,  50, 100, 100,  50, 100,  50, 100,  50, 100, 100},
/*Psychic*/ {100, 100, 100, 100, 100, 100, 100, 100, 200,  50, 100, 100, 100},
/*Ghost*/   {100, 100, 100, 100, 100, 100, 100, 100, 100, 200, 200, 100, 100},
/*Shadow*/  {100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},
/*Light*/   {100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},  
    };

    public static void Add(Player caster, byte[] data)
    {
        SpellQueue.Add(new IRPacket() {ID = caster.ID, Spell = (SpellType)data[1], Unique = data[2], Timestamp = Game.CurrentTime });
    }

    public static void Process(Player defender, byte[] data)
    {
        IRPacket spell = SpellQueue.Find(packet => packet.ID == (data[1] << 8 | data[2]) && packet.Spell == (SpellType)data[3] && packet.Unique == data[4]);
        if (spell != null)
        {
            SpellQueue.Remove(spell);
            DetermineSuccess(defender, spell);
        }
    }

    public static void ClearExpired()
    {
        List<IRPacket> expired = SpellQueue.FindAll(packet => Game.CurrentTime - packet.Timestamp >= SpellTimeout);
        foreach(IRPacket spell in expired)
        {
            SpellQueue.Remove(spell);
            Players.Get(spell.ID).Misses++;
        }
    }

    /// <summary>
    /// Calculates odds of success, carries out spell effect,
    /// awards XP, and increments hits or misses.
    /// </summary>
    /// <param name="defender">The receiving player</param>
    /// <param name="packet">The received packet</param>
    /// <returns>Whether the spell was successful or not.</returns>
    private static void DetermineSuccess(Player defender, IRPacket packet)
    {
        Player caster = Players.Get(packet.ID);

        float odds = ((float)caster.Strength / (caster.Strength + defender.Defense)) * MaxChance;
        odds += Chance.Next(caster.Luck);
        odds -= Chance.Next(defender.Luck);

#if RELEASE
        bool success = odds >= Chance.Next(MaxChance);
#elif DEBUG
        bool success = true;
#endif

        if (success)
        {
            switch(packet.Spell)
            {
                case SpellType.GenericDamage:
                    defender.Health -= 5;
                    Coordinator.SendMessage(new MAGEMsg(defender.Address, new[] { (byte)MsgFunc.Health, (byte)((float)defender.Health / defender.MaxHealth * 100), (byte)MsgFunc.UpdateDisplay, (byte)Colors.NoColor }));
                    break;
                case SpellType.GenericStun:
                    defender.State = EntityState.Stunned;
                    defender.ActiveEffect = new SpellEffect(3000);
                    Coordinator.SendMessage(new MAGEMsg(defender.Address, new[] { (byte)MsgFunc.State, (byte)defender.State, (byte)MsgFunc.UpdateDisplay, (byte)Colors.Blue }));
                    break;
                case SpellType.GenericHeal:
                    defender.Health += 5;
                    Coordinator.SendMessage(new MAGEMsg(defender.Address, new[] { (byte)MsgFunc.Health, (byte)((float)defender.Health / defender.MaxHealth * 100), (byte)MsgFunc.UpdateDisplay, (byte)Colors.Green }));
                    break;
                case SpellType.Fire:

                    break;
            }
            caster.Hits++;
            Logger.Log(LogEvents.WasHit, caster, defender, packet.Spell);
            //int strength = src.Strength * (float)(DamageMatrix[src.Device.Type, dst.Device.Type])/100;
        }
        else
        {
            defender.XP += 5; //XP for resisting
            Logger.Log(LogEvents.Resisted, caster, defender, packet.Spell);
        }
    }
}

public class SpellEffect
{
    //public 
    public long ExpireTime { get; set; }
    public SpellEffect(int lengthMillis)
    {
        ExpireTime = Game.CurrentTime + TimeSpan.TicksPerMillisecond * lengthMillis;
    }
}
