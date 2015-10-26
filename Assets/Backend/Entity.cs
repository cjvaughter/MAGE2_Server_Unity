//using System.Drawing;

public enum TeamColor : byte
{
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Purple,
}

public enum EntityState : byte
{
    Alive,
    Dead,
    Stunned,
}

public class Entity
{
    public string Name { get; set; }
    public TeamColor Team { get; set; }
    public EntityState State { get; set; }
}

public class Player : Entity
{
    int _health;
    public Player() { }
    public Player(string name, ushort id, /*Image picture,*/ TeamColor team, int level, int xp, int strength, int defense, int luck, int maxhealth, int levelspending)
    {
        Name = name;
        ID = id;
        //Picture = picture;
        Team = team;
        Level = level;
        XP = xp;
        Strength = strength;
        Defense = defense;
        Luck = luck;
        Health = MaxHealth = maxhealth;
        LevelsPending = levelspending;
        State = EntityState.Alive;
    }

    public ushort ID { get; private set; }
    //public Image Picture { get; private set; }

    public int Level { get; set; }
    public int LevelsPending { get; set; }
    public int XP { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Luck { get; set; }
    public int MaxHealth { get; set; }

    public int Hits { get; set; }
    public int Misses { get; set; }
    public float HM { get { if (Misses == 0) return 0; return (float)Hits / Misses; } }
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public float KD { get { if (Deaths == 0) return 0; return (float)Kills/Deaths; } }

    public int Health { get { return _health; } set { if (value <= 0) { _health = 0; State = EntityState.Dead; } else { _health = value; State = EntityState.Alive; } } }
    public ulong Address { get; set; }
    public long Heartbeat { get; set; }
    public bool Connected { get; set; }

    public Device Device { get; set; }
}

public class Team : Entity
{

}
