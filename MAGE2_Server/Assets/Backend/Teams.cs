using System.Collections.Generic;
using System.Linq;

public static class Teams
{
    public static List<Team> TeamList = new List<Team>();
    public static void Add(Team t) { TeamList.Add(t); }
    public static void Remove(Team t) { TeamList.Remove(t); }
    public static void Clear() { TeamList.Clear(); }
    public static int Count { get { return TeamList.Count; } }
    public static int Remaining { get { return TeamList.Count(t => t.State == EntityState.Alive); } }
    public static Team Survivor { get { return Remaining == 1 ? TeamList.FirstOrDefault(t => t.State == EntityState.Alive) : null; } }
}
