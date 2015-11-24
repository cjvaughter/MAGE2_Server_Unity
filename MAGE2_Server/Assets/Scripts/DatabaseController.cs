using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Windows.Forms;

public class DatabaseController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;
    public Image PictureBox;
    public InputField Name, ID;
    public Dropdown Team;

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

    public void SelectPicture()
    {
        Texture2D picture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        OpenFileDialog open = new OpenFileDialog
        {
            InitialDirectory = @"%HOMEPATH%\Pictures\",
            Filter = "PNG, JPG|*.png;*.jpg"
        };
        if (open.ShowDialog() == DialogResult.OK)
        {
            if(picture.LoadImage(File.ReadAllBytes(open.FileName)))
            {
                PictureBox.sprite = Sprite.Create(picture, new Rect(0,0,picture.width,picture.height), new Vector2(0.5f, 0.5f));
                PictureBox.color = Color.white;
            }
        }
    }

    public void OnPlayerSelected(GameObject listItem)
    {
        if(listItem == null)
        {
            Name.text = "";
            ID.text = "";
            Team.value = -1;
            return;
        }

        Text name = listItem.transform.FindChild("Name").GetComponent<Text>();
        Text id = listItem.transform.FindChild("ID").GetComponent<Text>();
        Text team = listItem.transform.FindChild("Team").GetComponent<Text>();

        Name.text = name.text;
        ID.text = id.text;
        
        switch(team.text)
        {
            case "Red":
                Team.value = 0;
                break;
            case "Yellow":
                Team.value = 1;
                break;
            case "Green":
                Team.value = 2;
                break;
            case "Blue":
                Team.value = 3;
                break;
        }
    }
}
