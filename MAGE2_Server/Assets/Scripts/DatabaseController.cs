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
    public UnityEngine.UI.Button NewButton, EditButton, DeleteButton, SaveButton, CancelButton, ReturnButton;
    public GameObject listPanel;
    public ListViewBehavior listView;
    public GameObject Dimmer;
    public GameObject AreYouSure;
    public Sprite DefaultPlayer;

    private Color unselectedColor;
    private Color textColor = new Color(50f/255f,50f/255f,50f/255f);
    private bool _editing;
    private string _originalName, _originalID;
    private int _originalTeam;
    private Sprite _originalPicture;

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
    void Start ()
    {
        unselectedColor = Team.GetComponentInChildren<Text>().color;
        OnPlayerSelected(null);
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
            Filter = "PNG, JPG|*.png;*.jpg",
            Title = "Choose an image",
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
        if (listItem == null || listItem == listPanel)
        {
            Name.text = "";
            ID.text = "";
            Team.value = -1;
            Text listbox = Team.GetComponentInChildren<Text>();
            listbox.text = "Team";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = unselectedColor;
            PictureBox.sprite = null;
            PictureBox.color = new Color(1, 1, 1, 0.5f);

            NewButton.interactable = true;
            EditButton.interactable = false;
            DeleteButton.interactable = false;
            return;
        }

        Text name = listItem.transform.FindChild("Name").GetComponent<Text>();
        Text id = listItem.transform.FindChild("ID").GetComponent<Text>();
        Text team = listItem.transform.FindChild("Team").GetComponent<Text>();
        Sprite picture = listItem.transform.FindChild("Picture").GetComponent<Image>().sprite;

        Name.text = name.text;
        ID.text = id.text;
        switch (team.text)
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

        PictureBox.sprite = picture;
        PictureBox.color = Color.white;

        NewButton.interactable = true;
        EditButton.interactable = true;
        DeleteButton.interactable = true;
    }

    public void OnTeamChanged()
    {
        Text listbox = Team.GetComponentInChildren<Text>();
        if (Team.value == -1)
        {
            listbox.text = "Team";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = unselectedColor;
        }
        else
        {
            listbox.fontStyle = FontStyle.Normal;
            listbox.color = textColor;
        }
    }

    public void NewPlayer()
    {
        _editing = false;
        OnPlayerSelected(null);
        NewButton.interactable = false;
        EditButton.interactable = false;
        DeleteButton.interactable = false;
        SaveButton.interactable = true;
        CancelButton.interactable = true;
        ReturnButton.interactable = false;

        PictureBox.GetComponent<UnityEngine.UI.Button>().interactable = true;
        PictureBox.sprite = DefaultPlayer;
        PictureBox.color = Color.white;
        Name.interactable = true;
        ID.interactable = true;
        Team.interactable = true;
        listView.Disable();
    }

    public void EditPlayer()
    {
        _editing = true;
        _originalName = Name.text;
        _originalID = ID.text;
        _originalTeam = Team.value;
        _originalPicture = PictureBox.sprite;
        NewButton.interactable = false;
        EditButton.interactable = false;
        DeleteButton.interactable = false;
        SaveButton.interactable = true;
        CancelButton.interactable = true;
        ReturnButton.interactable = false;

        PictureBox.GetComponent<UnityEngine.UI.Button>().interactable = true;
        Name.interactable = true;
        ID.interactable = true;
        Team.interactable = true;
        listView.Disable();
    }

    public void DeletePlayer()
    {
        Dimmer.SetActive(true);
        AreYouSure.SetActive(true);
    }

    public void YesDeletePlayer()
    {
        Database.DeletePlayer(ID.text);
        OnPlayerSelected(null);
        listView.Empty();
        listView.Fill();
        Dimmer.SetActive(false);
        AreYouSure.SetActive(false);
    }

    public void NoDeletePlayer()
    {
        Dimmer.SetActive(false);
        AreYouSure.SetActive(false);
    }

    public void SavePlayer()
    {
        //validate
        if(_editing)
            Database.UpdatePlayer(_originalID, Name.text, ID.text, PictureBox.sprite.texture, Team.captionText.text);
        else
            Database.InsertPlayer(Name.text, ID.text, PictureBox.sprite.texture, Team.captionText.text);

        listView.Empty();
        listView.Fill();
        Name.interactable = false;
        ID.interactable = false;
        Team.interactable = false;
        NewButton.interactable = true;
        EditButton.interactable = true;
        DeleteButton.interactable = true;
        SaveButton.interactable = false;
        CancelButton.interactable = false;
        ReturnButton.interactable = true;
        listView.Enable();
    }

    public void CancelPlayer()
    {
        PictureBox.GetComponent<UnityEngine.UI.Button>().interactable = false;
        Name.interactable = false;
        ID.interactable = false;
        Team.interactable = false;
        NewButton.interactable = true;
        SaveButton.interactable = false;
        CancelButton.interactable = false;
        ReturnButton.interactable = true;

        if (_editing)
        {
            PictureBox.sprite = _originalPicture;
            Name.text = _originalName;
            ID.text = _originalID;
            Team.value = _originalTeam;
            EditButton.interactable = true;
            DeleteButton.interactable = true;
        }
        else
        {
            OnPlayerSelected(null);
        }

        listView.Enable();
    }
}
