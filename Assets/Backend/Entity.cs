using UnityEngine;
using UnityEngine.UI;

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
    Dead,
    Stunned,
}

public class Entity
{
    public string Name { get; set; }
    public TeamColor Team { get; set; }
    public EntityState State { get; set; }
    //public Spell ActiveEffect { get; set; }
}

public class Player : Entity
{
    public Player() { }
    public Player(string name, ushort id, byte[] picture, TeamColor team, int level, int xp, int strength, int defense, int luck, int maxhealth, int levelspending)
    {
        Name = name;
        ID = id;
        Picture = new Texture2D(1, 1);
        Picture.LoadImage(picture);
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

    public void CreatePanel()
    {
        Panel = GameObject.Instantiate(Resources.Load("Prefabs/PlayerPanel")) as GameObject;
        Panel.transform.Find("Name").GetComponent<TextMesh>().text = Name;
        Panel.transform.Find("Player").GetComponent<MeshRenderer>().material.mainTexture = Picture;
        Panel.transform.Find("Device").GetComponent<MeshRenderer>().material.mainTexture = Device.Picture;
        PanelStatus = Panel.transform.Find("Status").GetComponent<TextMesh>();
        PanelStatus.text = State.ToString();
        HealthBar = Panel.transform.Find("Health").GetComponent<HealthBehavior>();
        if (Game.Type == GameType.TeamBattle)
            switch (Team)
            {
                case TeamColor.Red:
                    Panel.transform.Find("Back").GetComponent<MeshRenderer>().material = Resources.Load("Materials/Red") as Material;
                    break;
                case TeamColor.Yellow:
                    Panel.transform.Find("Back").GetComponent<MeshRenderer>().material = Resources.Load("Materials/Yellow") as Material;
                    Panel.transform.Find("Name").GetComponent<TextMesh>().color = Color.black;
                    PanelStatus.color = Color.black;
                    break;
                case TeamColor.Green:
                    Panel.transform.Find("Back").GetComponent<MeshRenderer>().material = Resources.Load("Materials/Green") as Material;
                    break;
                case TeamColor.Blue:
                    Panel.transform.Find("Back").GetComponent<MeshRenderer>().material = Resources.Load("Materials/Blue") as Material;
                    break;
            }
    }

    public ushort ID { get; private set; }
    public Texture2D Picture { get; private set; }

    private GameObject Panel;
    private TextMesh PanelStatus;
    private HealthBehavior HealthBar;

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

    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            if (value <= 0) { _health = 0; State = EntityState.Dead; }
            else { _health = value; if (_health > MaxHealth) _health = MaxHealth; State = EntityState.Alive; }
            if (HealthBar) HealthBar.SetHealth(_health, MaxHealth);
        }
    }

    private EntityState _state;
    public new EntityState State { get { return _state; } set { _state = value; if(PanelStatus) PanelStatus.text = _state.ToString(); } }

    public SpellEffect ActiveEffect { get; set; }

    public ulong Address { get; set; }
    public long Heartbeat { get; set; }
    public bool Connected { get; set; }

    public Device Device { get; set; }
}

public class Team : Entity
{

}
