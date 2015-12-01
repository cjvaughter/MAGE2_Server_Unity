using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera MainCamera;
    public float ScrollSpeed = 8;
    public int PlayerRows = 3;

    public Vector3 pos = new Vector3();
    private float _zoom;

    private float _defaultZoom = 8;
    private float _zoomIn = 4;

    void Start()
    {
        float aspect = (float)Screen.width / (float)Screen.height;

        if (Mathf.Abs(aspect - Constants.Aspect_16_9) < 0.01)
        {
            //all good
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_16_10) < 0.01)
        {
            //also good
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_4_3) < 0.01)
        {
            _defaultZoom = 11;
            pos.y = -2f;
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_5_4) < 0.01)
        {
            _defaultZoom = 12;
            pos.y = -1.5f;
        }

        MainCamera.orthographicSize = _defaultZoom;
        _zoom = _defaultZoom;
        pos.z = -10;
        transform.position = pos;
    }

    void LateUpdate()
    {
        //Scroll
        /*
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _zoom -= Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed;
        }
        */
        _zoom = Mathf.Clamp(_zoom, _zoomIn, _defaultZoom);
        MainCamera.orthographicSize = Mathf.Lerp(MainCamera.orthographicSize, _zoom, 0.10f);
        if(Mathf.Abs(MainCamera.orthographicSize - _zoom) < 0.05f)
        {
            MainCamera.orthographicSize = _zoom;
        }

        //Pan
        transform.position = Vector3.Lerp(transform.position, pos, 0.15f);
    }

    public void SetZoom(float zoom)
    {
        _zoom = _defaultZoom - ((zoom / 100) * (_defaultZoom - _zoomIn));
    }

    public void SetPosition(Vector3 pos, bool clamped = true)
    {
        this.pos = pos;
        this.pos.z = -10;
        if (clamped)
        {
            this.pos.y = Mathf.Clamp(this.pos.y, (float)-3.5 * (PlayerRows - 3), 0);
            this.pos.x = 0;
        }
    }

    public void SetRows(int num)
    {
        if(num > 3) PlayerRows = num;
    }
}
