using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public float scrollSpeed = 8;
    public float panSpeed = 2;
    private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
    private bool isPanning;		// Is the camera being panned?

    void LateUpdate()
    {
        //Scroll
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, 4, 30);


        //Pan
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse origin
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if (!Input.GetMouseButton(0)) isPanning = false;

        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
            pos.z = 0;
            transform.Translate(pos, Space.Self);
        }
    }
}
