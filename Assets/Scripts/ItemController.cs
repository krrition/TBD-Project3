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
            else if (gameObject.CompareTag("Turret") || gameObject.CompareTag("Armor"))SP.color = isP1 ? Color.clear/5 + Color.cyan / 2 : Color.clear/5 + Color.green / 2;
            doOnce = true;
        }

        else if (!isGhost && doOnce)
        {
            SP.color = Color.white;
            doOnce = false;
        }

        
    }

}
    

