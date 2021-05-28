using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnvironementObserver))]
public abstract class Movement : MonoBehaviour
{
    protected Rigidbody rb;
    public float breakForce = 10000;
    public WheelCollider backWheel;
    public WheelCollider frontWheel;

    public bool isGrounded = true;
    protected float targetAngle = 0;

    protected int closestNode = 0;
    public int ClosestNode
    {
        get
        {
            return closestNode;
        }
    }
    public float velocityDrag = 1f;

    [SerializeField]
    protected bool idleMode = true;
    public bool IdleMode
    {
        get
        {
            return idleMode;
        }
        set
        {
            idleMode = value;
            if (idleMode)
            {
                EnterIdleMode();
            }
            else
            {
                ExitIdleMode();
            }
        }

    }

    protected void MovementStart()
    {
        rb = GetComponent<Rigidbody>();

        if (backWheel == null || frontWheel == null)
        {
            Debug.LogError("Wheel not attached to " + gameObject.name);
        }

        backWheel.ConfigureVehicleSubsteps(1, 12, 15);
        frontWheel.ConfigureVehicleSubsteps(1, 12, 15);

    }

    public void ApplyVelocityDrag(float drag)
    {
        rb.AddForce(-drag * rb.velocity.normalized * Mathf.Abs(Vector3.SqrMagnitude(rb.velocity)));
    }

    protected void SetRotationUp()
    {
        Vector3 normal = Vector3.zero;
        GetNormal(out normal);

        if (isGrounded)
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, normal), 50f * Time.deltaTime);
        else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 50f * Time.deltaTime);
    }

    public Vector3 WheelNormal
    {
        get
        {
            Vector3 normal;
            GetNormal(out normal);
            return new Vector3(normal.x, normal.y, normal.z);
        }
    }
    protected void GetNormal(out Vector3 normal)
    {
        WheelHit hit;
        Vector3 vect = Vector3.zero;
        bool isGrounded = this.isGrounded;
        if (backWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }
        else if (frontWheel.GetGroundHit(out hit))
        {
            vect += hit.normal;
            isGrounded = true;
        }
        else
        {

            isGrounded = false;
        }
        normal = vect;
        normal.Normalize();
        this.isGrounded = isGrounded;
    }

    public void Go(float _motorTorque)
    {
        backWheel.brakeTorque = 0;
        frontWheel.brakeTorque = 0;
        backWheel.motorTorque = _motorTorque;
    }
    public void Stop(float _breakForce)
    {
        backWheel.brakeTorque = _breakForce;
        frontWheel.brakeTorque = 0;
        backWheel.motorTorque = 0;
    }

    protected abstract void EnterIdleMode();
    protected abstract void ExitIdleMode();
    protected abstract void SetSteeringAngle();

}
