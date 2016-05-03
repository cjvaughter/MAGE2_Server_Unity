using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera MainCamera;
    public float ScrollSpeed = 8;
    public int PlayerRows = 4;

    public Vector3 pos = new Vector3();
    private float _zoom;

    private float _defaultZoom = 8f;
    private float _zoomIn = 4f;
    private float _defaultPosY = -0.33f;
    private int _numVisibleRows = 3;

    void Start()
    {
        float aspect = (float)Screen.width / (float)Screen.height;

        if (Mathf.Abs(aspect - Constants.Aspect_16_9) <= 0.01f)
        {
            //all good
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_16_10) <= 0.01f)
        {
            _defaultZoom = 9f;
            _defaultPosY = -1f;
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_3_2) <= 0.01f)
        {
            _defaultZoom = 9.5f;
            _defaultPosY = -1.5f;
            _numVisibleRows = 4;
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_4_3) <= 0.01f)
        {
            _defaultZoom = 10.75f;
            _defaultPosY = -2.33f;
            _numVisibleRows = 4;
        }
        else if (Mathf.Abs(aspect - Constants.Aspect_5_4) <= 0.01f)
        {
            _defaultZoom = 11.5f;
            _defaultPosY = -2.75f;
            _numVisibleRows = 4;
        }

        MainCamera.orthographicSize = _defaultZoom;
        _zoom = _defaultZoom;
        pos.y = _defaultPosY;
        pos.z = -10;
        transform.position = pos;
        PlayerRows = _numVisibleRows;
        PlayerRows = 4;
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
            int rows = PlayerRows - _numVisibleRows;
            float min = _defaultPosY - (3.5f * rows);
            this.pos.y = Mathf.Clamp(this.pos.y, min, _defaultPosY);
            this.pos.x = 0;
        }
    }

    public void SetRows(int num)
    {
        PlayerRows = Mathf.Clamp(num, _numVisibleRows, int.MaxValue);
    }
}
