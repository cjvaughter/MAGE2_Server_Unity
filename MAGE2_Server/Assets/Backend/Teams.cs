using System.Collections.Generic;
using System.Linq;

public static class Teams
{
    public static List<Team> TeamList = new List<Team>();

    public static void Add(Colors c)
    {
        if (TeamList.Find(t => t.Team == c) == null)
        {
            TeamList.Add(new Team(c));
        }
    }

    public static Team Get(Colors c)
    {
        return TeamList.Find(t => t.Team == c);
    }

    public static List<Player> GetPlayers(Colors c)
    {
        return Players.PlayerList.FindAll(p => p.Team == c);
    }

    public static void Remove(Team t) { TeamList.Remove(t); }
    public static void Clear() { TeamList.Clear(); }
    public static int Count { get { return TeamList.Count; } }
    public static int Remaining { get { return TeamList.Count(t => t.State == EntityState.Alive); } }
    public static Team Survivor { get { return Remaining == 1 ? TeamList.FirstOrDefault(t => t.State == EntityState.Alive) : null; } }
}
