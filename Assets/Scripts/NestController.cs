using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class NestController : MonoBehaviour
{
    [SerializeField]
    private float stealTime = 3;
    
    private float stealTimer;

    private bool activate;

    public GameObject Stealer;

    public List<GameObject> nextStealers = new List<GameObject>();

    [NonSerialized]
    public bool p1W, p2W;

    [SerializeField] private Sprite neutral, p1, p2;

    private SpriteRenderer SP;

    private void Start()
    {
        SP = GetComponent<SpriteRenderer>();
        stealTimer = stealTime;
    }

    private void Update()
    {
        if (!activate) return;
        if (Stealer != null && !Stealer.GetComponent<PlayerController>().isGhost)
        {
            Stealing();
        }

        else if (Stealer != null && Stealer.GetComponent<PlayerController>().isGhost && nextStealers != null)
        {
            Stealer = null;
            ResetSteal();
            Stealer = nextStealers[0];
        }
        
        else if (Stealer != null && Stealer.GetComponent<PlayerController>().isGhost && nextStealers == null)
        {
            Stealer = null;
            ResetSteal();
            activate = false;
        }
        
    }

    private void Stealing()
    {
        if (stealTimer > 0)
        {
            stealTimer -= Time.deltaTime;
        }
        else
        {
            if (Stealer.CompareTag("P1"))
            {
                p1W = true;
                p2W = false;
                ChangeOwner();
                Stealer = null;
                if (nextStealers!= null)RemoveStealers();
                stealTimer = stealTime;
                if (nextStealers!= null)Stealer = nextStealers.First();
            }

            else if (Stealer.CompareTag("P2"))
            {
                p2W = true;
                p1W = false;
                ChangeOwner();
                Stealer = null;
                if (nextStealers!= null)RemoveStealers();
                stealTimer = stealTime;
                if (nextStealers!= null)Stealer = nextStealers.First();
            }

            

        }
    }

    private void ResetSteal()
    {
        stealTimer = stealTime;
    }

    private void RemoveStealers()
    {
        foreach (var Stlr in nextStealers)
        {
            if (p1W && Stlr.CompareTag("P1"))
            {
                nextStealers.Remove(Stlr);
            }

            else if (p2W && Stlr.CompareTag("P2"))
            {
                nextStealers.Remove(Stlr);
            }
        }
    }

    private void ChangeOwner()
    {
        if (p1W)
        {
            SP.sprite = p1;
        }

        else if (p2W)
        {
            SP.sprite = p2;

        }
        
        else if (!p1W && !p2W)
        {
            SP.sprite = neutral;

        }
    }

    public void RoundReset()
    {
        p1W = false;
        p2W = false;
        
        ChangeOwner();

        stealTimer = stealTime;

        activate = false;

        Stealer = null;

        nextStealers = new List<GameObject>();

        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("P1") && !other.CompareTag("P2")) return;
        if (other.GameObject().GetComponent<PlayerController>().isGhost) return;
        if (p1W && other.CompareTag("P1") || p2W && other.CompareTag("P2")) return;
        
            if (Stealer == null)
            {
                activate = true;
                Stealer = other.GameObject();

            }

            else if (Stealer != null)
            {
                nextStealers.Add(other.GameObject());
            }
        




    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("P1") && !other.CompareTag("P2")) return;
        if (other.GameObject().GetComponent<PlayerController>().isGhost) return;


        if (other.GameObject() == Stealer && nextStealers != null)
        {
            Stealer = null;
            ResetSteal();
            Stealer = nextStealers.First();
            nextStealers.Remove(nextStealers.First());
        }
        
        else if (other.GameObject() == Stealer && nextStealers == null)
        {
            Stealer = null;
            ResetSteal();
            activate = false;

        }
        
        else if (other.GameObject() != Stealer && nextStealers != null)
        {
            nextStealers.Remove(other.GameObject());
        }
    }
}
