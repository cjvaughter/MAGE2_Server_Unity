using UnityEngine;
using System.Collections;

public class ThunderBehavior : MonoBehaviour, IKillable
{
    public MeshRenderer Mr;
    public AudioSource ThunderSound, ElectricSound;
    public GameObject Top, Left, Right, Bottom;
    bool _lerping;
	
	void Update()
    {
        if(_lerping)
            Mr.material.SetColor("_Color", Color.Lerp(Mr.material.GetColor("_Color"), Color.clear, 0.15f));
	}
    
    IEnumerator Flash()
    {
        yield return new WaitForSeconds(0.3f);
        _lerping = true;
        yield return new WaitForSeconds(0.4f);
        Mr.material.SetColor("_Color", Color.white);
    }
    
    void OnEnable()
    {
       _lerping = false;
        Mr.material.SetColor("_Color", Color.white);
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
