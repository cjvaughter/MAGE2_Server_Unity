using UnityEngine;
using UnityEngine.EventSystems;

public class PanelBehavior : MonoBehaviour
{
    void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Select();
    }

    public void Select()
    {
        Camera.main.GetComponent<CameraMovement>().SelectPlayer(gameObject);
    }
}
