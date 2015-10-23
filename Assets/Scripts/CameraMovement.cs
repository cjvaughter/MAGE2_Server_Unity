using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float ScrollSpeed = 8;
    public float PanSpeed = 2;
    private Vector3 _mouseOrigin = new Vector3(0,0,0);	// Position of cursor when mouse dragging starts
    private Vector3 _pos = new Vector3(0, 0, 0);
    private Vector3 _startPos;
    private bool _panning;		// Is the camera being panned?
    private float _zoom;


    void Start()
    {
        _startPos = transform.position;
        _zoom = 0;
    }

    void LateUpdate()
    {
        //Scroll
        _zoom += Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        _zoom = Mathf.Clamp(_zoom, 4, 30);

        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, _zoom, 0.10f);

        //Pan
        if (Input.GetMouseButtonDown(0))
        {
            _mouseOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            _startPos = transform.position;
            _panning = true;
        }
        if (!Input.GetMouseButton(0))
        {
            _panning = false;
        }
        if (_panning)
        {
            _pos = (_mouseOrigin - Camera.main.ScreenToViewportPoint(Input.mousePosition)) * PanSpeed * Camera.main.orthographicSize;
        }
        transform.position = Vector3.Lerp(transform.position, _startPos + _pos, 0.15f);
    }
}
