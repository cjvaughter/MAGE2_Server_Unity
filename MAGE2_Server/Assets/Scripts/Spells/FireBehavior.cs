using UnityEngine;
using System.Collections;

public class FireBehavior : MonoBehaviour, IKillable
{
    public ParticleSystem Fire;
    public ParticleSystem Flames;
    public ParticleSystem Smoke;
    public ParticleSystem Glow;
    public ParticleSystem Sparks;
    public AudioSource FireSound;
    private float _targetVolume = 0.2f;

    public void Kill()
    {
        StartCoroutine("StopEmitting");
    }

    void FixedUpdate()
    {
        FireSound.volume = Mathf.Lerp(FireSound.volume, _targetVolume, 0.1f);
    }

    IEnumerator StopEmitting()
    {
        Fire.enableEmission = false;
        Flames.enableEmission = false;
        Smoke.enableEmission = false;
        Glow.enableEmission = false;
        Sparks.enableEmission = false;
        _targetVolume = 0;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
