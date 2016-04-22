using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public CameraMovement MainCamera;
    public ScrollRect LogScroll;
    public Text LogPanel;
    public GameObject Overlay;
    public GameObject PauseDimmer;
    public GameObject PlayAgainDimmer;
    public GameObject ExitDimmer;
    public GameObject FaderIn;
    public GameObject FaderOut;
    public InfoPanelBehavior InfoPanel;
    public GameObject ControlPanel;
    public Button PauseButton;

    public bool PlayerSelected = false;
    private PanelBehavior _selectedPlayer;
    private Vector3 _lastCameraPos;
    private int _playerCount = 0;
    private int _teamCount = 0;
    private int _rowCount = 0;
    private bool _paused;
    public bool Legacy;

    public void SelectPlayer(PanelBehavior player)
    {
        if (_selectedPlayer == null)
        {
            _selectedPlayer = player;
            _lastCameraPos = Camera.main.GetComponent<CameraMovement>().pos;
            Camera.main.GetComponent<CameraMovement>().SetPosition(player.gameObject.transform.position, false);
            Camera.main.GetComponent<CameraMovement>().SetZoom(100);
            InfoPanel.SetPlayer(player.Player);
            PlayerSelected = true;
        }
        else if (_selectedPlayer == player)
        {
            _selectedPlayer = null;
            Camera.main.GetComponent<CameraMovement>().SetPosition(_lastCameraPos);
            Camera.main.GetComponent<CameraMovement>().SetZoom(0);
            InfoPanel.SetPlayer(null);
            PlayerSelected = false;
        }
        else
        {
            _selectedPlayer = player;
            Camera.main.GetComponent<CameraMovement>().SetPosition(player.gameObject.transform.position, false);
            InfoPanel.SetPlayer(player.Player);
            PlayerSelected = true;
        }
    }

    public void AddPlayerPanel(PanelBehavior player)
    {
        float xPos = 0.0f;
        float yPos = 0.0f;

        if(Legacy)
        {
            xPos = -10.5f + 7 * (_playerCount % 4);
            yPos = 3.5f - 3.5f * (_playerCount / 4);
            _rowCount = (_playerCount / 4);
        }
        else if (Game.Rules.TeamBased)
        {
            int num = Teams.GetPlayers(player.Player.Team).Count;
            if (num > _rowCount) _rowCount = num;

            yPos = 3.5f - 3.5f * (num - 1);
            switch(player.Player.Team)
            {
                case Colors.Red:
                    xPos = -10.5f;
                    break;
                case Colors.Yellow:
                    xPos = -3.5f;
                    break;
                case Colors.Green:
                    xPos = 3.5f;
                    break;
                case Colors.Blue:
                    xPos = 10.5f;
                    break;
            }
        }
        else
        {
            xPos = -10.5f + 7 * (_playerCount % 4);
            yPos = 3.5f - 3.5f * (_playerCount / 4);
            _rowCount = (_playerCount / 4);
        }
        MainCamera.SetRows(_rowCount);
        player.gameObject.transform.position = new Vector3(xPos, yPos, 0);
        _playerCount++;
    }

    void Awake()
    {
        FaderIn.SetActive(true);
    }

    void Start()
    {
        if (Settings.GameType == "Legacy")
        {
            global::Legacy.Logger.LogScroll = LogScroll;
            global::Legacy.Logger.LogPanel = LogPanel;
            global::Legacy.Logger.Initialize();
        }
        else
        {
            Logger.LogScroll = LogScroll;
            Logger.LogPanel = LogPanel;
            Logger.Initialize();
        }

        try
        {
            if (Settings.GameType == "Legacy")
            {
                Legacy = true;
                PauseButton.gameObject.SetActive(false);
                global::Legacy.Game.Start();
            }
            else
            {
                Game.Announcer = Overlay.GetComponent<AnnouncerBehavior>();
                Game.Start();
                if (Game.Type == GameType.TestMode)
                {
                    ControlPanel.SetActive(true);
                    InfoPanel.gameObject.SetActive(false);
                }
            }
        }
        catch (Exception e)
        {
            if (Legacy) global::Legacy.Logger.Log(e.Message + "\r\n" + e.StackTrace + "\r\n");
            else Logger.Log(e.Message + "\r\n" + e.StackTrace + "\r\n");
        }
    }

    void OnApplicationQuit()
    {
        if (Legacy) global::Legacy.Game.Stop();
        else Game.Stop();
    }

    void Update()
    {
        int time = (Game.State == GameState.Active) ? Game.TimeRemaining : 0;
        HUDPanelBehavior.UpdatePanel(time, Game.Round);
    }

    void FixedUpdate()
    {
        try
        {
            if (Legacy) global::Legacy.Game.Run();
            else Game.Run();
        }
        catch(Exception e)
        {
            StackTrace trace = new StackTrace(e, true);
            if(Legacy) global::Legacy.Logger.Log(e.Message + " in \"" + trace.GetFrame(0).GetFileName() + "\" at " + trace.GetFrame(0).GetFileLineNumber() + "\r\n");
            else Logger.Log(e.Message + " in \"" + trace.GetFrame(0).GetFileName() + "\" at " + trace.GetFrame(0).GetFileLineNumber() + "\r\n");
        }
    }

    public void PauseGame()
    {
        try
        {
            if (!_paused && Game.State != GameState.Waiting)
            {
                Game.Pause();
                _paused = true;
                PauseDimmer.SetActive(true);
                Time.timeScale = 0;
            }
        }
        catch (Exception e)
        {
            Logger.Log(e.Message);
        }
    }

    public void UnpauseGame()
    {
        try
        { 
            if (_paused)
            {
                Game.Unpause();
                _paused = false;
                PauseDimmer.SetActive(false);
                Time.timeScale = 1;
            }
        }
        catch (Exception e)
        {
            Logger.Log(e.Message);
        }
    }

    public void ShowExit()
    {
        if (Game.State != GameState.Waiting)
            ExitDimmer.SetActive(true);
        else
            Exit();
    }

    public void HideExit()
    {
        ExitDimmer.SetActive(false);
    }

    public void ShowPlayAgain()
    {
        PlayAgainDimmer.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("GameScreen");
    }

    void Exit()
    {
        StartCoroutine("Stop"); //Game.Stop can hang for a second, this eliminates the user seeing that
        FaderOut.SetActive(true);
    }

    IEnumerator Stop()
    {
        if (Legacy) return global::Legacy.Game.Stop();
        return Game.Stop();
    }

    public void Damage() { CastSpell(SpellType.Damage); }
    public void Stun() { CastSpell(SpellType.Stun); }
    public void Heal() { CastSpell(SpellType.Heal); }
    public void Fire() { CastSpell(SpellType.Fire); }
    public void Water() { CastSpell(SpellType.Water); }
    public void Earth() { CastSpell(SpellType.Earth); }
    public void Wind() { CastSpell(SpellType.Wind); }
    public void Ice() { CastSpell(SpellType.Ice); }
    public void Rock() { CastSpell(SpellType.Rock); }
    public void Thunder() { CastSpell(SpellType.Thunder); }
    public void Poison() { CastSpell(SpellType.Poison); }
    public void Psychic() { CastSpell(SpellType.Psychic); }
    public void Ghost() { CastSpell(SpellType.Ghost); }
    public void Shadow() { CastSpell(SpellType.Shadow); }
    public void Light() { CastSpell(SpellType.Light); }

    private void CastSpell(SpellType spell)
    {
        if (_selectedPlayer)
            Spell.DetermineSuccess(_selectedPlayer.Player, new IRPacket() { Spell = spell }, true);
        else
        {
            Spell.DetermineSuccess(new Player() { Team = Colors.Red}, new IRPacket() { Spell = (SpellType)Enum.Parse(typeof(SpellType), spell.ToString() + "Team", true) }, true);
        }
    }
}
