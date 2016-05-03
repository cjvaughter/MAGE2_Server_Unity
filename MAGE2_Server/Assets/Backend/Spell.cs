using System;
using System.Collections.Generic;

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
    public bool SecondaryComplete { get; set; }

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
            if (PrimaryEffect != SpellEffect.Stun)
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
        SecondaryComplete = false;
        TemporaryComplete = true;
        TemporaryLength = 0;
        StrengthModifier = 0;
        DefenseModifier = 0;
        LuckModifier = 0;
    }






    public const long SpellTimeout = TimeSpan.TicksPerSecond * 2;
    public const int MaxChance = 100;
    static List<IRPacket> SpellQueue = new List<IRPacket>();
    static Random Chance = new Random();

    private static readonly int[][] DamageMatrix = {
                  //Nor, Fir, Wat, Ear, Air, Ice, Roc, Ele, Poi, Psy, Gho, Sha, Lig 
/*Normal*/  new [] {100, 100, 100, 100, 100, 100,  50, 100, 100, 100,   0, 100, 100},
/*Fire*/    new [] {100,  50,  50, 100, 100, 200,  50, 100, 100, 100, 100, 100, 100},
/*Water*/   new [] {100, 200,  50, 200, 100, 100, 200, 100, 100, 100, 100, 100, 100},
/*Earth*/   new [] {100, 200, 100, 100,   0, 100, 200, 200, 200, 100, 100, 100, 100},
/*Air*/     new [] {100, 100, 100, 100, 100, 100,  50,  50, 100, 100, 100, 100, 100},
/*Ice*/     new [] {100,  50,  50, 200, 200,  50, 100, 100, 100, 100, 100, 100, 100},
/*Rock*/    new [] {100, 200, 100,  50, 200, 200, 100, 100, 100, 100, 100, 100, 100},
/*Electric*/new [] {100, 100, 200,   0, 200, 100, 100,  50, 100, 100, 100, 100, 100},
/*Poison*/  new [] {100, 100, 100,  50, 100, 100,  50, 100,  50, 100,  50, 100, 100},
/*Psychic*/ new [] {100, 100, 100, 100, 100, 100, 100, 100, 200,  50, 100, 100, 100},
/*Ghost*/   new [] {100, 100, 100, 100, 100, 100, 100, 100, 100, 200, 200, 100, 100},
/*Shadow*/  new [] {100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},
/*Light*/   new [] {100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100},
    };

    public static void Add(Player caster, byte[] data)
    {
        SpellQueue.Add(new IRPacket() { ID = caster.ID, Unique = data[1], Timestamp = Game.CurrentTime });
    }

    public static void Process(Player defender, byte[] data)
    {
        IRPacket spell = new IRPacket() { ID = (ushort)(data[1] << 8 | data[2]), Spell = (SpellType)data[3], Strength = data[4], Unique = data[5] };

        IRPacket queuedSpell = SpellQueue.Find(packet => packet.ID == spell.ID && packet.Unique == spell.Unique);
        if (queuedSpell != null)
        {
            if (defender.ID == queuedSpell.ID && Game.Type != GameType.TestMode)
                return; //can't hit self
            SpellQueue.Remove(queuedSpell);
            if (defender.ActiveEffect == null)
                DetermineSuccess(defender, spell);
            else if (defender.ActiveEffect.Overridable)
                DetermineSuccess(defender, spell);
        }
    }

    public static void ForceProcess(Player defender, byte[] data)
    {
        IRPacket spell = new IRPacket() { Spell = (SpellType)data[1], Strength = data[2] };

        if (defender.ActiveEffect == null)
            ForceDetermineSuccess(defender, spell);
        else if (defender.ActiveEffect.Overridable)
            ForceDetermineSuccess(defender, spell);
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

    public static void DetermineSuccess(Player defender, IRPacket packet, bool force = false)
    {
        if(force)
        {
            Player god = new Player() { Name = "God", Strength = 1, Device = new Device() { Type = DeviceType.Generic } };
            if (packet.Spell.ToString().Contains("Team"))
            {
                packet.Spell = (SpellType)Enum.Parse(typeof(SpellType), packet.Spell.ToString().Replace("Team", ""), true);

                Logger.Log(LogEvents.WasHit, god, new Entity() { Name = "Everyone" }, packet.Spell);
                foreach (Player p in Teams.GetPlayers(defender.Team))
                {
                    DoSpell(p, god, packet, true);
                }
            }
            else
            {
                DoSpell(defender, god, packet);
            }
            return;
        }

        Player caster = Players.Get(packet.ID);

        float odds = ((float)caster.Strength / (caster.Strength + defender.Defense)) * MaxChance;
        odds += Chance.Next(caster.Luck);
        odds -= Chance.Next(defender.Luck);
        bool success = odds >= Chance.Next(MaxChance);

        //disable resisting spells
        success = true;

        if (success || Game.Type == GameType.TestMode)
        {
            caster.Hits++;

            if(packet.Spell.ToString().Contains("Team"))
            {
                caster.XP += 50;
                Team team = Teams.Get(defender.Team);
                packet.Spell = (SpellType)Enum.Parse(typeof(SpellType), packet.Spell.ToString().Replace("Team", ""), true);

                Logger.Log(LogEvents.WasHit, caster, team, packet.Spell);
                foreach (Player p in Teams.GetPlayers(defender.Team))
                {
                    DoSpell(p, caster, packet, true);
                }
            }
            else
            {
                caster.XP += 10;
                DoSpell(defender, caster, packet);
            }
        }
        else
        {
            defender.XP += 5; //XP for resisting
            Logger.Log(LogEvents.Resisted, caster, defender, packet.Spell);
        }
    }

    public static void ForceDetermineSuccess(Player defender, IRPacket packet)
    {
        Player caster = defender;

        caster.Hits++;
        caster.XP++;

        if (packet.Spell.ToString().Contains("Team"))
        {
            Team team = Teams.Get(defender.Team);
            packet.Spell = (SpellType)Enum.Parse(typeof(SpellType), packet.Spell.ToString().Replace("Team", ""), true);

            Logger.Log(LogEvents.WasHit, caster, team, packet.Spell);
            foreach (Player p in Teams.GetPlayers(defender.Team))
            {
                DoSpell(p, caster, packet, true);
            }
        }
        else
        {
            DoSpell(defender, caster, packet);
        }

    }

    public static bool DoSpell(Player defender, Player caster, IRPacket packet, bool team = false)
    {
        if(defender.ActiveEffect != null)
        {
            if (defender.ActiveEffect.Overridable == false)
                return false;
            defender.KillEffect();
        }

        Spell spell = Activator.CreateInstance(Type.GetType(packet.Spell.ToString()), caster) as Spell;

        if (defender.State == EntityState.Dead && spell.PrimaryEffect != SpellEffect.Heal)
        {
            return false;
        }

        if(!team) Logger.Log(LogEvents.WasHit, caster, defender, packet.Spell);

        //disable variable strength
        //int strength = (int)(caster.Strength * (float)(DamageMatrix[(byte)caster.Device.Type, (byte)defender.Device.Type])/100.0f);
        //spell.Multiplier = (packet.Strength/100.0f) * (caster.Strength*caster.Level/10.0f) * (DamageMatrix[(byte)caster.Device.Type][(byte)defender.Device.Type]/100.0f);
        spell.Multiplier = 1f;

        switch (spell.PrimaryEffect)
        {
            case SpellEffect.Damage:
                defender.State = EntityState.Damaged;
                defender.Health -= spell.PrimaryValue;
                break;
            case SpellEffect.Stun:
                defender.State = EntityState.Stunned;
                spell.ExpireTime = spell.PrimaryValue;
                break;
            case SpellEffect.Heal:
                defender.Health += spell.PrimaryValue;
                defender.State = EntityState.Healed;
                break;
        }

        switch (spell.SecondaryEffect)
        {
            case SpellEffect.Damage:
                defender.Health -= spell.SecondaryValue;
                break;
        }

        defender.Restore(); //Set strength defense and luck back to normal

        if (defender.State == EntityState.Dead)
        {
            caster.Kills++;
        }
        else
        {
            defender.Strength += spell.StrengthModifier;
            defender.Defense += spell.DefenseModifier;
            defender.Luck += spell.LuckModifier;
            defender.ActiveEffect = spell;
        }

        Coordinator.UpdatePlayer(defender);
        return true;
    }
}
