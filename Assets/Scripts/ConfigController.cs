using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfigController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;
    public GameObject FaderFromSplash;
    public UpDown RoundLength;
    public UpDown Rounds;
    public UpDown Players;
    public Dropdown gameType;
    public Dropdown port;

    private int _oldRounds, _oldRoundLength, _oldPlayers;

    void Awake()
    {
        FaderFromSplash.SetActive(true);
    }

    void OnLevelWasLoaded()
    {
        FaderFromSplash.SetActive(false);
        FaderIn.SetActive(true);
    }

    void Start ()
    {
        LoadSettings();
	}

    public void StartGame()
    {
        if (ValidateSettings())
            FaderOut.SetActive(true);
    }

    void LoadSettings()
    {
        //Game Type
        foreach (string s in Enum.GetNames(typeof(GameType)))
            gameType.options.Add(new Dropdown.OptionData(s.Expand()));
        gameType.value = -1;
        gameType.value = (int)(GameType)Enum.Parse(typeof(GameType), Settings.GameType.Replace(" ", ""), true);

        //Round Length
        RoundLength.Value = Settings.RoundLength;

        //Rounds
        Rounds.Value = Settings.Rounds;

        //Port
        int count = 0;
        int selection = 0;
        foreach(string s in Coordinator.GetPorts())
        {
            port.options.Add(new Dropdown.OptionData(s));
            Debug.Log(s);
            if (s == Settings.Port) selection = count;
            count++;
        }
        port.value = -1;
        port.value = selection;

        //Players
        Players.Value = Settings.PlayerCount;
    }

    bool ValidateSettings()
    {
        //check values?
        Settings.GameType = ((GameType)gameType.value).ToString().Expand();
        Settings.RoundLength = RoundLength.Value;
        Settings.Rounds = Rounds.Value;
        Settings.Port = port.options[port.value].text;
        Settings.PlayerCount = Players.Value;
        Settings.Save();
        return true;
    }


    public void OnGameTypeChanged()
    {
        if(gameType.value == (int)GameType.TestMode)
        {
            _oldRoundLength = RoundLength.Value;
            _oldRounds = Rounds.Value;
            _oldPlayers = Players.Value;
            RoundLength.Value = 0;
            Rounds.Value = 0;
            Players.Value = 0;
            RoundLength.interactable = false;
            Rounds.interactable = false;
            Players.interactable = false;
        }
        else
        {
            if (_oldRoundLength == 0)
                RoundLength.Value = RoundLength.DefaultValue;
            else
                RoundLength.Value = _oldRoundLength;
            if (_oldRounds == 0)
                Rounds.Value = Rounds.DefaultValue;
            else
                Rounds.Value = _oldRounds;
            if (_oldPlayers == 0)
                Players.Value = Players.DefaultValue;
            else
                Players.Value = _oldPlayers;

            RoundLength.interactable = true;
            Rounds.interactable = true;
            Players.interactable = true;
        }
    }

    public void Exit()
    {
        FaderOut.GetComponent<FadeOut>().Scene = "Exit";
        FaderOut.SetActive(true);
    }
}
