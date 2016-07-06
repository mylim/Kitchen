﻿using UnityEngine;
using System.Collections;
using Leap;
using System;
using System.Collections.Generic;

public class UnitPlayer : Unit {
    
    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }
    

    // Update is called once per frame
    public override void Update() {

        // rotate only when left mouse button is down
        if (Input.GetMouseButton(0))
        {
            // character rotation
            //Debug.Log("X " + Input.GetAxis("Mouse X"));
            transform.Rotate(0f, Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime, 0f);

            // camera rotation        
            cameraRotX -= Input.GetAxis("Mouse Y");
            //Debug.Log("Y " + Input.GetAxis("Mouse Y"));
            cameraRotX = Mathf.Clamp(cameraRotX, -cameraPitchMax, cameraPitchMax); // limit the angle of camera rotation
            Camera.main.transform.forward = transform.forward; // reset the camera view
            Camera.main.transform.Rotate(cameraRotX, 90f, 0f);
        }

        // movement
        move = new Vector3(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
        move.y -= gravity * Time.deltaTime;
        //Debug.Log("Vertical " + Input.GetAxis("Vertical") + " Horizontal " + Input.GetAxis("Horizontal"));
        //Debug.Log(move.ToString());
        move.Normalize();
        // transform the movement to the character's local orientation
        move = transform.TransformDirection(move);

        base.Update();
	}
}