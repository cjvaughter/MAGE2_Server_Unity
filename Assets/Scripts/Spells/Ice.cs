using UnityEngine;
using System.Collections;

public class Ice : MonoBehaviour
{
    MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        mr.material.SetColor("_Color", Color.Lerp(mr.material.GetColor("_Color"), Color.white, 0.04f));
    }

    void OnEnable()
    {
        mr.material.SetColor("_Color", Color.clear);
    }
}
