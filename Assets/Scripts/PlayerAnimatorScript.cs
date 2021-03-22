using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour {
    private Animator playerAnimator;
    public float wheelRotationSpeed = 1;
    public float cyclingSpeedFactor  = 1f;
    public float wheelRotationFactor = 10; 
    public Transform frontWheel;
    public Transform backWheel; 

    //[SerializeField]

    //public float steerAngle;
    



    // Use this for initialization
    void Start () {
        playerAnimator = GetComponent<Animator>();

       

    }
    private void FixedUpdate()
    {
        //UpdateWheel rotation
        
       // frontWheel.Rotate(wheelRotationSpeed * wheelRotationFactor * Time.deltaTime,0, 0);
        backWheel.Rotate(wheelRotationSpeed * wheelRotationFactor*Time.deltaTime, 0, 0);
    }



    public void UpdateSteeringAngle (float angle) {
        float normalizedAngle = (-angle + 45)/90;
        //Debug.Log(normalizedAngle);
        playerAnimator.Play("Steering", 1, normalizedAngle);
	}
    public void UpdateCyclingSpeed(float torque)
    {
        playerAnimator.SetFloat("cyclingSpeed", torque* cyclingSpeedFactor );
    }
    public void UpdateWheelRotationSpeed()
    {



    }


}
