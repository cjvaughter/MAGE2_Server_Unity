using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PanController : MonoBehaviour
{
    public float PanSpeed;
    public Camera mainCamera;
    

    private Vector3 _startPos;
    private Vector3 _mouseOrigin;
    private bool _panning;	

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (!_panning)
        {
            _startPos = mainCamera.transform.position;
            _mouseOrigin = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            _panning = true;
        }
    }

	void Update()
    {
        if (!Input.GetMouseButton(0) && _panning)
        {
            _panning = false;
        }

        if (_panning)
        {
            Vector3 movement = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            movement.x = _mouseOrigin.x;
            Vector3 pos = ((_mouseOrigin - movement) * PanSpeed * mainCamera.orthographicSize) + _startPos;
            pos.y = Mathf.Clamp(pos.y, -10, 0);
            mainCamera.GetComponent<CameraMovement>().SetPosition(pos);
        }
	}
}
