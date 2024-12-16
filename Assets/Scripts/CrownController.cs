using System;
using Unity.VisualScripting;
using UnityEngine;

public class CrownController : MonoBehaviour
{
    [NonSerialized] public bool p1W, p2W;
    private GameObject owner;
    private Vector3 startPos;
    private Vector3 crownedPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        OwnerGhosted();
    }

    private void OwnerGhosted()
    {
        if (owner != null && owner.GetComponent<PlayerController>().isGhost)
        {
            gameObject.transform.parent = null;
            crownedPos = owner.transform.position;
            crownedPos.y = 0.5f;
            transform.position = crownedPos;
            p1W = false;
            p2W = false;
            owner = null;
        }
    }

    public void RoundReset()
    {
        gameObject.transform.parent = null;
        p1W = false;
        p2W = false;
        owner = null;
        transform.position = startPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("P1") && !other.GetComponent<PlayerController>().isGhost)
        {
            gameObject.transform.parent = other.GameObject().transform;
            crownedPos = other.GameObject().transform.position;
            crownedPos.y += 1f;
            transform.position = crownedPos;
            p1W = true;
            p2W = false;
            owner = other.GameObject();
            

        }
        
        else if (other.CompareTag("P2") && !other.GetComponent<PlayerController>().isGhost)
        {
            gameObject.transform.parent = other.GameObject().transform;
            crownedPos = other.GameObject().transform.position;
            crownedPos.y += 1f;
            transform.position = crownedPos;
            p1W = false;
            p2W = true;
            owner = other.GameObject();
            
        }
    }
}
