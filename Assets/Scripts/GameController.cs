using System;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public ScrollRect logScroll;
    public Text logPanel;
    public GameObject overlay;
    public GameObject Dimmer;
    public Text PauseButton;
    private bool _paused;

    void Start()
    {
        Logger.LogScroll = logScroll;
        Logger.LogPanel = logPanel;
        Game.Announcer = overlay.GetComponent<OverlayBehavior>();

        Game.Start();
    }

    void OnApplicationQuit()
    {
        Game.Stop();
    }

    void FixedUpdate()
    {
        Game.Run();
    }

    public void PauseGame()
    {
        if (_paused)
        {
            Game.Unpause();
            _paused = false;
            Dimmer.SetActive(false);
            PauseButton.text = "Pause";
            Time.timeScale = 1;
        }
        else
        {
            Game.Pause();
            _paused = true;
            Dimmer.SetActive(true);
            PauseButton.text = "Resume";
            Time.timeScale = 0;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
