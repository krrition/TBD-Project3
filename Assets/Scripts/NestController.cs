using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class NestController : MonoBehaviour
{
    [SerializeField]
    private float stealTime = 3;
    
    private float stealTimer;

    private bool activate;
    
    private bool doOnce;

    private GameObject Stealer;

    private List<GameObject> nextStealers = new List<GameObject>();

    [NonSerialized]
    public bool p1W, p2W;

    private void Start()
    {
        stealTimer = stealTime;
    }

    private void Update()
    {
        if (!activate) return;
        if (Stealer != null && !Stealer.GetComponent<PlayerController>().isGhost)
        {
            Stealing();
        }

        else if (Stealer != null && Stealer.GetComponent<PlayerController>().isGhost)
        {
            Stealer = null;
            Stealer = nextStealers.First();
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
            }

            else if (Stealer.CompareTag("P2"))
            {
                p2W = true;
                p1W = false;
            }
        }
    }

    private void ResetSteal()
    {
        Debug.Log("Reset steal");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.GameObject().CompareTag("P1") && !other.GameObject().CompareTag("P2")) return;
        if (other.GameObject().GetComponent<PlayerController>().isGhost) return;

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
        if (!other.GameObject().CompareTag("P1") && !other.GameObject().CompareTag("P2")) return;
        if (other.GameObject().GetComponent<PlayerController>().isGhost) return;


        if (other.GameObject() == Stealer && nextStealers != null)
        {
            Stealer = null;
            ResetSteal();
            Stealer = nextStealers[0];
            nextStealers.Remove(nextStealers[0]);
        }
        
        else if (other.GameObject() == Stealer && nextStealers == null)
        {
            Stealer = null;
            activate = false;

        }
        
        else if (other.GameObject() != Stealer && nextStealers != null)
        {
            nextStealers.Remove(other.GameObject());
        }
    }
}
