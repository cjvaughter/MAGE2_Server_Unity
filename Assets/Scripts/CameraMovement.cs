using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public Camera mainCamera;
    public float ScrollSpeed = 8;
    public int PlayerRows = 3;

    private Vector3 _pos;
    private float _zoom;
    private GameObject _selectedPlayer;
    private Vector3 _lastPos;

    void Start()
    {
        _pos = transform.position;
        _zoom = mainCamera.orthographicSize;
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
        _zoom = Mathf.Clamp(_zoom, 4, 30);
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, _zoom, 0.10f);
        if(Mathf.Abs(mainCamera.orthographicSize - _zoom) < 0.05f)
        {
            mainCamera.orthographicSize = _zoom;
        }

        //Pan
        transform.position = Vector3.Lerp(transform.position, _pos, 0.15f);
    }

    public void SetZoom(float zoom)
    {
        _zoom = (30 - (zoom * 0.3f)) + 4;
    }

    public void SetPosition(Vector3 pos, bool clamped = true)
    {
        _pos = pos;
        _pos.z = -10;
        if (clamped) _pos.y = Mathf.Clamp(_pos.y, (float)-3.5 * (PlayerRows - 3), 0);
    }

    public void SelectPlayer(GameObject player)
    {
        if(_selectedPlayer == null)
        {
            _selectedPlayer = player;
            _lastPos = transform.position;
            SetPosition(player.transform.position, false);
            _zoom = 4;
        }
        else if(_selectedPlayer == player)
        {
            _selectedPlayer = null;
            _pos = _lastPos;
            _zoom = 8;
        }
        else
        {
            _selectedPlayer = player;
            SetPosition(player.transform.position, false);
        }
    }
}
