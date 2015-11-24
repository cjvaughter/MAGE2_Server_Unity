using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListViewBehavior : MonoBehaviour
{
    public GameObject RowPrefab;

	void Start ()
    {
        Database.Connect();
        Database.FillPlayers();
        foreach(System.Data.DataRow row in Database.DTPlayers.Rows)
        {
            GameObject newRow = Instantiate(RowPrefab);
            newRow.transform.parent = gameObject.transform;
            newRow.transform.FindChild("Name").GetComponent<Text>().text = (string)row["Name"];
        }
	}
	
	void Update ()
    {
	
	}
}
