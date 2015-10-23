using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera mainCamera;
    public float ScrollSpeed = 8;
    private Vector3 _pos = new Vector3();
    private float _zoom;


    void Start()
    {
        _pos = transform.position;
        _zoom = mainCamera.orthographicSize;
    }

    void LateUpdate()
    {
        //Scroll
        _zoom -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        _zoom = Mathf.Clamp(_zoom, 4, 30);

        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, _zoom, 0.10f);
        if(Mathf.Abs(mainCamera.orthographicSize - _zoom) < 0.05f)
        {
            mainCamera.orthographicSize = _zoom;
        }

        transform.position = Vector3.Lerp(transform.position, _pos, 0.15f);
    }

    public void SetZoom(float zoom)
    {
        _zoom = (30 - (zoom * 0.3f)) + 4;
    }

    public void SetPosition(Vector3 pos)
    {
        _pos = pos;
        _pos.z = -10;
    }
}
