using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

     void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
        }
    }
    public enum RotationAxis
    {
        MouseXAndY = 0,
        MouseX = 1,
        MouseY = 2
    }

    public RotationAxis axes = RotationAxis.MouseXAndY;

    //sensitivity or speed variables
    private float sensivityHor = 3.0f;
    private float sensitivityVert = 3.0f;

    //variables to control the limitations for vertical rotation
    private float minimumVert = -45.0f;
    private float maximumVert = 45.0f;

    // variable for the vertical angle
    private float _rotationX = 0;

    void Update()
    {
        if (axes == RotationAxis.MouseX)                                                        // to check for horizontal rotation
        {
            // Code for horizontal rotation
            transform.Rotate(0f, Input.GetAxis("Mouse X")* sensivityHor,  0f, Space.World);
        }

        else if (axes == RotationAxis.MouseY)                                                   // to check for vertical rotation 
        {
            // Code for vertical rotation
            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            // variable to keep the same horizontal rotation
            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3 (_rotationX, rotationY, 0);
        }

        else                                                                                   // to check for vertical  and horizontal  rotation 
        {
            // Code for vertical and horizontal rotation or rotation in the z-axis
            transform.Rotate(0f, Input.GetAxis("Mouse X") * sensivityHor, 0f, Space.World);

            _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

            // variable to keep the same horizontal rotation
            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
        }
    }

    public void Deactivate()
    {
        enabled = false;
    }


    public void Activate()
    {
        enabled = false;
    }
}
