using UnityEngine;

public class PanelBehavior : MonoBehaviour
{
    public Camera mainCamera;

    void OnMouseUpAsButton()
    {
        Select();
    }

    void Select()
    {
        mainCamera.GetComponent<CameraMovement>().SetPosition(transform.position);
        mainCamera.GetComponent<CameraMovement>().SetZoom(100);
    }
}
