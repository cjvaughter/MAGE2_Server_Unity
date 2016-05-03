using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
//using mageload;

public class ConfigController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;
    public GameObject FaderFromSplash;
    public UpDown RoundLength;
    public UpDown Rounds;
    public UpDown Players;
    public Dropdown GameTypeDropdown;
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
    public GameObject NoPIUDialog;
    public GameObject DFUDialog;
    public GameObject DFUCompleteDialog;
    public Text DFUCompleteText;
    public Slider DFUWriteProgress;
    public Slider DFUVerifyProgress;

    private int _oldRounds, _oldRoundLength, _oldPlayers;
    private string _port;
    private Version currentVersion = typeof(Game).Assembly.GetName().Version;
    private Version _piuVersion = new Version(0, 0);
    private Version _newServerVersion = new Version(0, 0, 0, 0);
    private string _serverDownloadURL = "";
    private Version _newPIUVersion = new Version(0, 0);
    private string _piuDownloadURL = "";
    private bool _checkforupdates = false;
    private Thread dfuThread;

    void Awake()
    {
        Application.runInBackground = true;
        FaderFromSplash.SetActive(true);
        _checkforupdates = true;
    }

    void OnLevelWasLoaded()
    {
        FaderFromSplash.SetActive(false);
        FaderIn.SetActive(true);
    }

    void Start()
    {
        LoadSettings();
        setPIUVersion();
        if (_checkforupdates) StartCoroutine("CheckForUpdates");
	}

    void setPIUVersion()
    {
        Directory.CreateDirectory(@".\PIU Firmware");
        Info.text = Info.text.Replace("Build 0.0.0.0", "Build " + typeof(Game).Assembly.GetName().Version);
        Version highestVersion = new Version(0, 0);
        foreach (string s in Directory.GetFiles(@".\PIU Firmware", "PIU-*.hex"))
        {
            Version v = new Version(s.Replace(@".\PIU Firmware\PIU-", "").Replace(".hex", ""));
            if (v > highestVersion)
                highestVersion = v;
        }
        _piuVersion = highestVersion;
        Info.text = Info.text.Replace("PIU Firmware 0.0", "PIU Firmware " + _piuVersion);
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
        GameTypeDropdown.options.Add(new Dropdown.OptionData(s.Expand()));
        GameTypeDropdown.value = -1;
        GameTypeDropdown.value = (int)(GameType)Enum.Parse(typeof(GameType), Settings.GameType.Replace(" ", ""), true);

        //GameTypeDropdown.options.Add(new Dropdown.OptionData("Test Mode"));
        //GameTypeDropdown.value = -1;
        //GameTypeDropdown.value = (int)(GameType)Enum.Parse(typeof(GameType), "TestMode", true);
        OnGameTypeChanged();

        //Round Length
        RoundLength.Value = Settings.RoundLength;

        //Rounds
        Rounds.Value = Settings.Rounds;

        //Players
        Players.Value = Settings.PlayerCount;
    }

    bool ValidateSettings()
    {
        _port = Coordinator.GetCoordinatorPort();
        if(_port == null)
        {
            Dimmer.SetActive(true);
            NoCoordinatorDialog.SetActive(true);
            return false;
        }

        Settings.GameType = ((GameType)GameTypeDropdown.value).ToString().Expand();
        Settings.RoundLength = RoundLength.Value;
        Settings.Rounds = Rounds.Value;
        Settings.Port = _port;
        Settings.PlayerCount = Players.Value;
        Settings.Save();
        return true;
    }


    public void OnGameTypeChanged()
    {
        if(GameTypeDropdown.value == (int)global::GameType.TestMode || GameTypeDropdown.value == (int)global::GameType.Legacy)
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
            RoundLength.Value = _oldRoundLength == 0 ? RoundLength.DefaultValue : _oldRoundLength;
            Rounds.Value = _oldRounds == 0 ? Rounds.DefaultValue : _oldRounds;
            Players.Value = _oldPlayers == 0 ? Players.DefaultValue : _oldPlayers;

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
        _port = Coordinator.GetPIUPort();
        Dimmer.SetActive(true);
        if (_port == null)
        {
            NoPIUDialog.SetActive(true);
        }
        else
        {
            DFUDialog.SetActive(true);
            dfuThread = new Thread(runMageload) { IsBackground = true };
            dfuThread.Start();
            StartCoroutine("mageloadProgress");
        }
    }

    IEnumerator mageloadProgress()
    {
        while(dfuThread.IsAlive)
        {
            DFUWriteProgress.value = mageload.Mageload.writeProgress;
            DFUVerifyProgress.value = mageload.Mageload.verifyProgress;
            yield return new WaitForSeconds(0.5f);
        }
        DFUDialog.SetActive(false);
        DFUCompleteText.text = mageload.Mageload.message.Replace("\n", "");
        UnityEngine.Debug.Log(mageload.Mageload.message);
        DFUCompleteDialog.SetActive(true);
    }

    public void runMageload()
    {
        string[] args = new string[5];
        args[0] = "-p";
        args[1] = Coordinator.GetPIUPort();
        UnityEngine.Debug.Log(Coordinator.GetPIUPort());
        args[2] = "-f";
        args[3] = @".\PIU Firmware\PIU-" + _piuVersion + ".hex";
        args[4] = "-v";
        mageload.Mageload.Main(args);
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
                if (v > _newServerVersion) _newServerVersion = v;
            }
            if (_newServerVersion > currentVersion)
            {
                serverUpdate = true;
                pattern = new Regex(Constants.URLPattern + _newServerVersion + Constants.EndPattern);
                _serverDownloadURL = pattern.Match(www.text).Value;
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
                if (v > _newPIUVersion) _newPIUVersion = v;
            }
            if (_newPIUVersion > _piuVersion)
            {
                piuUpdate = true;
                pattern = new Regex(Constants.URLPattern + _newPIUVersion + Constants.EndPattern);
                _piuDownloadURL = pattern.Match(www.text).Value;
            }
        }

        if(serverUpdate)
        {
            Dimmer.SetActive(true);
            ServerUpdateText.text = ServerUpdateText.text.Replace("0.0.0.0", _newServerVersion.ToString());
            ServerUpdateDialog.SetActive(true);
        }
        else if(piuUpdate)
        {
            Dimmer.SetActive(true);
            PIUUpdateText.text = PIUUpdateText.text.Replace("0.0", _newPIUVersion.ToString());
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
        WWW www = new WWW(_serverDownloadURL);
        while(true)
        {
            DownloadProgress.value = www.progress;
            if (www.progress == 1.0f) break;
            yield return new WaitForSeconds(0.5f);
        }
        File.WriteAllBytes(@".\Temp\MAGE 2 Setup-" + _newServerVersion + ".exe", www.bytes);

        Process.Start(@".\Temp\MAGE 2 Setup-" + _newServerVersion + ".exe");
        Exit();
        yield return null;
    }

    IEnumerator DownloadPIU()
    {
        WWW www = new WWW(_piuDownloadURL);
        while (true)
        {
            DownloadProgress.value = www.progress;
            if (www.isDone) break;
            yield return new WaitForSeconds(0.5f);
        }
        File.WriteAllBytes(@".\PIU Firmware\PIU-" + _newPIUVersion + ".hex", www.bytes);
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

    public void NoPIUOK()
    {
        NoPIUDialog.SetActive(false);
        Dimmer.SetActive(false);
    }

    public void DFUCompleteOK()
    {
        DFUCompleteDialog.SetActive(false);
        Dimmer.SetActive(false);
    }
}
