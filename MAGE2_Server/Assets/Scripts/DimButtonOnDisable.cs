using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DimButtonOnDisable : MonoBehaviour
{
    Button button;
    Image image;
    bool interactable = true;

	void Start ()
    {
        button = GetComponent<Button>();
        image = gameObject.transform.GetChild(0).GetComponent<Image>();
    }

	void Update()
    {
	    if(button.interactable != interactable)
        {
            interactable = button.interactable;
            if(interactable)
            {
                image.color = Color.white;
            }
            else
            {
                image.color = Color.gray;
            }
        }
	}
}
