using UnityEngine;

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
    public Device(string name, ushort id, byte[] picture, DeviceType type, string description, bool destructible)
    {
        Name = name;
        ID = id;
        Picture = new Texture2D(1, 1);
        Picture.LoadImage(picture);
        Type = type;
        Description = description;
        Destructible = destructible;
    }

    public string Name { get; set; }
    public ushort ID { get; set; }
    public Texture2D Picture { get; set; }
    public DeviceType Type { get; set; }
    public string Description { get; set; }
    public bool Destructible { get; set; }
}  

