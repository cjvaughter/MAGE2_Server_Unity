using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ListViewBehavior : MonoBehaviour
{
    public DatabaseController Controller;
    public GameObject RowPrefab;
    public Scrollbar Scroll;
    public GameObject Spacer;
    public bool DeviceTable = false;

    private int _rowCount = 0;

	void Start ()
    {
        //Spacer.GetComponent<LayoutElement>().minHeight = gameObject.GetComponentInParent<RectTransform>().rect.height;
        Database.Create();
        Fill();
	}

    void OnItemSelected(GameObject newRow)
    {
        if(DeviceTable) Controller.OnDeviceSelected(newRow);
        else Controller.OnPlayerSelected(newRow);
    }

    public void Fill()
    {
        if (DeviceTable)
        {
            Database.FillDevices();
            foreach (DataRow row in Database.DTDevices.Rows)
            {
                Texture2D picture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                GameObject newRow = Instantiate(RowPrefab, new Vector3(), Quaternion.identity) as GameObject;
                newRow.transform.SetParent(gameObject.transform, false);

                newRow.transform.FindChild("Name").GetComponent<Text>().text = (string)row["Name"];
                newRow.transform.FindChild("ID").GetComponent<Text>().text = (string)row["ID"];
                if (picture.LoadImage((byte[])row["Picture"]))
                {
                    newRow.transform.FindChild("Picture").GetComponent<Image>().sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));
                }
                newRow.transform.FindChild("Type").GetComponent<Text>().text = (string)row["Type"];
                newRow.transform.FindChild("Destructible").GetComponentInChildren<Toggle>().isOn = (((long)row["Destructible"]) == 1);
                newRow.transform.FindChild("Description").GetComponent<Text>().text = (string)row["Description"];

                newRow.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(newRow));

                if (Spacer.activeSelf)
                {
                    Spacer.GetComponent<LayoutElement>().minHeight -= 57;
                    if (Spacer.GetComponent<LayoutElement>().minHeight <= 0) Spacer.SetActive(false);
                }
                Spacer.transform.SetAsLastSibling();
                _rowCount++;
            }
            Scroll.value = 1;
        }
        else
        {
            Database.FillPlayers();
            foreach (DataRow row in Database.DTPlayers.Rows)
            {
                Texture2D picture = new Texture2D(1, 1, TextureFormat.RGB24, false);
                GameObject newRow = Instantiate(RowPrefab, new Vector3(), Quaternion.identity) as GameObject;
                newRow.transform.SetParent(gameObject.transform, false);

                newRow.transform.FindChild("Name").GetComponent<Text>().text = (string)row["Name"];
                newRow.transform.FindChild("ID").GetComponent<Text>().text = (string)row["ID"];
                if (picture.LoadImage((byte[])row["Picture"]))
                {
                    newRow.transform.FindChild("Picture").GetComponent<Image>().sprite = Sprite.Create(picture, new Rect(0, 0, picture.width, picture.height), new Vector2(0.5f, 0.5f));
                }
                newRow.transform.FindChild("Team").GetComponent<Text>().text = (string)row["Team"];
                newRow.transform.FindChild("Level").GetComponent<Text>().text = ((long)row["Level"]).ToString();
                newRow.transform.FindChild("Strength").GetComponent<Text>().text = ((long)row["Strength"]).ToString();
                newRow.transform.FindChild("Defense").GetComponent<Text>().text = ((long)row["Defense"]).ToString();
                newRow.transform.FindChild("Luck").GetComponent<Text>().text = ((long)row["Luck"]).ToString();
                newRow.transform.FindChild("Health").GetComponent<Text>().text = ((long)row["Health"]).ToString();

                newRow.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(newRow));

                if (Spacer.activeSelf)
                {
                    Spacer.GetComponent<LayoutElement>().minHeight -= 57;
                    if (Spacer.GetComponent<LayoutElement>().minHeight <= 0) Spacer.SetActive(false);
                }
                Spacer.transform.SetAsLastSibling();
                _rowCount++;
            }
            Scroll.value = 1;
        }
    }

    public void Empty()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if(t.gameObject.name != "Spacer" && t.gameObject.name != "ListView")
                Destroy(t.gameObject);
        }
        Spacer.SetActive(true);
        Spacer.GetComponent<LayoutElement>().minHeight = gameObject.GetComponentInParent<RectTransform>().rect.height;
        //Spacer.GetComponent<LayoutElement>().preferredHeight = 774;
        _rowCount = 0;
        Scroll.value = 1;
    }

    public void Enable()
    {
        foreach (Button b in gameObject.GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name != "Spacer" && b.gameObject.name != "ListView")
                b.interactable = true;
        }
    }

    public void Disable()
    {
        foreach (Button b in gameObject.GetComponentsInChildren<Button>())
        {
            if (b.gameObject.name != "Spacer" && b.gameObject.name != "ListView")
                b.interactable = false;
        }
    }

    public bool FindDuplicateID(string id)
    {
        if (DeviceTable)
        {
            foreach (DataRow row in Database.DTDevices.Rows)
            {
                if (id == (string)row["ID"]) return true;
            }
        }
        else
        {
            foreach (DataRow row in Database.DTPlayers.Rows)
            {
                if (id == (string)row["ID"]) return true;
            }
        }
        return false;
    }
}
