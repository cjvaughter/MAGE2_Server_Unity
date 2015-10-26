//using System.Drawing;

public enum DeviceType : byte
{
    Generic,
    Fire,
    Water,
    Earth,
    Air,
    Ice,
    Rock,
    Electric,
    Poison,
    Psychic,
    Ghost,
    Shadow,
    Light,
}

public class Device
{
    public Device() { }
    public Device(string name, ushort id, /*Image picture,*/ DeviceType type, string description, bool destructible)
    {
        Name = name;
        ID = id;
        //Picture = picture;
        Type = type;
        Description = description;
        Destructible = destructible;
    }

    public string Name { get; set; }
    public ushort ID { get; set; }
    //public Image Picture { get; set; }
    public DeviceType Type { get; set; }
    public string Description { get; set; }
    public bool Destructible { get; set; }
}  

