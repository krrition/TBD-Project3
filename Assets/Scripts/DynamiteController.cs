using System;
using Unity.VisualScripting;
using UnityEngine;

public class DynamiteController : MonoBehaviour
{
    [NonSerialized]public GameObject owner;
    private Vector3 startPos;
    private Vector3 posPos;
    private PlayerController PC;
    private bool doOnce;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        if (owner == null) return;
        if (PC == null) return;
        OwnerGhosted();
        if (PC.isGhost) return;
        if (!PC.SpFlip && !doOnce)
        {
            transform.position = new Vector3(transform.position.x - 0.6f, transform.position.y, transform.position.z);
            doOnce = true;
        }
        else if(PC.SpFlip && doOnce)
        {
            transform.position = new Vector3(transform.position.x +0.6f, transform.position.y, transform.position.z);
            doOnce = false;
        }
    }
    

    private void OwnerGhosted()
    {
        if (owner == null || !owner.GetComponent<PlayerController>().isGhost) return;
        PC.miteOwn = false;
        gameObject.transform.parent = null;
        owner = null;
    }


    public void RoundReset()
    {
        if (owner != null)PC.miteOwn = false;
        gameObject.transform.parent = null;
        transform.position = startPos;
        GetComponent<BoxCollider>().enabled = true;
        owner = null;
        
    }
    
    public void ArmReset()
    {
        if (owner == null) return;
        PC.miteOwn = false;
        gameObject.transform.parent = null;
        owner = null;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null) return;
        if (!other.CompareTag("P1") && !other.CompareTag("P2")) return;
        if(other.GetComponent<PlayerController>().isGhost) return;
        owner = other.GameObject();
        gameObject.transform.parent = other.GameObject().transform;
        PC = owner.GetComponent<PlayerController>();
        posPos = owner.transform.position;
        posPos.x = !PC.SpFlip? posPos.x -0.3f : posPos.x +0.3f;
        doOnce = !PC.SpFlip;
        transform.position = posPos;
        PC.miteOwn = true;

    }
}
