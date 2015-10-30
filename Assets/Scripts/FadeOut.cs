using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image image;
    public string Scene = "";
    private bool _lerping = true;
    private Color _target = new Color(34.0f / 255.0f, 44.0f / 255.0f, 55.0f / 255.0f);

    void FixedUpdate()
    {
        if (_lerping)
        {
            image.color = Color.Lerp(image.color, _target, 0.1f);
            if (_target.a - image.color.a < 0.01f)
            {
                image.color = _target;
                _lerping = false;
                if (Scene == "Exit")
                    Application.Quit();
                else
                    Application.LoadLevel(Scene);
            }
        }
    }


}