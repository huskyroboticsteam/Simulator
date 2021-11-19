using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Rover : MonoBehaviour
{
    public float linearSpeed;
    public float angularSpeed;
    public float armBaseSpeed;
    public float shoulderSpeed;
    public float elbowSpeed;
    public float webcamFPS;
    public int webcamVideoWidth;
    public int webcamVideoHeight;
    public Vector3 centerOfMass;
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    public GameObject armBase;
    public GameObject lowerArm;
    public GameObject upperArm;
    public Camera webcam;

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
        rb.centerOfMass = centerOfMass;
        eStopped = false;
        driveforwardBackward = 0.0f;
        driveLeftRight = 0.0f;
        armBasePower = 0.0f;
        shoulderPower = 0.0f;
        elbowPower = 0.0f;
        StartCoroutine(StreamWebcamVideo());
    }

    private void FixedUpdate()
    {
        // Keep the rover grounded.
        rb.AddForce(transform.up * -5f);

        float leftDriveTorque = -driveforwardBackward * linearSpeed + driveLeftRight * angularSpeed;
        frontLeftWheel.motorTorque = leftDriveTorque;
        rearLeftWheel.motorTorque = leftDriveTorque;

        float rightDriveTorque = -driveforwardBackward * linearSpeed - driveLeftRight * angularSpeed;
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
    public void SetVelocity(float forwardBackward, float leftRight)
    {
        if (eStopped)
        {
            return;
        }
        driveforwardBackward = forwardBackward;
        driveLeftRight = leftRight;
    }

    public void EStop(bool release)
    {
        bool redundant = eStopped != release;
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

    private IEnumerator StreamWebcamVideo()
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
            WebcamMessage message = new WebcamMessage();
            message.type = "webcam";
            message.bytes = Convert.ToBase64String(bytes);

            // Clean up to prevent memory leaks.
            webcam.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
            Destroy(frame);

            Server.instance.Broadcast("/mission-control", JsonUtility.ToJson(message));
            yield return new WaitForSeconds(1f / webcamFPS);
        }
    }

    [Serializable]
    private struct WebcamMessage
    {
        public string type;
        public string bytes;
    }
}
