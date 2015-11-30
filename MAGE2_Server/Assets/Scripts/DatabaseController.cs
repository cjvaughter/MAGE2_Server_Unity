using System.Globalization;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class DatabaseController : MonoBehaviour
{
    public GameObject FaderIn;
    public GameObject FaderOut;

    public GameObject PlayerEditor, DeviceEditor;
    public GameObject PlayerEditorPanel, DeviceEditorPanel;
    public Slider PlayerTableSwitch, DeviceTableSwitch;
    public GameObject PlayerPicture, DevicePicture;
    public InputField PlayerName, DeviceName, PlayerID, DeviceID;
    public Dropdown Team, Type;
    public Toggle DeviceDestructible;
    public InputField DeviceDescription;
    public Button NewPlayerButton, EditPlayerButton, DeletePlayerButton, SavePlayerButton, CancelPlayerButton, PlayerReturnButton;
    public Button NewDeviceButton, EditDeviceButton, DeleteDeviceButton, SaveDeviceButton, CancelDeviceButton, DeviceReturnButton;
    public GameObject PlayerListPanel, DeviceListPanel;
    public ListViewBehavior PlayerListView, DeviceListView;

    public GameObject Dimmer;
    public GameObject PlayerAreYouSure, DeviceAreYouSure, InvalidName, InvalidID, DuplicateID, InvalidTeam, InvalidType;
    public Sprite DefaultPlayer, DefaultDevice;

    private Color _unselectedColor;
    private Color textColor = new Color(50f/255f,50f/255f,50f/255f);
    private bool _editing;
    private string _originalName, _originalID, _originalDescription;
    private int _originalTeam, _originalType;
    private bool _originalDestructible;
    private Sprite _originalPicture;

    void OnLevelWasLoaded()
    {
        FaderIn.SetActive(true);
    }

    void Start()
    {
        _unselectedColor = Team.GetComponentInChildren<Text>().color;
        OnPlayerSelected(null);
    }

    public void Return()
    {
        FaderOut.GetComponent<FadeOut>().Scene = "Config";
        FaderOut.SetActive(true);
    }

    public void DialogOK()
    {
        Dimmer.SetActive(false);
        InvalidName.SetActive(false);
        InvalidID.SetActive(false);
        DuplicateID.SetActive(false);
        InvalidTeam.SetActive(false);
        InvalidType.SetActive(false);
    }

    Sprite SelectPicture()
    {
        Texture2D picture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        using (OpenFileDialog open = new OpenFileDialog
        {
            InitialDirectory = @"%HOMEPATH%\Pictures\",
            Filter = "PNG, JPG|*.png;*.jpg",
            Title = "Choose an image",
        })
        {
            if (open.ShowDialog() == DialogResult.OK)
            {
                if (picture.LoadImage(File.ReadAllBytes(open.FileName)))
                {
                    return Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));
                }
            }
        }
        return null;
    }

    #region PlayerEditor

    public void PlayerSwitch()
    {
        if (PlayerTableSwitch.value == 1)
        {
            OnPlayerSelected(null);
            PlayerEditor.SetActive(false);
            DeviceEditor.SetActive(true);
            DeviceTableSwitch.value = 1;
        }
    }

    public void SelectPlayerPicture()
    {
        Sprite sprite = SelectPicture();
        if (sprite != null)
        {
            PlayerPicture.GetComponent<Image>().sprite = sprite;
            PlayerPicture.GetComponent<Image>().color = Color.white;
        }
    }

    public void OnPlayerSelected(GameObject listItem)
    {
        if (listItem == null || listItem == PlayerListPanel)
        {
            PlayerName.text = "";
            PlayerID.text = "";
            Team.value = 255;
            Text listbox = Team.GetComponentInChildren<Text>();
            listbox.text = "Team";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = _unselectedColor;
            PlayerPicture.GetComponent<Image>().sprite = null;
            PlayerPicture.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            PlayerEditorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            NewPlayerButton.interactable = true;
            EditPlayerButton.interactable = false;
            DeletePlayerButton.interactable = false;
            return;
        }

        Text name = listItem.transform.FindChild("Name").GetComponent<Text>();
        Text id = listItem.transform.FindChild("ID").GetComponent<Text>();
        Text team = listItem.transform.FindChild("Team").GetComponent<Text>();
        Sprite picture = listItem.transform.FindChild("Picture").GetComponent<Image>().sprite;

        PlayerName.text = name.text;
        PlayerID.text = id.text;
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

        PlayerPicture.GetComponent<Image>().sprite = picture;
        PlayerPicture.GetComponent<Image>().color = Color.white;

        NewPlayerButton.interactable = true;
        EditPlayerButton.interactable = true;
        DeletePlayerButton.interactable = true;
    }

    public void OnTeamChanged()
    {
        Text listbox = Team.GetComponentInChildren<Text>();
        if (Team.value == 255)
        {
            listbox.text = "Team";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = _unselectedColor;
        }
        else
        {
            listbox.fontStyle = FontStyle.Normal;
            listbox.color = textColor;
        }
        switch (Team.value)
        {
            case 0:
                PlayerEditorPanel.GetComponent<Image>().color = new Color(1, 0, 0, 0.5f);
                break;
            case 1:
                PlayerEditorPanel.GetComponent<Image>().color = new Color(1, 1, 0, 0.5f);
                break;
            case 2:
                PlayerEditorPanel.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                break;
            case 3:
                PlayerEditorPanel.GetComponent<Image>().color = new Color(0, 0, 1, 0.5f);
                break;
            default:
                PlayerEditorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
                break;
        }
    }

    public void NewPlayer()
    {
        _editing = false;
        _originalID = "";
        OnPlayerSelected(null);
        NewPlayerButton.interactable = false;
        EditPlayerButton.interactable = false;
        DeletePlayerButton.interactable = false;
        SavePlayerButton.interactable = true;
        CancelPlayerButton.interactable = true;
        PlayerTableSwitch.interactable = false;
        PlayerReturnButton.interactable = false;

        PlayerPicture.GetComponent<Button>().interactable = true;
        PlayerPicture.GetComponent<Image>().sprite = DefaultPlayer;
        PlayerPicture.GetComponent<Image>().color = Color.white;
        PlayerName.interactable = true;
        PlayerID.interactable = true;
        Team.interactable = true;
        PlayerListView.Disable();
    }

    public void EditPlayer()
    {
        _editing = true;
        _originalName = PlayerName.text;
        _originalID = PlayerID.text;
        _originalTeam = Team.value;
        _originalPicture = PlayerPicture.GetComponent<Image>().sprite;
        NewPlayerButton.interactable = false;
        EditPlayerButton.interactable = false;
        DeletePlayerButton.interactable = false;
        SavePlayerButton.interactable = true;
        CancelPlayerButton.interactable = true;
        PlayerTableSwitch.interactable = false;
        PlayerReturnButton.interactable = false;

        PlayerPicture.GetComponent<Button>().interactable = true;
        PlayerName.interactable = true;
        PlayerID.interactable = true;
        Team.interactable = true;
        PlayerListView.Disable();
    }

    public void DeletePlayer()
    {
        Dimmer.SetActive(true);
        PlayerAreYouSure.SetActive(true);
    }

    public void YesDeletePlayer()
    {
        Database.DeletePlayer(PlayerID.text);
        OnPlayerSelected(null);
        PlayerListView.Empty();
        PlayerListView.Fill();
        Dimmer.SetActive(false);
        PlayerAreYouSure.SetActive(false);
    }

    public void NoDeletePlayer()
    {
        Dimmer.SetActive(false);
        PlayerAreYouSure.SetActive(false);
    }

    public void SavePlayer()
    {
        PlayerName.text = PlayerName.text.Trim();
        if (string.IsNullOrEmpty(PlayerName.text))
        {
            Dimmer.SetActive(true);
            InvalidName.SetActive(true);
            return;
        }
        ushort id;
        PlayerID.text = PlayerID.text.ToUpper();
        if(!ushort.TryParse(PlayerID.text, NumberStyles.HexNumber, null, out id))
        {
            Dimmer.SetActive(true);
            InvalidID.SetActive(true);
            return;
        }
        if (PlayerListView.FindDuplicateID(PlayerID.text))
        {
            if (PlayerID.text != _originalID)
            {
                Dimmer.SetActive(true);
                DuplicateID.SetActive(true);
                return;
            }
        }
        if(Team.value == 255)
        {
            Dimmer.SetActive(true);
            InvalidTeam.SetActive(true);
            return;
        }

        if (_editing)
            Database.UpdatePlayer(_originalID, PlayerName.text, PlayerID.text, PlayerPicture.GetComponent<Image>().sprite.texture, Team.captionText.text);
        else
            Database.InsertPlayer(PlayerName.text, PlayerID.text, PlayerPicture.GetComponent<Image>().sprite.texture, Team.captionText.text);

        PlayerListView.Empty();
        PlayerListView.Fill();
        PlayerName.interactable = false;
        PlayerID.interactable = false;
        Team.interactable = false;
        NewPlayerButton.interactable = true;
        EditPlayerButton.interactable = true;
        DeletePlayerButton.interactable = true;
        SavePlayerButton.interactable = false;
        CancelPlayerButton.interactable = false;
        PlayerTableSwitch.interactable = true;
        PlayerReturnButton.interactable = true;
        PlayerListView.Enable();
    }

    public void CancelPlayer()
    {
        PlayerPicture.GetComponent<Button>().interactable = false;
        PlayerName.interactable = false;
        PlayerID.interactable = false;
        Team.interactable = false;
        NewPlayerButton.interactable = true;
        SavePlayerButton.interactable = false;
        CancelPlayerButton.interactable = false;
        PlayerTableSwitch.interactable = true;
        PlayerReturnButton.interactable = true;

        if (_editing)
        {
            PlayerPicture.GetComponent<Image>().sprite = _originalPicture;
            PlayerName.text = _originalName;
            PlayerID.text = _originalID;
            Team.value = _originalTeam;
            EditPlayerButton.interactable = true;
            DeletePlayerButton.interactable = true;
        }
        else
        {
            OnPlayerSelected(null);
        }

        PlayerListView.Enable();
    }
    #endregion


    #region DeviceEditor

    public void DeviceSwitch()
    {
        if (DeviceTableSwitch.value == 0)
        {
            OnDeviceSelected(null);
            PlayerEditor.SetActive(true);
            DeviceEditor.SetActive(false);
            PlayerTableSwitch.value = 0;
        }
    }

    public void SelectDevicePicture()
    {
        Sprite sprite = SelectPicture();
        if (sprite != null)
        {
            DevicePicture.GetComponent<Image>().sprite = sprite;
            DevicePicture.GetComponent<Image>().color = Color.white;
        }
        SetDevicePanelColor();
    }

    public void SetDevicePanelColor()
    {
        if(DevicePicture.GetComponent<Image>().sprite != null)
        {
            Texture2D picture = DevicePicture.GetComponent<Image>().sprite.texture;
            Color color = picture.GetPixel(picture.width / 2, picture.height / 2);
            color.a = 0.5f;
            DeviceEditorPanel.GetComponent<Image>().color = color;
        }
        else
        {
            DeviceEditorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void OnDeviceSelected(GameObject listItem)
    {
        if (listItem == null || listItem == DeviceListPanel)
        {
            DeviceName.text = "";
            DeviceID.text = "";
            Type.value = 255;
            Text listbox = Type.GetComponentInChildren<Text>();
            listbox.text = "Type";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = _unselectedColor;
            DevicePicture.GetComponent<Image>().sprite = null;
            DevicePicture.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            DeviceEditorPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            DeviceDestructible.isOn = false;
            DeviceDescription.text = "";

            NewDeviceButton.interactable = true;
            EditDeviceButton.interactable = false;
            DeleteDeviceButton.interactable = false;
            return;
        }

        Text name = listItem.transform.FindChild("Name").GetComponent<Text>();
        Text id = listItem.transform.FindChild("ID").GetComponent<Text>();
        Text type = listItem.transform.FindChild("Type").GetComponent<Text>();
        Sprite picture = listItem.transform.FindChild("Picture").GetComponent<Image>().sprite;
        bool destructible = listItem.transform.FindChild("Destructible").GetComponentInChildren<Toggle>().isOn;
        Text description = listItem.transform.FindChild("Description").GetComponent<Text>();

        DeviceName.text = name.text;
        DeviceID.text = id.text;
        
        switch (type.text)
        {
            case "Generic":
                Type.value = 0;
                break;
            case "Fire":
                Type.value = 1;
                break;
            case "Water":
                Type.value = 2;
                break;
            case "Earth":
                Type.value = 3;
                break;
            case "Wind":
                Type.value = 4;
                break;
            case "Ice":
                Type.value = 5;
                break;
            case "Rock":
                Type.value = 6;
                break;
            case "Thunder":
                Type.value = 7;
                break;
            case "Poison":
                Type.value = 8;
                break;
            case "Psychic":
                Type.value = 9;
                break;
            case "Ghost":
                Type.value = 10;
                break;
            case "Shadow":
                Type.value = 11;
                break;
            case "Light":
                Type.value = 12;
                break;
        }
        
        DevicePicture.GetComponent<Image>().sprite = picture;
        DevicePicture.GetComponent<Image>().color = Color.white;
        DeviceDestructible.isOn = destructible;
        DeviceDescription.text = description.text;

        NewDeviceButton.interactable = true;
        EditDeviceButton.interactable = true;
        DeleteDeviceButton.interactable = true;
        SetDevicePanelColor();
    }

    public void OnTypeChanged()
    {
        Text listbox = Type.GetComponentInChildren<Text>();
        if (Type.value == 255)
        {
            listbox.text = "Type";
            listbox.fontStyle = FontStyle.Italic;
            listbox.color = _unselectedColor;
        }
        else
        {
            listbox.fontStyle = FontStyle.Normal;
            listbox.color = textColor;
        }
    }

    public void NewDevice()
    {
        _editing = false;
        OnDeviceSelected(null);
        NewDeviceButton.interactable = false;
        EditDeviceButton.interactable = false;
        DeleteDeviceButton.interactable = false;
        SaveDeviceButton.interactable = true;
        CancelDeviceButton.interactable = true;
        DeviceTableSwitch.interactable = false;
        DeviceReturnButton.interactable = false;

        DevicePicture.GetComponent<Button>().interactable = true;
        DevicePicture.GetComponent<Image>().sprite = DefaultDevice;
        DevicePicture.GetComponent<Image>().color = Color.white;
        DeviceName.interactable = true;
        DeviceID.interactable = true;
        Type.interactable = true;
        DeviceDestructible.interactable = true;
        DeviceDescription.interactable = true;
        DeviceListView.Disable();
    }

    public void EditDevice()
    {
        _editing = true;
        _originalName = DeviceName.text;
        _originalID = DeviceID.text;
        _originalType = Type.value;
        _originalPicture = DevicePicture.GetComponent<Image>().sprite;
        _originalDestructible = DeviceDestructible.isOn;
        _originalDescription = DeviceDescription.text;
        NewDeviceButton.interactable = false;
        EditDeviceButton.interactable = false;
        DeleteDeviceButton.interactable = false;
        SaveDeviceButton.interactable = true;
        CancelDeviceButton.interactable = true;
        DeviceTableSwitch.interactable = false;
        DeviceReturnButton.interactable = false;

        DevicePicture.GetComponent<Button>().interactable = true;
        DeviceName.interactable = true;
        DeviceID.interactable = true;
        Type.interactable = true;
        DeviceDestructible.interactable = true;
        DeviceDescription.interactable = true;
        DeviceListView.Disable();
    }

    public void DeleteDevice()
    {
        Dimmer.SetActive(true);
        DeviceAreYouSure.SetActive(true);
    }

    public void YesDeleteDevice()
    {
        Database.DeleteDevice(DeviceID.text);
        OnDeviceSelected(null);
        DeviceListView.Empty();
        DeviceListView.Fill();
        Dimmer.SetActive(false);
        DeviceAreYouSure.SetActive(false);
    }

    public void NoDeleteDevice()
    {
        Dimmer.SetActive(false);
        DeviceAreYouSure.SetActive(false);
    }

    public void SaveDevice()
    {
        DeviceName.text = DeviceName.text.Trim();
        if (string.IsNullOrEmpty(DeviceName.text))
        {
            Dimmer.SetActive(true);
            InvalidName.SetActive(true);
            return;
        }
        ushort id;
        DeviceID.text = DeviceID.text.ToUpper();
        if (!ushort.TryParse(DeviceID.text, NumberStyles.HexNumber, null, out id))
        {
            Dimmer.SetActive(true);
            InvalidID.SetActive(true);
            return;
        }
        if (DeviceListView.FindDuplicateID(DeviceID.text))
        {
            Dimmer.SetActive(true);
            DuplicateID.SetActive(true);
            return;
        }
        if (Type.value == 255)
        {
            Dimmer.SetActive(true);
            InvalidType.SetActive(true);
            return;
        }

        if (_editing)
            Database.UpdateDevice(_originalID, DeviceName.text, DeviceID.text, DevicePicture.GetComponent<Image>().sprite.texture, Type.captionText.text, DeviceDescription.text, (DeviceDestructible.isOn) ? 1 : 0);
        else
            Database.InsertDevice(DeviceName.text, DeviceID.text, DevicePicture.GetComponent<Image>().sprite.texture, Type.captionText.text, DeviceDescription.text, (DeviceDestructible.isOn) ? 1 : 0);

        DeviceListView.Empty();
        DeviceListView.Fill();
        DeviceName.interactable = false;
        DeviceID.interactable = false;
        Type.interactable = false;
        DeviceDestructible.interactable = false;
        DeviceDescription.interactable = false;
        NewDeviceButton.interactable = true;
        EditDeviceButton.interactable = true;
        DeleteDeviceButton.interactable = true;
        SaveDeviceButton.interactable = false;
        CancelDeviceButton.interactable = false;
        DeviceTableSwitch.interactable = true;
        DeviceReturnButton.interactable = true;
        DeviceListView.Enable();
    }

    public void CancelDevice()
    {
        DevicePicture.GetComponent<Button>().interactable = false;
        DeviceName.interactable = false;
        DeviceID.interactable = false;
        Type.interactable = false;
        DeviceDestructible.interactable = false;
        DeviceDescription.interactable = false;
        NewDeviceButton.interactable = true;
        SaveDeviceButton.interactable = false;
        CancelDeviceButton.interactable = false;
        DeviceTableSwitch.interactable = true;
        DeviceReturnButton.interactable = true;

        if (_editing)
        {
            DevicePicture.GetComponent<Image>().sprite = _originalPicture;
            DeviceName.text = _originalName;
            DeviceID.text = _originalID;
            Type.value = _originalType;
            DeviceDestructible.isOn = _originalDestructible;
            DeviceDescription.text = _originalDescription;
            EditDeviceButton.interactable = true;
            DeleteDeviceButton.interactable = true;
        }
        else
        {
            OnDeviceSelected(null);
        }

        DeviceListView.Enable();
    }
    #endregion
}
