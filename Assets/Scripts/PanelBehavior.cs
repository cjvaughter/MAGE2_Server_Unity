using UnityEngine;

public class PanelBehavior : MonoBehaviour
{
    void OnMouseUpAsButton()
    {
        Select();
    }

    void Select()
    {
        Camera.main.GetComponent<CameraMovement>().SetPosition(transform.position);
        Camera.main.GetComponent<CameraMovement>().SetZoom(100);
    }
}
