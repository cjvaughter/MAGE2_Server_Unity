using UnityEngine;
using UnityEngine.EventSystems;

public class PanController : MonoBehaviour
{
    public float PanSpeed;
    public Camera MainCamera;
    

    private Vector3 _startPos;
    private Vector3 _mouseOrigin;
    private bool _panning;	
    
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!_panning)
        {
            _startPos = MainCamera.transform.position;
            _mouseOrigin = MainCamera.ScreenToViewportPoint(Input.mousePosition);
            _panning = true;
        }
    }
    
	void LateUpdate()
    {
        if (!Input.GetMouseButton(0) && _panning)
        {
            _panning = false;
        }
	}

    void OnMouseDrag()
    {
        if (_panning)
        {
            Vector3 movement = MainCamera.ScreenToViewportPoint(Input.mousePosition);
            movement.x = _mouseOrigin.x;
            Vector3 pos = ((_mouseOrigin - movement) * PanSpeed * MainCamera.orthographicSize) + _startPos;
            MainCamera.GetComponent<CameraMovement>().SetPosition(pos);
        }
    }
}
