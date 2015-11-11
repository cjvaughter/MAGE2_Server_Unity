using UnityEngine;
using System.Collections;

public class Thunder : MonoBehaviour
{
    MeshRenderer mr;
    bool _lerping;

	void Awake()
    {
        mr = GetComponent<MeshRenderer>();
	}
	
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
        //yield return null;
    }
    
    void OnEnable()
    {
       _lerping = false;
        mr.material.SetColor("_Color", Color.white);
        StartCoroutine("Flash");
    }
}
