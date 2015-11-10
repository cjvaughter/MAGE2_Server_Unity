using UnityEngine;

public class HealthBehavior : MonoBehaviour
{
    private float _health;
    private MeshRenderer _bar;

    public Material Green, Yellow, Red;

	void Start()
    {
        _bar = GetComponent<MeshRenderer>();
        SetHealth(1.0f);
	}
	
	public void SetHealth(float health)
    {
        _health = health;
        float xScale = (_health * 3.0f);
        float xPos = -0.17f + (_health * 1.5f);
        transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);
        transform.localPosition = new Vector3(xPos, transform.localPosition.y, transform.localPosition.z);

        if(_health >= 0.5f)
        {
            _bar.material = Green;
        }
        else if(_health >= 0.25f)
        {
            _bar.material = Yellow;
        }
        else
        {
            _bar.material = Red;
        }
	}

    public void SetHealth(float health, float maxhealth)
    {
        SetHealth(health / maxhealth);
    }
}
