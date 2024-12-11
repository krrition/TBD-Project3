using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public bool isGhost;
    public bool doOnce;

    [SerializeField]
    private Material ghostMat;

    [SerializeField]
    private Material normMat;

    private void Update()
    {
        if (isGhost && !doOnce)
        {
            gameObject.GetComponent<MeshRenderer>().material = ghostMat;
            doOnce = true;
        }

        else if (!isGhost && doOnce)
        {
            gameObject.GetComponent<MeshRenderer>().material = normMat;
            doOnce = false;
        }

        
    }
}
    

