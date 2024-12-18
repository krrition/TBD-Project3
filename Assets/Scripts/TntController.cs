using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class TntController : MonoBehaviour
{
    public float ArmTime, DefuseTime;
    private float ArmTimer, DefuseTimer;
    public bool Arming, Defusing;
    public bool p1W, p2W;
    [SerializeField] private GameObject mite;
    private DynamiteController DC;
    private GameObject defuser;
    


    private void Start()
    {
        ArmTimer = ArmTime;
        DefuseTimer = DefuseTime;
        DC = mite.GetComponent<DynamiteController>();

    }

    private void Update()
    {
        if (Arming)
        {
            if(DC.owner == null)
            {
                QuickReset();
                return;
            }
            Arm();
        }
        else if (Defusing)
        {
            if(defuser.GetComponent<PlayerController>().isGhost)
            {
                QuickReset();
                return;
            }
            Defuse();
        }
    }

    private void Arm()
    {
        if (ArmTimer>0)
        {
            ArmTimer -= Time.deltaTime;
        }
        else
        {
            if (gameObject.CompareTag("Tnt2") && DC.owner.CompareTag("P1"))
            {
                p1W = true;
                p2W = false;
                Armed();
            }
            else if (gameObject.CompareTag("Tnt1") && DC.owner.CompareTag("P2"))
            {
                p2W = true;
                p1W = false;
                Armed();
            }
            
        }
        
    }
    
    private void Defuse()
    {
        if (DefuseTimer>0)
        {
            DefuseTimer -= Time.deltaTime;
        }
        else
        {
            if (gameObject.CompareTag("Tnt2") && defuser.CompareTag("P2"))
            {
                p1W = false;
                Defused();
            }
            else if (gameObject.CompareTag("Tnt1") && defuser.CompareTag("P1"))
            {
                p2W = false;
                Defused();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("Tnt1") && other.CompareTag("P1") && p2W && !other.GetComponent<PlayerController>().isGhost && defuser == null) 
        {
            Defusing = true;
            defuser = other.GameObject();
        }

        else if(gameObject.CompareTag("Tnt2") && other.CompareTag("P2") && p1W && !other.GetComponent<PlayerController>().isGhost && defuser == null)
        {
            Defusing = true;
            defuser = other.GameObject();
        }

        if (gameObject.CompareTag("Tnt1") && other.CompareTag("P2") && other.GameObject() == DC.owner)
        {
            Arming = true;
        }
        
        else if (gameObject.CompareTag("Tnt2") && other.CompareTag("P1") && other.GameObject() == DC.owner)
        {
            Arming = true;
        }

    }
    
    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("Tnt1") && other.CompareTag("P1") && p2W && !other.GetComponent<PlayerController>().isGhost && defuser == null) 
        {
            Defusing = false;
            QuickReset();
        }

        else if(gameObject.CompareTag("Tnt2") && other.CompareTag("P2") && p1W && !other.GetComponent<PlayerController>().isGhost && defuser == null)
        {
            Defusing = false;
            QuickReset();
        }

        if (gameObject.CompareTag("Tnt1") && other.CompareTag("P2") && other.GameObject() == DC.owner)
        {
            Arming = false;
            QuickReset();
        }
        
        else if (gameObject.CompareTag("Tnt2") && other.CompareTag("P1") && other.GameObject() == DC.owner)
        {
            Arming = false;
            QuickReset();
        }

    }

    private void Armed()
    {
        Arming = false;
        Defusing = false;
        ArmTimer = ArmTime;
        DefuseTimer = DefuseTime;
        DC.ArmReset();
        mite.transform.position = transform.position;
        mite.GetComponent<BoxCollider>().enabled = false;
    }
    private void Defused()
    {
        Arming = false;
        Defusing = false;
        ArmTimer = ArmTime;
        DefuseTimer = DefuseTime;
        defuser = null;
        DC.ArmReset();
        mite.transform.position = transform.position;
        mite.GetComponent<BoxCollider>().enabled = true;
    }

    public void RoundReset()
    {
        Arming = false;
        Defusing = false;
        ArmTimer = ArmTime;
        DefuseTimer = DefuseTime;
        p1W = false;
        p2W = false;
        DC.RoundReset();
    }

    private void QuickReset()
    {
        Arming = false;
        Defusing = false;
        defuser = null;
        ArmTimer = ArmTime;
        DefuseTimer = DefuseTime;
    }
}