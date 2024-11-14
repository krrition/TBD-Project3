using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRecorder : MonoBehaviour
{
    public InputAction moveAction;
    public InputAction fireAction;

    private List<InputFrame> recordedInputs = new List<InputFrame>();
    private bool isRecording = false;
    private bool isReplaying = false;
    private float startTime;

    private void OnEnable()
    {
        // Subscribe to input events
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        fireAction.performed += OnFire;
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        fireAction.performed -= OnFire;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (isRecording)
        {
            var inputFrame = new InputFrame(Time.time - startTime, context.ReadValue<Vector2>(), false);
            recordedInputs.Add(inputFrame);
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if (isRecording)
        {
            var inputFrame = new InputFrame(Time.time - startTime, Vector2.zero, true);
            recordedInputs.Add(inputFrame);
        }
    }

    public void StartRecording()
    {
        recordedInputs.Clear();
        startTime = Time.time;
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;
    }

    public void StartReplay()
    {
        startTime = Time.time;
        isReplaying = true;
    }

    public void StopReplay()
    {
        isReplaying = false;
    }

    private void Update()
    {
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
                // Apply recorded movement and fire actions
                if (inputFrame.fire)
                {
                    //gameObject.FireWeapon();
                }
                else
                {
                    //gameObject.Move(inputFrame.movement);
                }
            }
        }
    }
}

public struct InputFrame
{
    public float timestamp;
    public Vector2 movement;
    public bool fire;

    public InputFrame(float time, Vector2 move, bool fireAction)
    {
        timestamp = time;
        movement = move;
        fire = fireAction;
    }
}
