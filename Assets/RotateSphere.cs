using UnityEngine;
using System.Collections;

public class RotateSphere : MonoBehaviour
{
    Vector3 startPos;

	void Start()
    {
        startPos = transform.position;
        transform.position = new Vector3(startPos.x + 2, startPos.y, startPos.z);
	}
	
	void FixedUpdate()
    {
        transform.RotateAround(startPos, Vector3.up, 10.0f);
        transform.RotateAround(startPos, Vector3.back, 5.0f);
        //transform.Rotate(new Vector3(10, 0, 0), Space.World);
        //transform.Rotate(new Vector3(0, 1, 0));
    }
}
