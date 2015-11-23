using UnityEngine;
using UnityEngine.EventSystems;

public class PanelBehavior : MonoBehaviour
{
    public ushort ID { get; set; }
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void OnMouseUpAsButton()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Select();
    }

    public void Select()
    {
        gameController.SelectPlayer(this);
    }
}
