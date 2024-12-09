using System;
using System.Collections.Generic;
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
            var inputFrame = new InputFrame(Time.time - startTime, transform.position, true);
            recordedInputs.Add(inputFrame);
            Item = Instantiate(Ability, transform.position, Quaternion.identity);
            Item.GetComponent<ItemController>().isGhost = isGhost;
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
        startTime = Time.time;
        transform.position = startPos;
        ResetGhost();
        isReplaying = true;
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


    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRecording)
            {
                StopRecording();
            }
            
            else if (!isRecording)
            {
                StartReplay();
            }
            
            else if (isReplaying)
            {
                StopReplay();
            }
        }*/



    }
    
    private void ReplayInputs(float currentTime)
    {
        foreach (var inputFrame in recordedInputs)
        {
            if (Mathf.Abs(inputFrame.timestamp - currentTime) < 0.02f)
            {
                if (inputFrame.fire)
                {
                    RB.MovePosition(inputFrame.movement);
                    Item = Instantiate(Ability, transform.position, Quaternion.identity);
                    Item.GetComponent<ItemController>().isGhost = isGhost;
                    RB.MovePosition(inputFrame.movement);
                }
                else
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
        if (other.CompareTag("Trap") && !isGhost)
        {
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
}
