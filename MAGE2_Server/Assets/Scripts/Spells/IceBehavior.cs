using UnityEngine;
using System.Collections;

public class IceBehavior : MonoBehaviour, IKillable
{
    public AudioSource IceSound;
    public MeshRenderer mr;
    Color target = Color.white;
    float targetVolume = 0.2f;

    void FixedUpdate()
    {
        mr.material.SetColor("_Color", Color.Lerp(mr.material.GetColor("_Color"), target, 0.04f));
        IceSound.volume = Mathf.Lerp(IceSound.volume, targetVolume, 0.1f);
    }

    void OnEnable()
    {
        mr.material.SetColor("_Color", Color.clear);
    }

    public void Kill()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        target = Color.clear;
        targetVolume = 0;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
