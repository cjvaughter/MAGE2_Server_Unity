using UnityEngine;

public class GlowBehavior : MonoBehaviour
{
    MeshRenderer mr;
    float val;
    bool _upDown;

	void Start()
    {
        val = 0;
        mr = GetComponent<MeshRenderer>();
	}
	
	void FixedUpdate()
    {
        if (_upDown)
        {
            val = Mathf.Lerp(val, 0.0f, 0.15f);
            if (val < 0.1f)
            {
                _upDown = false;
                val = 0;
            }
        }
        else
        {
            val = Mathf.Lerp(val, 0.75f, 0.15f);
            if (val > 0.7f)
            {
                _upDown = true;
                val = 0.75f;
            }
        }
        mr.material.SetFloat("_MKGlowPower", val);
    }
}
