using UnityEngine;

public class GlowBehavior : MonoBehaviour
{
    MeshRenderer _mr;
    float _val;
    bool _upDown;

	void Start()
    {
        _val = 0;
        _mr = GetComponent<MeshRenderer>();
	}
	
	void FixedUpdate()
    {
        if (_upDown)
        {
            _val = Mathf.Lerp(_val, 0.0f, 0.15f);
            if (_val < 0.1f)
            {
                _upDown = false;
                _val = 0;
            }
        }
        else
        {
            _val = Mathf.Lerp(_val, 0.75f, 0.15f);
            if (_val > 0.7f)
            {
                _upDown = true;
                _val = 0.75f;
            }
        }
        _mr.material.SetFloat("_MKGlowPower", _val);
    }
}
