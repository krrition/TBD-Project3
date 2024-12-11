using System;
using System.Collections.Generic;
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

    [SerializeField]
    private Material ghostMat;

    private Material normMat;

    public GameObject Ability;
    
    public GameObject Item;

    public bool ItemOnce = true;

    private void Start()
    {
        normMat = gameObject.GetComponent<MeshRenderer>().material;
        RB = gameObject.GetComponent<Rigidbody>();
        StartRecording();
    }

    public void OnMove(InputAction.CallbackContext cc)
    {
        if (!isRecording) return;
        
        if (cc.performed)
        {

            addMove = true;

            Vector2 input = cc.ReadValue<Vector2>();


            move = new Vector3(input.x,0,input.y).normalized;

        }

        else if (cc.canceled)
        {
            addMove = false;
            move = Vector3.zero;
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, false);
            recordedInputs.Add(inputFrame);
        }
    }

    public void OnFire(InputAction.CallbackContext cc)
    {
        if (!isRecording) return;

        if (cc.canceled)
        {
            Debug.Log("Up");
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, true);
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
        var inputFrame = new InputFrame(Time.time - startTime, transform.position, false);
        recordedInputs.Add(inputFrame);
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartReplay()
    {
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
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
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, false);
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
                else if (!inputFrame.fire)
                {
                    RB.MovePosition(inputFrame.movement);
                }
            }
        }
    }


    private struct InputFrame
    {
        public float timestamp;
        public Vector3 movement;
        public bool fire;

        public InputFrame(float time, Vector3 move, bool fireAction)
        {
            timestamp = time;
            movement = move;
            fire = fireAction;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("P1") && other.CompareTag("Trap2") && !other.GameObject().GetComponent<ItemController>().isGhost && !isGhost)
        {
            other.GameObject().SetActive(false);
            TurnGhost();

        }
        
        else if (gameObject.CompareTag("P2") && other.CompareTag("Trap1") && !other.GameObject().GetComponent<ItemController>().isGhost  && !isGhost)
        {
            other.GameObject().SetActive(false);
            TurnGhost();
        }
    }

    public void TurnGhost()
    {
        isGhost = true;
        gameObject.GetComponent<MeshRenderer>().material = ghostMat;

    }

    public void ResetGhost()
    {
        isGhost = false;
        gameObject.GetComponent<MeshRenderer>().material = normMat;
    }

    private void SpawnItem()
    {
        if (!ItemOnce) return;
        Item = Instantiate(Ability, transform.position, Quaternion.identity);
        Item.tag = gameObject.CompareTag("P1") ? "Trap1" : "Trap2";
        Item.GetComponent<ItemController>().isGhost = isGhost;
        ItemOnce = false;
    }
}
