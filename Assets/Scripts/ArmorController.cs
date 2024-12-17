using System;
using Unity.VisualScripting;
using UnityEngine;

public class ArmorController : MonoBehaviour
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
        if (PC == null) return;
        if (!PC.SpFlip && !doOnce)
        {
            transform.position = new Vector3(transform.position.x + 0.6f, transform.position.y, transform.position.z);
            doOnce = true;
        }
        else if(PC.SpFlip && doOnce)
        {
            transform.position = new Vector3(transform.position.x -0.6f, transform.position.y, transform.position.z);
            doOnce = false;
        }
    }


    public void RoundReset()
    {
        if (owner == null) return;
        PC.armorHit = false;
        gameObject.transform.parent = null;
        transform.position = startPos;
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
        posPos.x = !PC.SpFlip? posPos.x +0.3f : posPos.x -0.3f;
        doOnce = !PC.SpFlip;
        transform.position = posPos;
        PC.armorHit = true;


    }
}
