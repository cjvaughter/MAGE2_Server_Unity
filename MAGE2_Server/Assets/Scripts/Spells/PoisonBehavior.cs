using UnityEngine;
using System.Collections;

public class PoisonBehavior : MonoBehaviour, IKillable
{
    public ParticleEmitter Smoke;

    public void Kill()
    {
        StartCoroutine("StopEmitting");
    }

    IEnumerator StopEmitting()
    {
        Smoke.emit = false;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
