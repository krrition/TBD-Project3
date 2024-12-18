using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody RB;

    private bool addMove;

    private Vector3 move;

    public float walkSpeed = 5;
    
    public float sprintSpeed = 10f;

    private float speed = 5;
    
    private bool sprinting;

    private List<InputFrame> recordedInputs = new List<InputFrame>();

    private bool isRecording;

    private bool isReplaying;
    
    private float startTime;

    private Vector3 startPos;

    public bool isGhost;

    private SpriteRenderer SP;

    public Sprite sp1;

    public Sprite sp2;

    private Color cnorm = new Color(255, 255, 255, 255);

    public GameObject Ability;
    
    public GameObject Item;
    
    public ItemController IC;

    public bool ItemOnce = true;

    [NonSerialized] public bool SpFlip;

    public bool armorHit,miteOwn;

    private void Start()
    {
        SP = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody>();
        StartRecording();

        if (gameObject.CompareTag("P1"))
        {
            SP.sprite = sp1;
            SP.color = cnorm;
            SpFlip = false;
        }

        else if (gameObject.CompareTag("P2"))
        {
            SP.sprite = sp2;
            SP.color = cnorm;
            SpFlip = true;
            SP.flipX = true;

        }
    }

    public void OnMove(InputAction.CallbackContext cc)
    {
        if (!isRecording) return;
        
        if (cc.performed)
        {

            addMove = true;

            Vector2 input = cc.ReadValue<Vector2>();


            move = new Vector3(input.x,0,input.y).normalized;

            if (input.x >= 0)
            {
                SpFlip = false;
                SP.flipX = false;
            }
            else
            {
                SpFlip = true;
                SP.flipX = true;
            }

        }

        else if (cc.canceled)
        {
            addMove = false;
            move = Vector3.zero;
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, false, SpFlip);
            recordedInputs.Add(inputFrame);
        }
    }

    public void OnFire(InputAction.CallbackContext cc)
    {
        if (!isRecording) return;

        if (cc.canceled)
        {
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, true, SpFlip);
            recordedInputs.Add(inputFrame);
            if (!Ability.CompareTag("Sprint"))SpawnItem();
            else if (Ability.CompareTag("Sprint") && !sprinting)ActivateSprint();
            else if (Ability.CompareTag("Sprint") && sprinting)DeactivateSprint();
        }
    }

    public void StartRecording()
    {
        recordedInputs.Clear();
        startTime = Time.time;
        startPos = transform.position;
        isRecording = true;
        var inputFrame = new InputFrame(Time.time - startTime, transform.position, false, SpFlip);
        recordedInputs.Add(inputFrame);
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartReplay()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        RB.constraints = RigidbodyConstraints.FreezePositionY;
        var tmp = GetComponentInChildren<TextMeshPro>();
        tmp.text = gameObject.name[3].ToString();
        tmp.color = Color.black;
        tmp.fontSize = 9;
        startTime = Time.time;
        transform.position = startPos;
        ResetGhost();
        isReplaying = true;
        if (Item != null && !Ability.CompareTag("Sprint"))
        {
            if (Item.CompareTag("Turret")) Item.GetComponent<TurretController>().Activate();
            else if (Item.CompareTag("Armor")) Item.GetComponent<ArmorController>().RoundReset();
            else if (Item.CompareTag("Hammer")) Item.GetComponent<HammerController>().RoundReset();
            Item.SetActive(false);
        }
    }

    public void StopReplay()
    {
        isReplaying = false;
    }


    private void FixedUpdate()
    {
        if (addMove)
        {
            RB.linearVelocity = move*speed;
            
        }
        
        if (isRecording)
        {
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, false, SpFlip);
            recordedInputs.Add(inputFrame);
        }

        if (isReplaying) 
        { 
            ReplayInputs(Time.time - startTime);
        }

    }

    private void ReplayInputs(float currentTime)
    {
        foreach (var inputFrame in recordedInputs)
        {
            if (Mathf.Abs(inputFrame.timestamp - currentTime) < 0.02f)
            {
                if (inputFrame.fire && Item != null)
                {
                    RB.MovePosition(inputFrame.movement);
                    if (!Ability.CompareTag("Sprint"))Item.SetActive(true);
                    IC.isGhost = isGhost;
                    RB.MovePosition(inputFrame.movement);
                }
                else if (!inputFrame.fire && inputFrame.flip)
                {
                    RB.MovePosition(inputFrame.movement);
                    SpFlip = true;
                    SP.flipX = true;
                }
                else if (!inputFrame.fire && !inputFrame.flip)
                {
                    RB.MovePosition(inputFrame.movement);
                    SpFlip = false;
                    SP.flipX = false;
                }
            }
        }
    }


    private struct InputFrame
    {
        public float timestamp;
        public Vector3 movement;
        public bool fire;
        public bool flip;

        public InputFrame(float time, Vector3 move, bool fireAction, bool flipx)
        {
            timestamp = time;
            movement = move;
            fire = fireAction;
            flip = flipx;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (isGhost) return;
        
        if (other.CompareTag("Bullet") && !other.GetComponent<BulletController>().isGhost)
        {
            Destroy(other.GameObject());
            TurnGhost();
        }

        if (gameObject.CompareTag("P1") && other.CompareTag("Trap2") && !other.GetComponent<ItemController>().isGhost)
        {
            StartCoroutine(TrapGhost(other.GameObject()));

        }
        
        else if (gameObject.CompareTag("P2") && other.CompareTag("Trap1") && !other.GetComponent<ItemController>().isGhost)
        {
            StartCoroutine(TrapGhost(other.GameObject()));
        }
    }

    public void TurnGhost()
    {
        if (armorHit)
        {
            StartCoroutine(BreakShield());
            return;
        }
        isGhost = true;
        SP.color = gameObject.CompareTag("P1") ? Color.clear/5 + Color.cyan / 2 : Color.clear/5 + Color.green / 2;

    }

    public void ResetGhost()
    {
        isGhost = false;
        SP.color = cnorm;
    }

    private void SpawnItem()
    {
        if (!ItemOnce) return;
        Vector3 spn = transform.position;
        spn = new Vector3(!SpFlip ? spn.x + 0.8f : spn.x - 0.8f, spn.y, spn.z);
        
        if (Ability.CompareTag("Trap1"))
        {
            Item = Instantiate(Ability, spn, Quaternion.identity);
            IC = Item.GetComponent<ItemController>();
            Item.GetComponentInChildren<TextMeshPro>().text = gameObject.name[3].ToString();
            Item.tag = gameObject.CompareTag("P1") ? "Trap1" : "Trap2";
            IC.isGhost = isGhost;
        }
        
        else if (Ability.CompareTag("Turret"))
        {
            Item = Instantiate(Ability, spn, Quaternion.identity);
            Item.GetComponent<TurretController>().DirectionRight(SpFlip);
            IC = Item.GetComponent<ItemController>();
            Item.GetComponentInChildren<TextMeshPro>().text = gameObject.name[3].ToString();
            IC.isGhost = isGhost;
            IC.isP1 = gameObject.CompareTag("P1");
        }
        
        else if (Ability.CompareTag("Armor"))
        {
            Item = Instantiate(Ability, spn, Quaternion.identity);
            IC = Item.GetComponent<ItemController>();
            Item.GetComponentInChildren<TextMeshPro>().text = gameObject.name[3].ToString();
            IC.isGhost = isGhost;
            IC.isP1 = gameObject.CompareTag("P1");
        }
        
        else if (Ability.CompareTag("Hammer"))
        {
            Item = Instantiate(Ability, spn, Quaternion.identity);
            IC = Item.GetComponent<ItemController>();
            Item.GetComponentInChildren<TextMeshPro>().text = gameObject.name[3].ToString();
            IC.isGhost = isGhost;
            IC.isP1 = gameObject.CompareTag("P1");
        }

        ItemOnce = false;
    }

    IEnumerator TrapGhost(GameObject G)
    {
        yield return new WaitForSeconds(0.2f);
        TurnGhost();
        G.SetActive(false);
    }
    
    IEnumerator BreakShield()
    {
        yield return new WaitForSeconds(0.5f);
        armorHit = false;
        var ac = GetComponentInChildren<ArmorController>();
        ac.RoundReset();
        ac.GameObject().SetActive(false);
    }

    private void ActivateSprint()
    {
        sprinting = true;
        speed = sprintSpeed;
    }
    
    private void DeactivateSprint()
    {
        sprinting = false;
        speed = walkSpeed;
    }

}
