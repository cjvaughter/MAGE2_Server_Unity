using UnityEngine;
using System.Collections;

public class PanelBehavior : MonoBehaviour
{
    private Vector3 pos;
    private bool selected = false;
    void OnMouseDown()
    {
        Select();
    }

    void LateUpdate()
    {
        if (selected)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, pos, 0.15f);
            Camera.main.orthographicSize = Vector3.Lerp(new Vector3(0,0,Camera.main.orthographicSize), new Vector3(0,0,4), 0.15f).z;
            if (Camera.main.transform.position.x - pos.x < 0.02 && Camera.main.transform.position.y - pos.y < 0.02 && Camera.main.orthographicSize < 4.02)
            {
                Camera.main.transform.position = pos;
                Camera.main.orthographicSize = 3;
                selected = false;
            }
        }
    }

    void Select()
    {
        pos = transform.position;
        pos.z = -10;
        selected = true;
    }
}
