
public enum GameType : byte
{
    FreeForAll,
    TeamBattle,
    TestMode,
}

public interface IGameRules
{
    bool CanStart { get; }
    bool GameIsOver { get; }
    bool ConnectAnytime { get; }
    bool PlayersCanBeSaved { get; }
    bool OneHitKill { get; }
    Entity WhoWon { get; }
}

public class FreeForAll : IGameRules
{
    public bool CanStart {
        get { return (Game.PlayerCount == Players.Count); }
    }
    public bool GameIsOver {
        get { return (Players.Remaining == 1); }
    }
    public bool ConnectAnytime
    {
        get { return false; }
    }
    public bool PlayersCanBeSaved {
        get { return false; }
    }
    public bool OneHitKill {
        get { return false; }
    }
    public Entity WhoWon {
        get { return Players.Survivor; }
    }
}

public class TeamBattle : IGameRules
{
    public bool CanStart {
        get { return (Game.PlayerCount == Players.Count && Teams.Count > 1); }
    }
    public bool GameIsOver {
        get { return (Teams.Remaining == 1); }
    }
    public bool ConnectAnytime
    {
        get { return false; }
    }
    public bool PlayersCanBeSaved {
        get { return false; }
    }
    public bool OneHitKill {
        get { return false; }
    }
    public Entity WhoWon {
        get { return Teams.Survivor; }
    }
}

public class TestMode : IGameRules
{
    public bool CanStart
    {
        get { return true; }
    }
    public bool GameIsOver
    {
        get { return false; }
    }
    public bool ConnectAnytime
    {
        get { return true; }
    }
    public bool PlayersCanBeSaved
    {
        get { return true; }
    }
    public bool OneHitKill
    {
        get { return false; }
    }
    public Entity WhoWon
    {
        get { return null; }
    }
}
