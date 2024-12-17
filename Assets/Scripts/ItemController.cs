using System;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public bool isGhost;
    private bool doOnce;
    private SpriteRenderer SP;
    public bool isP1;

    private void Start()
    {
        SP = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isGhost && !doOnce)
        {
            if (gameObject.name == "Trap(Clone)")SP.color = gameObject.CompareTag("Trap1") ? Color.clear/5 + Color.cyan / 2 : Color.clear/5 + Color.green / 2;
            if (gameObject.name == "Turret(Clone)")SP.color = isP1 ? Color.clear/5 + Color.cyan / 2 : Color.clear/5 + Color.green / 2;
            doOnce = true;
        }

        else if (!isGhost && doOnce)
        {
            if (gameObject.name == "Trap(Clone)")SP.color = Color.white;
            if (gameObject.name == "Turret(Clone)")SP.color = Color.white;
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
    

