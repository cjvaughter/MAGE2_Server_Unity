using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;

public class ConfigController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;
    public GameObject FaderFromSplash;
    public UpDown RoundLength;
    public UpDown Rounds;
    public UpDown Players;
    public Dropdown gameType;
    public GameObject Dimmer;
    public GameObject InfoDialog;
    public Text Info;
    public GameObject ServerUpdateDialog;
    public Text ServerUpdateText;
    public GameObject PIUUpdateDialog;
    public Text PIUUpdateText;
    public GameObject DownloadDialog;
    public Slider DownloadProgress;
    public GameObject DownloadCompleteDialog;
    public GameObject NoCoordinatorDialog;

    private int _oldRounds, _oldRoundLength, _oldPlayers;
    private string port;
    private Version currentVersion = typeof(Game).Assembly.GetName().Version;
    private Version PIUVersion = new Version(0, 0);
    private Version newServerVersion = new Version(0, 0, 0, 0);
    private string server_download_url = "";
    private Version newPIUVersion = new Version(0, 0);
    private string piu_download_url = "";
    private bool checkforupdates = false;

    void Awake()
    {
        Application.runInBackground = true;
        FaderFromSplash.SetActive(true);
        checkforupdates = true;
    }

    void OnLevelWasLoaded()
    {
        FaderFromSplash.SetActive(false);
        FaderIn.SetActive(true);
    }

    void Start()
    {
        LoadSettings();
        Info.text = Info.text.Replace("Build 0.0.0.0", "Build " + typeof(Game).Assembly.GetName().Version.ToString());
        Version highestVersion = new Version(0, 0);
        foreach(string s in Directory.GetFiles(@".\PIU Firmware", "PIU-*.hex"))
        {
            Version v = new Version(s.Replace(@".\PIU Firmware\PIU-", "").Replace(".hex", ""));
            if (v > highestVersion)
                highestVersion = v;
        }
        PIUVersion = highestVersion;
        Info.text = Info.text.Replace("PIU Firmware 0.0", "PIU Firmware " + PIUVersion.ToString());

        if (checkforupdates) StartCoroutine("CheckForUpdates");
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

        //Players
        Players.Value = Settings.PlayerCount;
    }

    bool ValidateSettings()
    {
        port = Coordinator.GetPort();
        if(port == null)
        {
            Dimmer.SetActive(true);
            NoCoordinatorDialog.SetActive(true);
            return false;
        }

        Settings.GameType = ((GameType)gameType.value).ToString().Expand();
        Settings.RoundLength = RoundLength.Value;
        Settings.Rounds = Rounds.Value;
        Settings.Port = port;
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

    public void Database()
    {
        FaderOut.GetComponent<FadeOut>().Scene = "Database";
        FaderOut.SetActive(true);
    }

    public void DFU()
    {
        FaderOut.GetComponent<FadeOut>().Scene = "DFU";
        FaderOut.SetActive(true);
    }

    public void ShowInfo()
    {
        Dimmer.SetActive(true);
        InfoDialog.SetActive(true);
    }

    public void HideInfo()
    {
        InfoDialog.SetActive(false);
        Dimmer.SetActive(false);
    }

    IEnumerator CheckForUpdates()
    {
        bool serverUpdate = false, piuUpdate = false;

        WWW www = new WWW(Constants.ServerUpdateURL);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            Regex pattern = new Regex(Constants.NamePattern + Constants.EndPattern);
            foreach (Match m in pattern.Matches(www.text))
            {
                Version v = new Version(m.Captures[0].Value.Replace("MAGE 2 Setup-", "").Replace(".exe", ""));
                if (v > newServerVersion) newServerVersion = v;
            }
            if (newServerVersion > currentVersion)
            {
                serverUpdate = true;
                pattern = new Regex(Constants.URLPattern + newServerVersion.ToString() + Constants.EndPattern);
                server_download_url = pattern.Match(www.text).Value;
            }
        }

        www = new WWW(Constants.PIUUpdateURL);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            Regex pattern = new Regex(Constants.NamePattern + Constants.EndPattern);
            foreach (Match m in pattern.Matches(www.text))
            {
                Version v = new Version(m.Captures[0].Value.Replace("PIU-", "").Replace(".hex", ""));
                if (v > newPIUVersion) newPIUVersion = v;
            }
            if (newPIUVersion > PIUVersion)
            {
                piuUpdate = true;
                pattern = new Regex(Constants.URLPattern + newPIUVersion.ToString() + Constants.EndPattern);
                piu_download_url = pattern.Match(www.text).Value;
            }
        }

        if(serverUpdate)
        {
            Dimmer.SetActive(true);
            ServerUpdateText.text = ServerUpdateText.text.Replace("0.0.0.0", newServerVersion.ToString());
            ServerUpdateDialog.SetActive(true);
        }
        else if(piuUpdate)
        {
            Dimmer.SetActive(true);
            PIUUpdateText.text = PIUUpdateText.text.Replace("0.0", newPIUVersion.ToString());
            PIUUpdateDialog.SetActive(true);
        }
        yield return null;
    }

    public void YesDownloadServer()
    {
        StartCoroutine("DownloadServer");
        DownloadDialog.SetActive(true);
        ServerUpdateDialog.SetActive(false);
    }

    public void NoDownloadServer()
    {
        ServerUpdateDialog.SetActive(false);
        Dimmer.SetActive(false);
    }

    public void YesDownloadPIU()
    {
        StartCoroutine("DownloadPIU");
        DownloadDialog.SetActive(true);
        PIUUpdateDialog.SetActive(false);
    }

    public void NoDownloadPIU()
    {
        PIUUpdateDialog.SetActive(false);
        Dimmer.SetActive(false);
    }

    IEnumerator DownloadServer()
    {
        WWW www = new WWW(server_download_url);
        while(true)
        {
            DownloadProgress.value = www.progress;
            if (www.progress == 1.0f) break;
            yield return new WaitForSeconds(0.5f);
        }
        File.WriteAllBytes(@".\Temp\MAGE 2 Setup-" + newServerVersion.ToString() + ".exe", www.bytes);

        Process.Start(@".\Temp\MAGE 2 Setup-" + newServerVersion.ToString() + ".exe");
        Exit();
        yield return null;
    }

    IEnumerator DownloadPIU()
    {
        WWW www = new WWW(piu_download_url);
        while (true)
        {
            DownloadProgress.value = www.progress;
            if (www.isDone) break;
            yield return new WaitForSeconds(0.5f);
        }
        File.WriteAllBytes(@".\PIU Firmware\PIU-" + newPIUVersion.ToString() + ".hex", www.bytes);
        DownloadCompleteDialog.SetActive(true);
        DownloadDialog.SetActive(false);
        yield return null;
    }

    public void DownloadCompleteOK()
    {
        DownloadCompleteDialog.SetActive(false);
        Dimmer.SetActive(false);
    }

    public void NoCoordinatorOK()
    {
        NoCoordinatorDialog.SetActive(false);
        Dimmer.SetActive(false);
    }
}
