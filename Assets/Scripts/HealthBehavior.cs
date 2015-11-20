using UnityEngine;

public class HealthBehavior : MonoBehaviour
{
    private float _health, xScale, xPos;
    private MeshRenderer _bar;

    public Material Green, Yellow, Red;

	void Start()
    {
        xScale = 3.0f;
        xPos = 1.33f;
        _bar = GetComponent<MeshRenderer>();
        SetHealth(1.0f);
	}
	
	public void SetHealth(float health)
    {
        _health = health;
        xScale = (_health * 3.0f);
        xPos = -0.17f + (_health * 1.5f);

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

    void FixedUpdate()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(xScale, transform.localScale.y, transform.localScale.z), 0.15f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(xPos, transform.localPosition.y, transform.localPosition.z), 0.15f);
    }
}
