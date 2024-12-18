using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [NonSerialized] public Transform target;
    private Vector3 offset;
    public float smooth = 0.125f;
    private float Rsmooth;
    private bool doOnce;
    public float zoomOutSize = 2;

    private void Start()
    {
        Rsmooth = smooth;
    }

    private void Update()
    {
        if (target != null && !doOnce)
        {
            offset = transform.position - target.position;
            doOnce = true;
        }
    }


    void FixedUpdate()
    {
        if(target == null) return;
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, Rsmooth);
        transform.position = smoothPos;
        
        
    }

    public void ZoomOut(float a)
    {
        target = null;
        offset = Vector3.zero;
        Rsmooth = smooth;
        var tr = transform.position;
        tr.x = a;
        transform.position = tr;
        GetComponent<Camera>().orthographicSize *= zoomOutSize;
    }
}