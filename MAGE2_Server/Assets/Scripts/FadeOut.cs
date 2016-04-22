using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image image;
    public string Scene = "";
    private bool _lerping = true;
    private Color _target;

    void Start()
    {
        _target = image.color;
        _target.a = 1.0f;
    }

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
                    SceneManager.LoadScene(Scene);
            }
        }
    }


}