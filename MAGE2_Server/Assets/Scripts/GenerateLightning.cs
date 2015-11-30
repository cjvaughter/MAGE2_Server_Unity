using UnityEngine;
using VolumetricLines;

public class GenerateLightning : MonoBehaviour
{
    public Vector2 StartPoint, EndPoint;
    public float Range = 0.25f;
    public int FrameRate = 24;
    public VolumetricMultiLineBehavior Line;
    Vector3[] _points;
    float _linePosY, _linePosX;
    public bool Vertical;

    int _updateCount = 1;
    int _rate;

	void Start()
    {
        Random.seed = Time.frameCount;
        _rate = (int)60.0f / FrameRate;
        _points = new Vector3[Line.m_lineVertices.Length];
        for (int i = 0; i < _points.Length; i++)
        {
            _points[i].z = -0.1f;
        }
        _points[0].x = StartPoint.x;
        _points[0].y = StartPoint.y;

        _linePosY = (StartPoint.y + EndPoint.y) / 2;
        _linePosX = (StartPoint.x + EndPoint.x) / 2;
    }

	void FixedUpdate()
    {
	    if(_updateCount < _rate)
        {
            _updateCount++;
            return;
        }

        int length;
        if (Vertical)
        {
            length = (int)(Random.value * 4 + 4);
            float step = Mathf.Abs(EndPoint.y - StartPoint.y) / length;
            for (int i = 1; i < length; i++)
            {
                _points[i].y = StartPoint.y - step * i;
                _points[i].x = _linePosX - Range + Random.value * Range * 2;
            }
        }
        else
        {
            length = (int)(Random.value * 12 + 6);
            float step = Mathf.Abs(EndPoint.x - StartPoint.x) / length;
            for (int i = 1; i < length; i++)
            {
                _points[i].x = StartPoint.x + step * i;
                _points[i].y = _linePosY - Range + Random.value * Range * 2;
            }
        }

        for (int i = length - 1; i < _points.Length; i++)
        {
            _points[i].x = EndPoint.x;
            _points[i].y = EndPoint.y;
        }
        Line.UpdateLineVertices(_points);
        _updateCount = 1;
	}
}
