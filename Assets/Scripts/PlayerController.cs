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

    public float speed = 5;

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
    
    private Color cghost = new Color(255, 255, 255, 50);

    public GameObject Ability;
    
    public GameObject Item,debugDe;

    public bool ItemOnce = true;

    private bool SpFlip;

    private void Start()
    {
        SP = GetComponent<SpriteRenderer>();
        RB = GetComponent<Rigidbody>();
        StartRecording();
        //transform.rotation = Quaternion.Euler(90f,0,0);

        if (gameObject.CompareTag("P1"))
        {
            SP.sprite = sp1;
            SP.color = cnorm;
        }

        else if (gameObject.CompareTag("P2"))
        {
            SP.sprite = sp2;
            SP.color = cnorm;
            SP.flipX = true;

        }
    }

    private void Update()
    {
        /*if (debugDe != null)
        {
            debugDe.SetActive(false);
        }

        else if (debugDe != null && debugDe.activeSelf == false)
        {
            debugDe = null;
        }
        */
        
        
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
            SpawnItem();
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
        if (Item != null) Item.SetActive(false);
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
                    Item.GetComponent<ItemController>().isGhost = isGhost;
                    Item.SetActive(true);
                    RB.MovePosition(inputFrame.movement);
                }
                else if (!inputFrame.fire && inputFrame.flip)
                {
                    RB.MovePosition(inputFrame.movement);
                    SP.flipX = true;
                }
                else if (!inputFrame.fire && !inputFrame.flip)
                {
                    RB.MovePosition(inputFrame.movement);
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
        
        if (gameObject.CompareTag("P1") && other.CompareTag("Trap2") && !other.GameObject().GetComponent<ItemController>().isGhost && !isGhost)
        {
            StartCoroutine(TrapGhost());
            debugDe = other.GameObject();

        }
        
        else if (gameObject.CompareTag("P2") && other.CompareTag("Trap1") && !other.GameObject().GetComponent<ItemController>().isGhost  && !isGhost)
        {
            StartCoroutine(TrapGhost());
            debugDe = other.GameObject();
        }
    }

    public void TurnGhost()
    {
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
        Item = Instantiate(Ability, spn, Quaternion.identity);
        Item.tag = gameObject.CompareTag("P1") ? "Trap1" : "Trap2";
        Item.GetComponent<ItemController>().isGhost = isGhost;
        Item.GetComponentInChildren<TextMeshPro>().text = gameObject.name[3].ToString();
        ItemOnce = false;
    }

    IEnumerator TrapGhost()
    {
        yield return new WaitForSeconds(0.2f);
        TurnGhost();
        debugDe.SetActive(false);

    }
}
