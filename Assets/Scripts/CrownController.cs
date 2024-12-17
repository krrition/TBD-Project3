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
        if (owner == null || !owner.GetComponent<PlayerController>().isGhost) return;
        gameObject.transform.parent = null;
        crownedPos = owner.transform.position;
        crownedPos.y = 0.5f;
        transform.position = crownedPos;
        p1W = false;
        p2W = false;
        owner = null;
    }

    public void RoundReset()
    {
        if (owner == null) return;
        gameObject.transform.parent = null;
        p1W = false;
        p2W = false;
        transform.position = startPos;
        owner = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null) return;
        if (!other.CompareTag("P1") && !other.CompareTag("P2")) return;
        if (other.GetComponent<PlayerController>().isGhost) return;
        if (other.CompareTag("P1"))
        {
            gameObject.transform.parent = other.GameObject().transform;
            crownedPos = other.GameObject().transform.position;
            crownedPos.y += 1f;
            transform.position = crownedPos;
            p1W = true;
            p2W = false;
            owner = other.GameObject();
            

            

        }
        
        else if (other.CompareTag("P2"))
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
