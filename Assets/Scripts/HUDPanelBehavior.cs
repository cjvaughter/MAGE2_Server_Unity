using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanelBehavior : MonoBehaviour
{
    public Text GameType, Time, Round;

    private static DateTime _time = new DateTime();
    private static int _round = 0, _rounds = 0;
    private static string _gametype = "";

    public static void Initialize(string type, int rounds)
    {
        _gametype = type.ToUpper();
        _round = 0;
        _rounds = rounds;  
    }

    void Update()
    {
        GameType.text = _gametype;
        Time.text = _time.ToString("mm:ss:ff");
        Round.text = "ROUND " + _round + "/" + _rounds;
    }

    public static void UpdatePanel(int time, int round)
    {
        _round = round;
        _time = (time > 0) ? new DateTime().AddMilliseconds(time) : new DateTime();
    }
}
