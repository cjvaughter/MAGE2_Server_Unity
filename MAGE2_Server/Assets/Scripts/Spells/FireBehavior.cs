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
        ParticleSystem.EmissionModule emitter;
        emitter = Fire.emission; emitter.enabled = false;
        emitter = Flames.emission; emitter.enabled = false;
        emitter = Smoke.emission; emitter.enabled = false;
        emitter = Glow.emission; emitter.enabled = false;
        emitter = Sparks.emission; emitter.enabled = false;
        _targetVolume = 0;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
