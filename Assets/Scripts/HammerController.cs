using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HammerController : MonoBehaviour
{
    private ItemController IC;

    [SerializeField] private float upHeight = 5f;
    
    [SerializeField] private float downSpeed = 3f;
    
    [SerializeField] private float activeTime = 1.5f;
    
    private Vector3 height;
    
    private Vector3 startPos;

    private Rigidbody RB;

    public bool going;
    public bool doing;
    

    private void Start()
    {
        startPos = transform.position;
        RB = GetComponent<Rigidbody>();
        IC = GetComponent<ItemController>();
        downSpeed *= 100;
        going = true;
        StartCoroutine(HammerDown());

    }

    private void Update()
    {
        if (!going)
        {
            going = true;
        }

        else if (going && !doing)
        {
            StartCoroutine(HammerDown());
        }

    }

    IEnumerator HammerDown()
    {
        if (going)
        {
            doing = true;
            height = transform.position;
            height.y += upHeight;
            transform.position = height;
            RB.AddForce(Vector3.down*downSpeed);
            yield return new WaitForSeconds(activeTime);
            RB.linearVelocity = Vector3.zero;
            transform.position = startPos;
            going = false;
            doing = false;
            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Trap1") && !other.CompareTag("Trap2") && !other.CompareTag("Bullet") && !other.CompareTag("Turret") &&
            !other.CompareTag("Armor")) return;
        if (IC.isGhost) return;

        if (other.CompareTag("Armor"))
        {
            other.GetComponent<ArmorController>().RoundReset();
        }
            
        Destroy(other.GameObject());

    }

    public void RoundReset()
    {
        RB.linearVelocity = Vector3.zero;
        transform.position = startPos;
        going = false;
        doing = false;
    }
}
