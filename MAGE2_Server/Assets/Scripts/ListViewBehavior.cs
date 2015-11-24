using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class ListViewBehavior : MonoBehaviour
{
    public DatabaseController controller;
    public GameObject RowPrefab;
    public Scrollbar Scroll;
    public GameObject Spacer;

    private int rowCount = 0;

	void Start ()
    {
        //Spacer.GetComponent<LayoutElement>().preferredHeight = gameObject.GetComponent<RectTransform>().
        Database.Connect();
        Fill();
	}
	
	void Update ()
    {
	
	}

    void OnItemSelected(GameObject newRow)
    {
        controller.OnPlayerSelected(newRow);
    }

    public void Fill()
    {
        Database.FillPlayers();
        foreach (System.Data.DataRow row in Database.DTPlayers.Rows)
        {
            Texture2D picture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            GameObject newRow = GameObject.Instantiate(RowPrefab, new Vector3(), Quaternion.identity) as GameObject;
            newRow.transform.parent = gameObject.transform;
            newRow.transform.localScale = new Vector3(1, 1, 1);

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
                Spacer.transform.SetAsLastSibling();
                Spacer.GetComponent<LayoutElement>().preferredHeight -= 57;
                if (Spacer.GetComponent<LayoutElement>().preferredHeight <= 0) Spacer.SetActive(false);
            }
            rowCount++;
        }
        Scroll.value = 1;
    }

    public void Empty()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            if(t.gameObject.name != "Spacer" && t.gameObject.name != "ListView")
                Destroy(t.gameObject);
        }
        Spacer.SetActive(true);
        Spacer.GetComponent<LayoutElement>().preferredHeight = 632;
        rowCount = 0;
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
}
