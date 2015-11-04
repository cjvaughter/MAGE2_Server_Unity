using System;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Collections;

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

    private bool _paused;

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
        }
        catch (Exception e)
        {
            Logger.Log(e.Message);
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
    }

    void FixedUpdate()
    {
        try
        {
            Game.Run();
        }
        catch(Exception e)
        {
            Logger.Log(e.Message + "\r\n" + e.StackTrace + "\r\n");
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

    public void Damage()
    {
        Player me = Players.Get(0x13A20040A994A1);
        me.Health -= 5;
        Coordinator.UpdatePlayer(me);
    }

    public void Stun()
    {
        Player me = Players.Get(0x13A20040A994A1);
        me.State = EntityState.Stunned;
        me.ActiveEffect = new SpellEffect(3000);
        Coordinator.UpdatePlayer(me);
    }

    public void Heal()
    {
        Player me = Players.Get(0x13A20040A994A1);
        me.Health += 5;
        Coordinator.UpdatePlayer(me);
    }
}
