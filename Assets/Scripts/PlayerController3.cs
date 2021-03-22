using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3 : MonoBehaviour {
    public WheelCollider fW;
    public WheelCollider bW;

    public float cM;

    private Rigidbody rB;
    // Use this for initialization
    void Start()
    {
        rB = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
            rB.centerOfMass = new Vector3(0, cM, 0);

        fW.steerAngle = Input.GetAxis("Horizontal")*30;
        bW.motorTorque = Input.GetAxis("Vertical") * 100;
	}
}
