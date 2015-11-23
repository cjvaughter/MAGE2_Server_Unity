public class Heal : Spell
{
    public Heal(Player caster) : base(caster)
    {
        PrimaryEffect = SpellEffect.Heal;
    }
}

public class Wind : Heal
{
    public Wind(Player caster) : base(caster)
    {
        PrimaryValue = 10;

        TemporaryComplete = false;
        TemporaryLength = 30;
        DefenseModifier = 10;
    }
}

public class Light : Heal
{
    public Light(Player caster) : base(caster)
    {
        PrimaryValue = 25;

        TemporaryComplete = false;
        TemporaryLength = 15;
        StrengthModifier = 25;
        DefenseModifier = 25;
        LuckModifier = 25;
    }
}