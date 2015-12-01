using UnityEngine;

public enum TeamColor : byte
{
    Red,
    Yellow,
    Green,
    Blue,
}

public enum EntityState : byte
{
    Alive,
    Damaged,
    Stunned,
    Healed,
    Dead,
}

public class Entity
{
    public string Name { get; set; }
    public Colors Team { get; set; }
    public EntityState State { get; set; }
    //public Spell ActiveEffect { get; set; }
}

public class Player : Entity
{
    public Player() { }
    public Player(string name, ushort id, byte[] picture, Colors team, int level, int xp, int strength, int defense, int luck, int maxhealth, int levelspending)
    {
        Name = name;
        ID = id;
        Picture = new Texture2D(1, 1);
        Picture.LoadImage(picture);
        Team = team;
        Level = level;
        XP = xp;
        Strength = OriginalStrength = strength;
        Defense = OriginalDefense = defense;
        Luck = OriginalLuck = luck;
        Health = MaxHealth = maxhealth;
        LevelsPending = levelspending;
        State = EntityState.Alive;
    }

    public void Restore()
    {
        Strength = OriginalStrength;
        Defense = OriginalDefense;
        Luck = OriginalLuck;
    }

    public void CreatePanel()
    {
        GameObject playerPanel = GameObject.Instantiate(Resources.Load("Prefabs/PlayerPanel")) as GameObject;
        Panel = playerPanel.GetComponent<PanelBehavior>();
        Panel.SetPlayer(this);
    }

    public PanelBehavior Panel { get; private set; }

    public ushort ID { get; private set; }
    public Texture2D Picture { get; private set; }

    public int Level { get; set; }
    public int LevelsPending { get; set; }
    public int XP { get; set; }
    public int MaxHealth { get; set; }

    private int _strength, _defense, _luck;
    public int Strength { get { return _strength; } set { _strength = value; if (_strength < 0) _strength = 0; } }
    public int Defense { get { return _defense; } set { _defense = value; if (_defense < 0) _defense = 0; } }
    public int Luck { get { return _luck; } set { _luck = value; if (_luck < 0) _luck = 0; } }

    public int OriginalStrength { get; set; }
    public int OriginalDefense { get; set; }
    public int OriginalLuck { get; set; }

    public int Hits { get; set; }
    public int Misses { get; set; }
    public float HM { get { if (Misses == 0) return 0; return (float)Hits / Misses; } }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public float KD { get { if (Deaths == 0) return 0; return (float)Kills/Deaths; } }

    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            if (value <= 0) { _health = 0; State = EntityState.Dead; }
            else
            {
                _health = value;
                if (_health > MaxHealth)
                    _health = MaxHealth;
            }
        }
    }
    public float HealthPercent { get { return (Health * 100.0f / MaxHealth); } }
    public byte HealthNormalized { get { return (byte)(Health * 255.0f / MaxHealth); } }

    private EntityState _state;
    public new EntityState State
    {
        get { return _state; }
        set
        {
            if (_state == value) return;
            _state = value;

            if (_health == 0) _state = EntityState.Dead;

            if(_state == EntityState.Dead)
            {
                Deaths++;
                if (ActiveEffect != null)
                {
                    ActiveEffect.Caster.Kills++;
                    ActiveEffect.Caster.XP += 100;
                }
                Logger.Log(LogEvents.Died, this);
            }
            else if(_state == EntityState.Alive)
            {
                KillBorder();
            }
        }
    }

    private Spell _activeEffect;
    public Spell ActiveEffect
    {
        get { return _activeEffect; }
        set
        {
            if (_activeEffect != null)
            {
                State = EntityState.Alive;
            }
            _activeEffect = value;
        }
    }
    public void KillEffect()
    {
        _activeEffect = null;
        Panel.KillEffect();
    }

    public void KillBorder()
    {
        Panel.KillBorder();
    }

    public void UpdateBorder()
    {
        Panel.UpdateBorder();
    }

    public ulong Address { get; set; }
    public long Heartbeat { get; set; }
    public bool Connected { get; set; }

    public Device Device { get; set; }
}

public class Team : Entity
{
    public Team(Colors c)
    {
        Name = c.ToString() + " Team";
        Team = c;
        State = EntityState.Alive;
    }
}
