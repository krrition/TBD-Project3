using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public bool isGhost;
    public bool doOnce;

    [SerializeField]
    private Material ghostMat;

    private void Update()
    {
        if (!isGhost || doOnce) return;
        gameObject.GetComponent<MeshRenderer>().material = ghostMat;
        doOnce = true;
    }
}
