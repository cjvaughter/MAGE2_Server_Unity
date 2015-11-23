using UnityEngine;
using System.Collections;
using VolumetricLines;

public class GenerateLightning : MonoBehaviour
{
    public Vector2 StartPoint, EndPoint;
    public float Range = 0.25f;
    public int FrameRate = 24;
    public VolumetricMultiLineBehavior line;
    Vector3[] Points;
    float linePosY, linePosX;
    public bool Vertical;

    int updateCount = 1;
    int rate;

	void Start()
    {
        Random.seed = Time.frameCount;
        rate = (int)60.0f / FrameRate;
        Points = new Vector3[line.m_lineVertices.Length];
        for (int i = 0; i < Points.Length; i++)
        {
            Points[i].z = -0.1f;
        }
        Points[0].x = StartPoint.x;
        Points[0].y = StartPoint.y;

        linePosY = (StartPoint.y + EndPoint.y) / 2;
        linePosX = (StartPoint.x + EndPoint.x) / 2;
    }

	void FixedUpdate()
    {
	    if(updateCount < rate)
        {
            updateCount++;
            return;
        }

        int length;
        if (Vertical)
        {
            length = (int)(Random.value * 4 + 4);
            float step = Mathf.Abs(EndPoint.y - StartPoint.y) / length;
            for (int i = 1; i < length; i++)
            {
                Points[i].y = StartPoint.y - step * i;
                Points[i].x = linePosX - Range + Random.value * Range * 2;
            }
        }
        else
        {
            length = (int)(Random.value * 12 + 6);
            float step = Mathf.Abs(EndPoint.x - StartPoint.x) / length;
            for (int i = 1; i < length; i++)
            {
                Points[i].x = StartPoint.x + step * i;
                Points[i].y = linePosY - Range + Random.value * Range * 2;
            }
        }

        for (int i = length - 1; i < Points.Length; i++)
        {
            Points[i].x = EndPoint.x;
            Points[i].y = EndPoint.y;
        }
        line.UpdateLineVertices(Points);
        updateCount = 1;
	}
}
