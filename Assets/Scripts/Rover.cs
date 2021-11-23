using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Rover : MonoBehaviour
{
    [Header("Motor Speeds")]
    public float linearSpeed;
    public float angularSpeed;
    public float armBaseSpeed;
    public float shoulderSpeed;
    public float elbowSpeed;

    [Header("Physics")]
    public Vector3 centerOfMass;

    [Header("Camera Settings")]
    public float webcamFPS;
    public int webcamVideoWidth;
    public int webcamVideoHeight;

    [Header("Object References")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public GameObject armBase;
    public GameObject lowerArm;
    public GameObject upperArm;
    public Camera webcam;

    private Rigidbody rb;
    private Server server;
    private SimulatorConsole console;
    private bool emergencyStopped;
    private float driveStraight;
    private float driveSteer;
    private float armBasePower;
    private float shoulderPower;
    private float elbowPower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        server = FindObjectOfType<Server>();
        console = FindObjectOfType<SimulatorConsole>();
    }

    private void Start()
    {
        rb.centerOfMass = centerOfMass;
        emergencyStopped = false;
        driveStraight = 0.0f;
        driveSteer = 0.0f;
        armBasePower = 0.0f;
        shoulderPower = 0.0f;
        elbowPower = 0.0f;
        StartCoroutine(StreamVideo());
    }

    private void FixedUpdate()
    {
        // Keep the rover grounded.
        rb.AddForce(transform.up * -5f);

        float leftDriveTorque = -driveStraight * linearSpeed - driveSteer * angularSpeed;
        frontLeftWheel.motorTorque = leftDriveTorque;
        rearLeftWheel.motorTorque = leftDriveTorque;

        float rightDriveTorque = -driveStraight * linearSpeed + driveSteer * angularSpeed;
        frontRightWheel.motorTorque = rightDriveTorque;
        rearRightWheel.motorTorque = rightDriveTorque;

        armBase.transform.Rotate(0.0f, armBasePower * armBaseSpeed * Time.fixedDeltaTime, 0.0f);
        lowerArm.transform.Rotate(shoulderPower * shoulderSpeed * Time.fixedDeltaTime, 0.0f, 0.0f);
        upperArm.transform.Rotate(elbowPower * elbowSpeed * Time.fixedDeltaTime, 0.0f, 0.0f);
    }

    /// <summary>
    /// Sets the velocity of the rover.
    /// </summary>
    /// <param name="forwardBackward">The linear velocity [-1.0, 1.0]. Positive values correspond to forward motion. Negative values correspond to backward motion.</param>
    /// <param name="leftRight">The rotational velocity [-1.0, 1.0]. Positive values correspond to counterclockwise rotation. Negative values correspond to clockwise rotation.</param>
    public void SetVelocity(float straight, float steer)
    {
        if (emergencyStopped)
        {
            return;
        }
        driveStraight = straight;
        driveSteer = steer;
    }

    public void setEmergencyStopped(bool stopped)
    {
        bool redundant = emergencyStopped == stopped;
        if (redundant)
        {
            return;
        }
        emergencyStopped = stopped;
        if (emergencyStopped)
        {
            console.WriteLine("Emergency stop engaged");
            driveStraight = 0.0f;
            driveSteer = 0.0f;
            armBasePower = 0.0f;
            shoulderPower = 0.0f;
            elbowPower = 0.0f;
        }
        else
        {
            console.WriteLine("Emergency stop disengaged");
        }
    }

    public void SetMotorPower(string motor, float power)
    {
        if (emergencyStopped)
        {
            return;
        }
        switch (motor)
        {
            case "armBase":
                armBasePower = power;
                break;
            case "shoulder":
                shoulderPower = power;
                break;
            case "elbow":
                elbowPower = power;
                break;
            default:
                throw new ArgumentException(motor + " is not a valid motor");
        }
    }

    private IEnumerator StreamVideo()
    {
        while (true)
        {
            RenderTexture renderTexture = new RenderTexture(webcamVideoWidth, webcamVideoHeight, 24);
            RenderTexture.active = renderTexture;

            webcam.targetTexture = renderTexture;
            webcam.Render();

            Texture2D frame = new Texture2D(webcamVideoWidth, webcamVideoHeight, TextureFormat.RGB24, false);
            frame.ReadPixels(new Rect(0, 0, webcamVideoWidth, webcamVideoHeight), 0, 0);

            byte[] bytes = frame.EncodeToJPG();
            CameraStreamFrameMessage message = new CameraStreamFrameMessage();
            message.cameraName = "front";
            message.data = Convert.ToBase64String(bytes);

            // Clean up to prevent memory leaks.
            webcam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
            Destroy(frame);

            server.Broadcast("/mission-control", JsonUtility.ToJson(message));
            yield return new WaitForSeconds(1f / webcamFPS);
        }
    }
}
