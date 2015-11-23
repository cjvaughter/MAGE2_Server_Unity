using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum SpellType : byte
{
    Damage,      //Just damage
    DamageTeam,
    Stun,        //Just stun
    StunTeam,
    Heal,        //Just heal
    HealTeam,
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

public enum SpellEffect : byte
{
    None,
    Damage,
    Stun,
    Heal,
    Repeat
}

public class Spell
{
    public Player Caster { get; protected set; }

    private long _expireTime;
    public long ExpireTime
    {
        get { return _expireTime; }
        set { _expireTime = Game.CurrentTime + value * TimeSpan.TicksPerSecond; }
    }
    public bool Overridable { get; protected set; }

    public Colors Color { get; protected set; }
    public SpellEffect PrimaryEffect { get; protected set; }
    public int PrimaryValue { get; protected set; }
    public SpellEffect SecondaryEffect { get; protected set; }
    public int SecondaryValue { get; protected set; }
    public int RepeatCount { get; set; }

    public bool TemporaryComplete { get; set; }
    public int TemporaryLength { get; protected set; }
    public int StrengthModifier { get; protected set; }
    public int DefenseModifier { get; protected set; }
    public int LuckModifier { get; protected set; }

    private float _multiplier;
    public float Multiplier
    {
        get { return _multiplier; }
        set
        {
            _multiplier = value;
            PrimaryValue = (int)(PrimaryValue * _multiplier);
            if(SecondaryEffect != SpellEffect.Repeat)
                SecondaryValue = (int)(SecondaryValue * _multiplier);
            StrengthModifier = (int)(StrengthModifier * _multiplier);
            DefenseModifier = (int)(DefenseModifier * _multiplier);
            LuckModifier = (int)(LuckModifier * _multiplier);
        }
    }

    public Spell(Player caster)
    {
        Caster = caster;
        ExpireTime = 1;
        Overridable = true;
        Color = Colors.NoColor;
        PrimaryEffect = SpellEffect.None;
        PrimaryValue = 5;
        SecondaryEffect = SpellEffect.None;
        SecondaryValue = 0;
        RepeatCount = 1;
        TemporaryComplete = true;
        TemporaryLength = 0;
        StrengthModifier = 0;
        DefenseModifier = 0;
        LuckModifier = 0;
    }






    public const long SpellTimeout = TimeSpan.TicksPerSecond * 2;
    public const int MaxChance = 100;
    static List<IRPacket> SpellQueue = new List<IRPacket>();
    static System.Random Chance = new System.Random();

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
        SpellQueue.Add(new IRPacket() { ID = caster.ID, Unique = data[1], Timestamp = Game.CurrentTime });
    }

    public static void Process(Player defender, byte[] data)
    {
        IRPacket spell = new IRPacket() { ID = (ushort)(data[1] << 8 | data[2]), Spell = (SpellType)data[3], Strength = data[4], Unique = data[5] };

        IRPacket queuedSpell = SpellQueue.Find(packet => packet.ID == spell.ID && packet.Unique == spell.ID);
        if (queuedSpell != null)
        {
            SpellQueue.Remove(queuedSpell);
            if (defender.ActiveEffect == null)
                DetermineSuccess(defender, spell);
            else if (defender.ActiveEffect.Overridable)
                DetermineSuccess(defender, spell);
        }
    }

    public static void ClearExpired()
    {
        List<IRPacket> expired = SpellQueue.FindAll(packet => Game.CurrentTime - packet.Timestamp >= SpellTimeout);
        foreach (IRPacket spell in expired)
        {
            SpellQueue.Remove(spell);
            Players.Get(spell.ID).Misses++;
        }
    }

    public static void DetermineSuccess(Player defender, IRPacket packet, bool forceSuccess = false)
    {
        Player caster = Players.Get(packet.ID);

        float odds = ((float)caster.Strength / (caster.Strength + defender.Defense)) * MaxChance;
        odds += Chance.Next(caster.Luck);
        odds -= Chance.Next(defender.Luck);
        bool success = odds >= Chance.Next(MaxChance);

        if (success || forceSuccess)
        {
            caster.Hits++;
            caster.XP += 10;
            Logger.Log(LogEvents.WasHit, caster, defender, packet.Spell);

            defender.ActiveEffect = Activator.CreateInstance(Type.GetType(packet.Spell.ToString()), caster) as Spell;
            int strength = (int)(caster.Strength * (float)(DamageMatrix[(byte)caster.Device.Type, (byte)defender.Device.Type])/100.0f);
            defender.ActiveEffect.Multiplier = (packet.Strength/100.0f) * (caster.Strength/100.0f*caster.Level) * (DamageMatrix[(byte)caster.Device.Type, (byte)defender.Device.Type]/100.0f);
            
            switch(defender.ActiveEffect.PrimaryEffect)
            {
                case SpellEffect.Damage:
                    defender.State = EntityState.Damaged;
                    defender.Health -= defender.ActiveEffect.PrimaryValue;
                    break;
                case SpellEffect.Stun:
                    defender.State = EntityState.Stunned;
                    defender.ActiveEffect.ExpireTime = defender.ActiveEffect.PrimaryValue;
                    break;
                case SpellEffect.Heal:
                    defender.State = EntityState.Healed;
                    defender.Health += defender.ActiveEffect.PrimaryValue;
                    break;
            }

            switch(defender.ActiveEffect.SecondaryEffect)
            {
                case SpellEffect.Damage:
                    defender.Health -= defender.ActiveEffect.SecondaryValue;
                    break;
            }

            defender.Restore(); //Set strength defense and luck back to normal
            defender.Strength += defender.ActiveEffect.StrengthModifier;
            defender.Defense += defender.ActiveEffect.DefenseModifier;
            defender.Luck += defender.ActiveEffect.LuckModifier;

            Coordinator.UpdatePlayer(defender);
        }
        else
        {
            defender.XP += 5; //XP for resisting
            Logger.Log(LogEvents.Resisted, caster, defender, packet.Spell);
        }
    }
}
