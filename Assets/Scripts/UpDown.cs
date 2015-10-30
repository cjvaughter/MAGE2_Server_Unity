using System;
using UnityEngine.UI;

public class UpDown : Selectable
{
    public int Value = 0;
    public int DefaultValue = 0;
    public int Min = 0;
    public int Max = 0;
    public int Step = 1;
    public bool IsTime = false;
    public Text text;

    public void Increment()
    {
        if (interactable)
        {
            Value += Step;
            if (Value > Max) Value = Max;
        }
    }

    public void Decrement()
    {
        if (interactable)
        {
            Value -= Step;
            if (Value < Min) Value = Min;
        }
    }

    void Update()
    {
        if (IsTime)
        {
            DateTime dt = new DateTime();
            dt = dt.AddSeconds(Value);
            text.text = "    " + dt.ToString("mm:ss");
        }
        else
        {
            text.text = "    " + Value.ToString();
        }
    }
}
