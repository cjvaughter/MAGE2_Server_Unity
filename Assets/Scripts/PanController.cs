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

	void Update ()
    {
        if (!Input.GetMouseButton(0) && _panning)
        {
            _panning = false;
        }

        if (_panning)
        {
            Vector3 pos = ((_mouseOrigin - mainCamera.ScreenToViewportPoint(Input.mousePosition)) * PanSpeed * mainCamera.orthographicSize) + _startPos;
            mainCamera.GetComponent<CameraMovement>().SetPosition(pos);
        }
	}
}
