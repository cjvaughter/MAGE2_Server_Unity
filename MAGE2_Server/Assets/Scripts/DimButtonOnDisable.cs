using UnityEngine;
using UnityEngine.UI;

public class DimButtonOnDisable : MonoBehaviour
{
    Button _button;
    Image _image;
    bool _interactable = true;

	void Start ()
    {
        _button = GetComponent<Button>();
        _image = gameObject.transform.GetChild(0).GetComponent<Image>();
    }

	void Update()
    {
	    if(_button.interactable != _interactable)
	    {
	        _interactable = _button.interactable;
	        _image.color = _interactable ? Color.white : Color.gray;
	    }
    }
}
