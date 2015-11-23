using UnityEngine;
using System.Collections;

public class DatabaseController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;

    void OnLevelWasLoaded()
    {
        FaderIn.SetActive(true);
    }

    public void Return()
    {
        FaderOut.GetComponent<FadeOut>().Scene = "Config";
        FaderOut.SetActive(true);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
