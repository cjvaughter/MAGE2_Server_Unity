using UnityEngine;
using System.Collections;

public class ThunderBehavior : MonoBehaviour, IKillable
{
    public MeshRenderer mr;
    public AudioSource ThunderSound, ElectricSound;
    public GameObject Top, Left, Right, Bottom;
    bool _lerping;
	
	void Update()
    {
        if(_lerping)
            mr.material.SetColor("_Color", Color.Lerp(mr.material.GetColor("_Color"), Color.clear, 0.15f));
	}
    
    IEnumerator Flash()
    {
        yield return new WaitForSeconds(0.3f);
        _lerping = true;
        yield return new WaitForSeconds(0.4f);
        mr.material.SetColor("_Color", Color.white);
    }
    
    void OnEnable()
    {
       _lerping = false;
        mr.material.SetColor("_Color", Color.white);
        StartCoroutine("Flash");
    }

    public void Kill()
    {
        StartCoroutine("Stop");
    }

    IEnumerator Stop()
    {
        Destroy(Top);
        Destroy(Left);
        Destroy(Right);
        Destroy(Bottom);
        ElectricSound.Stop();
        while (ThunderSound.isPlaying)
        {
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
}
