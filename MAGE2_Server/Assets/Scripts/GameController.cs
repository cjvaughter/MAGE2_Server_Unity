using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Collections;
using System.Diagnostics;

public class GameController : MonoBehaviour
{
    public ScrollRect logScroll;
    public Text logPanel;
    public GameObject overlay;
    public GameObject PauseDimmer;
    public GameObject PlayAgainDimmer;
    public GameObject ExitDimmer;
    public GameObject FaderIn;
    public GameObject FaderOut;
    public InfoPanelBehavior infoPanel;
    public GameObject controlPanel;

    private PanelBehavior SelectedPlayer;
    private Vector3 _lastCameraPos;

    private bool _paused;

    public void SelectPlayer(PanelBehavior player)
    {
        if (SelectedPlayer == null)
        {
            SelectedPlayer = player;
            _lastCameraPos = Camera.main.transform.position;
            Camera.main.GetComponent<CameraMovement>().SetPosition(player.gameObject.transform.position, false);
            Camera.main.GetComponent<CameraMovement>().SetZoom(100);
            infoPanel.SetPlayer(Players.Get(SelectedPlayer.ID));
        }
        else if (SelectedPlayer == player)
        {
            SelectedPlayer = null;
            Camera.main.GetComponent<CameraMovement>().SetPosition(_lastCameraPos);
            Camera.main.GetComponent<CameraMovement>().SetZoom(0);
            infoPanel.SetPlayer(null);
        }
        else
        {
            SelectedPlayer = player;
            Camera.main.GetComponent<CameraMovement>().SetPosition(player.gameObject.transform.position, false);
            infoPanel.SetPlayer(Players.Get(SelectedPlayer.ID));
        }
    }

    void Awake()
    {
        FaderIn.SetActive(true);
    }

    void Start()
    {
        Logger.LogScroll = logScroll;
        Logger.LogPanel = logPanel;
        Logger.Initialize();

        try
        {
        #if !UNITY_EDITOR
            Console.SetOut(new ConsoleWriter(logPanel));
        #endif
            Game.Announcer = overlay.GetComponent<AnnouncerBehavior>();
            Game.Start();
            if (Game.Type == GameType.TestMode)
            {
                controlPanel.SetActive(true);
                infoPanel.gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Logger.Log(e.Message + "\r\n" + e.StackTrace + "\r\n");
        }
    }

    void OnApplicationQuit()
    {
        Game.Stop();
    }

    void Update()
    {
        int time = (Game.State == GameState.Active) ? Game.TimeRemaining : 0;
        HUDPanelBehavior.UpdatePanel(time, Game.Round);
        infoPanel.UpdateInfo();
    }

    void FixedUpdate()
    {
        try
        {
            Game.Run();
        }
        catch(Exception e)
        {
            StackTrace trace = new StackTrace(e, true);
            Logger.Log(e.Message + " in \"" + trace.GetFrame(0).GetFileName() + "\" at " + trace.GetFrame(0).GetFileLineNumber() + "\r\n");
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
        Application.LoadLevel("GameScreen");
    }

    void Exit()
    {
        StartCoroutine("Stop"); //Game.Stop can hang for a second, this eliminates the user seeing that
        FaderOut.SetActive(true);
    }

    IEnumerator Stop()
    {
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
        if(SelectedPlayer)
            Spell.DetermineSuccess(Players.Get(SelectedPlayer.ID), new IRPacket() { ID = SelectedPlayer.ID, Spell = spell });
    }
}
