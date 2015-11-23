public class Stun : Spell
{
    public Stun(Player caster) : base(caster)
    {
        PrimaryEffect = SpellEffect.Stun;
    }
}

public class Ice : Stun
{
    public Ice(Player caster) : base(caster)
    {
        PrimaryValue = 5;
        SecondaryEffect = SpellEffect.Damage;
        SecondaryValue = 5;
    }
}

public class Rock : Stun
{
    public Rock(Player caster) : base(caster)
    {
        PrimaryValue = 5;
        SecondaryEffect = SpellEffect.Damage;
        SecondaryValue = 10;

        TemporaryComplete = false;
        TemporaryLength = 30;
        StrengthModifier = -10;
        DefenseModifier = -10;
    }
}

public class Shadow : Stun
{
    public Shadow(Player caster) : base(caster)
    {
        PrimaryValue = 10;
        SecondaryEffect = SpellEffect.Damage;
        SecondaryValue = 25;

        TemporaryComplete = false;
        TemporaryLength = 30;
        StrengthModifier = int.MinValue;
        DefenseModifier = int.MinValue;
        LuckModifier = int.MinValue;

        Overridable = false;
    }
}