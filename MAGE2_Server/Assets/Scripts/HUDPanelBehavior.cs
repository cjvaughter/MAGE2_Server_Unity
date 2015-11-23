using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanelBehavior : MonoBehaviour
{
    public Text GameType, Time, Round;

    private static string _time = "";
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
        Time.text = _time;
        Round.text = "ROUND " + _round + "/" + _rounds;
    }

    public static void UpdatePanel(int time, int round)
    {
        _round = round;
        DateTime dt = (time > 0) ? new DateTime().AddMilliseconds(time) : new DateTime();;
        _time = dt.ToString("mm:ss:ff");
    }

}
