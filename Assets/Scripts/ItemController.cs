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


    /*private void OnTriggerEnter(Collider other)
    {
        
        if (gameObject.CompareTag("Trap1") && other.CompareTag("P2") && !isGhost/* && !other.GetComponent<PlayerController>().isGhost#1#
            || gameObject.CompareTag("Trap2") && other.CompareTag("P1") && !isGhost/* && !other.GetComponent<PlayerController>().isGhost#1#)
        {
            Invoke("Deactive",0.4f);
        }
    }

    void Deactive()
    {
        gameObject.SetActive(false);
    }*/

}
    

