using UnityEngine;

public class PanelBehavior : MonoBehaviour
{
    private Vector3 _pos;
    private bool _lerping;

    void OnMouseUpAsButton()
    {
        Select();
    }

    void LateUpdate()
    {
        if (_lerping)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _pos, 0.15f);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 4, 0.15f);
            if (Mathf.Abs(Camera.main.transform.position.x - _pos.x) < 0.02 && Mathf.Abs(Camera.main.transform.position.y - _pos.y) < 0.02 && Camera.main.orthographicSize < 4.02)
            {
                Camera.main.transform.position = _pos;
                Camera.main.orthographicSize = 3;
                _lerping = false;
            }
        }
    }

    void Select()
    {
        _pos = transform.position;
        _pos.z = -10;
        _lerping = true;
    }
}
