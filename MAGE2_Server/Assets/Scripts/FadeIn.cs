using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public Image image;
    private bool _lerping = true;
    private Color _target;

    void Start()
    {
        _target = image.color;
        _target.a = 0f;
    }

	void FixedUpdate ()
    {
	    if(_lerping)
        {
            image.color = Color.Lerp(image.color, Color.clear, 0.1f);
            if(image.color.a - Color.clear.a < 0.1f)
            {
                image.color = Color.clear;
                _lerping = false;
                gameObject.SetActive(false);
            }
        }
	}
}
