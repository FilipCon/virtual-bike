using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : Movement
{
    public float speedMultiplier = 1f;
	public float angleChangeRange = 180f;

    //To handle animation 
    public PlayerAnimatorScript playerAnimatorScript; 

    private float TargetAngle
    {
        get
        {
            return targetAngle;
        }
        set
        {
            targetAngle = value;
            frontWheel.steerAngle = value;
            if(playerAnimatorScript!=null)
            playerAnimatorScript.UpdateSteeringAngle(value);

        }
    }

    public Transform handleBar;
    public Vector3 centerOfMass = Vector3.down;


    void Start ()
	{   
        MovementStart();
        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

        rb.centerOfMass = centerOfMass;
        IdleMode = true;
    }   

    private void Update()
    {   

    }   

    void FixedUpdate()
    {   



            Stop(breakForce);
            // if(playerAnimatorScript!=null)
            // playerAnimatorScript.UpdateCyclingSpeed(0);
            IdleMode = true;


        SetSteeringAngle();

		if (handleBar != null)
        {handleBar.localRotation = Quaternion.Euler (0, TargetAngle + 90, 90);}

        SetRotationUp();
        ApplyVelocityDrag(velocityDrag);

    }   
    
    protected override void SetSteeringAngle()
    {

    }

    protected override void EnterIdleMode()
    { }
    protected override void ExitIdleMode()
    { }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnDrawGizmos()
    {
    }
}
