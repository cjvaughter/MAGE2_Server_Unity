using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    int millis = 0;
    public Text text;

    void Start()
    {
        millis = 6000;
        text.text = "00:06:00";
    }

    void Update()
    {
        millis -= (int)(Time.deltaTime * 1000);
        if (millis < 0) millis = 0;
        DateTime time = new DateTime(millis * 10000);
        text.text = time.ToString("mm:ss:ff");
    }
}