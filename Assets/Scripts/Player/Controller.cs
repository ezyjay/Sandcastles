using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Controller : MonoBehaviour 
{
    public Rigidbody rb;
    public float deccelerationModifier;
    public float speed;
    public float maxSpeed;

    private Vector3 inputDirection;
    private Vector3 previousInputDirection = Vector3.zero;
    private float inputDirectionToStartMoving = 0.4f;

	public Action<bool> PlayerMoving;

    public Vector3 InputDirection { get => inputDirection; set => inputDirection = value; }

    void Update()
    {
        inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void FixedUpdate() {
        
        if (inputDirection != Vector3.zero) { 

			if (Mathf.Abs(inputDirection.z) > 0.2f && Mathf.Abs(inputDirection.x) > 0.2f) {
				if(rb.velocity.z == 0)
					inputDirection = new Vector3(inputDirection.x, 0);
				else if (rb.velocity.x == 0)
					inputDirection = new Vector3(0, 0, inputDirection.z);
			}

            bool startMoving = Mathf.Abs(inputDirection.x) > inputDirectionToStartMoving || Mathf.Abs(inputDirection.z) > inputDirectionToStartMoving; 
            if (rb.velocity.magnitude <= maxSpeed && startMoving) 
                rb.velocity = inputDirection.normalized * speed;

            previousInputDirection = inputDirection;

        } 
        else {
			
            if (rb.velocity.magnitude > 0) {
                if (rb.velocity.magnitude >= 0.4f * maxSpeed)
                    rb.velocity = deccelerationModifier * rb.velocity;
                else
                    rb.velocity = 0.1f * rb.velocity;
            }
        }
    }
}
