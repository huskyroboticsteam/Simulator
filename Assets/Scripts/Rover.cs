using System;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Rover : MonoBehaviour
{
    public float linearSpeed;
    public float rotationSpeed;
    public float armBaseSpeed;
    public float shoulderSpeed;
    public float elbowSpeed;
    public GameObject armBase;
    public GameObject lowerArm;
    public GameObject upperArm;

    private Rigidbody rb;
    private bool eStopped;
    private float driveforwardBackward;
    private float driveLeftRight;
    private float armBasePower;
    private float shoulderPower;
    private float elbowPower;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        eStopped = false;
        driveforwardBackward = 0.0f;
        driveLeftRight = 0.0f;
        armBasePower = 0.0f;
        shoulderPower = 0.0f;
        elbowPower = 0.0f;
    }

    private void FixedUpdate()
    {
        rb.velocity = -transform.forward * driveforwardBackward;
        rb.angularVelocity = new Vector3(0.0f, -driveLeftRight, 0.0f);
        // prevent obstacles from forcing unwanted transformations on rover
        Vector3 pos = transform.position;
        pos.y = 0.0f;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);

        armBase.transform.Rotate(0.0f, armBasePower * armBaseSpeed * Time.fixedDeltaTime, 0.0f);
        lowerArm.transform.Rotate(shoulderPower * shoulderSpeed * Time.fixedDeltaTime, 0.0f, 0.0f);
        upperArm.transform.Rotate(elbowPower * elbowSpeed * Time.fixedDeltaTime, 0.0f, 0.0f);
    }

    /// <summary>
    /// Sets the velocity of the rover.
    /// </summary>
    /// <param name="forwardBackward">The linear velocity [-1.0, 1.0]. Positive values correspond to forward motion. Negative values correspond to backward motion.</param>
    /// <param name="leftRight">The rotational velocity [-1.0, 1.0]. Positive values correspond to counterclockwise rotation. Negative values correspond to clockwise rotation.</param>
    public void SetVelocity(float forwardBackward, float leftRight)
    {
        if (eStopped)
        {
            return;
        }
        this.driveforwardBackward = forwardBackward * linearSpeed;
        this.driveLeftRight = leftRight * rotationSpeed;
    }

    public void EStop(bool release)
    {
        Debug.LogError(release);
        bool redundant = this.eStopped != release;
        if (redundant)
        {
            return;
        }
        eStopped = !release;
        if (eStopped)
        {
            Debug.Log("E-stop engaged");
            driveforwardBackward = 0.0f;
            driveLeftRight = 0.0f;
            armBasePower = 0.0f;
            shoulderPower = 0.0f;
            elbowPower = 0.0f;
        }
        else
        {
            Debug.Log("E-stop disengaged");
        }
    }

    public void SetMotorPower(string motor, float power)
    {
        if (eStopped)
        {
            return;
        }
        switch (motor)
        {
            case "arm_base":
                armBasePower = power;
                break;
            case "shoulder":
                shoulderPower = power;
                break;
            case "elbow":
                elbowPower = power;
                break;
            case "forearm":
                Debug.LogError("Forearm not supported yet");
                break;
            case "diffleft":
                Debug.LogError("Diffleft not supported yet");
                break;
            case "diffright":
                Debug.LogError("Diffright not supported yet");
                break;
            case "hand":
                Debug.LogError("Hand not supported yet");
                break;
            default:
                throw new ArgumentException(motor + " is not a valid motor");
        }
    }
}
