using UnityEngine;

public static class Settings
{
    public static string Port = PlayerPrefs.GetString("Port", "COM1");
    public static int BaudRate = PlayerPrefs.GetInt("BaudRate", 38400);
    public static string GameType = PlayerPrefs.GetString("GameType", "Test Mode");
    public static int Rounds = PlayerPrefs.GetInt("Rounds", 3);
    public static int RoundLength = PlayerPrefs.GetInt("RoundLength", 10);
    public static int PlayerCount = PlayerPrefs.GetInt("PlayerCount", 1);

    public static void Save()
    {
        PlayerPrefs.SetString("Port", Port);
        PlayerPrefs.SetInt("BaudRate", BaudRate);
        PlayerPrefs.SetString("GameType", GameType);
        PlayerPrefs.SetInt("Rounds", Rounds);
        PlayerPrefs.SetInt("RoundLength", RoundLength);
        PlayerPrefs.SetInt("PlayerCount", PlayerCount);
    }
}