public class Damage : Spell
{
    public Damage(Player caster) : base(caster)
    {
        PrimaryEffect = SpellEffect.Damage;
    }
}

public class Fire : Damage
{
    public Fire(Player caster) : base(caster)
    {
        PrimaryValue = 1;
        SecondaryEffect = SpellEffect.Repeat;
        SecondaryValue = 10;
    }
}

public class Water : Damage
{
    public Water(Player caster) : base(caster)
    {
        PrimaryValue = 10;
        SecondaryEffect = SpellEffect.Stun;
        SecondaryValue = 5;
    }
}

public class Thunder : Damage
{
    public Thunder(Player caster) : base(caster)
    {
        PrimaryValue = 25;
        SecondaryEffect = SpellEffect.Stun;
        SecondaryValue = 5;

        TemporaryComplete = false;
        TemporaryLength = 30;
        StrengthModifier = -15;
        DefenseModifier = -15;
        LuckModifier = int.MinValue;
    }
}

public class Poison : Damage
{
    public Poison(Player caster) : base(caster)
    {
        PrimaryValue = 1;
        SecondaryEffect = SpellEffect.Repeat;
        SecondaryValue = 30;

        StrengthModifier = -10;
        DefenseModifier = -10;
        LuckModifier = -10;
    }
}

public class Ghost : Damage
{
    public Ghost(Player caster) : base(caster)
    {
        PrimaryValue = 20;

        TemporaryComplete = false;
        TemporaryLength = 30;
        StrengthModifier = int.MinValue;
    }
}

public class Psychic : Damage
{
    public Psychic(Player caster) : base(caster)
    {
        PrimaryValue = 10;

        TemporaryComplete = false;
        TemporaryLength = 30;
        StrengthModifier = -10;
        LuckModifier = -10;
    }
}