using UnityEngine;
using System.Collections;

public class IceBehavior : MonoBehaviour, IKillable
{
    public AudioSource IceSound;
    public MeshRenderer Mr;
    Color _target = Color.white;
    float _targetVolume = 0.2f;

    void FixedUpdate()
    {
        Mr.material.SetColor("_Color", Color.Lerp(Mr.material.GetColor("_Color"), _target, 0.04f));
        IceSound.volume = Mathf.Lerp(IceSound.volume, _targetVolume, 0.1f);
    }

    void OnEnable()
    {
        Mr.material.SetColor("_Color", Color.clear);
    }

    public void Kill()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeOut()
    {
        _target = Color.clear;
        _targetVolume = 0;
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
