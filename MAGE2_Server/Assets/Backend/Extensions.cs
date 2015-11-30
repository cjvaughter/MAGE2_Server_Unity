using System;
using System.Linq;
using System.Collections.Generic;

public static class Extensions
{
    public static string Expand(this string input)
    {
        return new string(input.ToCharArray().SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new char[] { ' ', c } : new char[] { c }).ToArray());
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
}
