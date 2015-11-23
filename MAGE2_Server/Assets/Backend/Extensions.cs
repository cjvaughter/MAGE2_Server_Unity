using System.Linq;

public static class Extensions
{
    public static string Expand(this string input)
    {
        return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new char[] { ' ', c } : new char[] { c }).ToArray());
    }
}
