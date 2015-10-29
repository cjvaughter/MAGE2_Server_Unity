using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public ScrollRect logScroll;
    public Text logPanel;
    public GameObject overlay;
    public GameObject PauseDimmer;
    public GameObject PlayAgainDimmer;
    public GameObject ExitDimmer;

    private bool _paused;

    void Start()
    {
        Logger.LogScroll = logScroll;
        Logger.LogPanel = logPanel;
#if !UNITY_EDITOR
        Console.SetOut(new ConsoleWriter(logPanel));
#endif
        Game.Announcer = overlay.GetComponent<AnnouncerBehavior>();

        Game.Start();
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
        Game.Run();
    }

    public void PauseGame()
    {
        if (!_paused)
        {
            Game.Pause();
            _paused = true;
            PauseDimmer.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void UnpauseGame()
    {
        if (_paused)
        {
            Game.Unpause();
            _paused = false;
            PauseDimmer.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ShowExit()
    {
        ExitDimmer.SetActive(true);
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
        Application.LoadLevel(0);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
