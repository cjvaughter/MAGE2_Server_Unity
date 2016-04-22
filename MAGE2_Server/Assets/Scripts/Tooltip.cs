using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Text text;

    void Start()
    {
        text.enabled = false;
    }

    void OnMouseEnter()
    {
        text.enabled = true;
    }

    void OnMouseExit()
    {
        text.enabled = false;
    }
}
