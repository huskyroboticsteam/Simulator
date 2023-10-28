using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject that provides an interface for
/// accessing virtual hardware components.
/// </summary>
public class Rover : MonoBehaviour
{
    private IDictionary<string, Motor> _motors;
    private IDictionary<string, RoverCamera> _cameras;

    /// A collection containing all of the motors on this rover.
    /// </summary>
    public ICollection<Motor> Motors
    {
        get { return _motors.Values; }
    }

    /// <summary>
    /// A collection containing all of the cameras on this rover.
    /// </summary>
    public ICollection<RoverCamera> Cameras
    {
        get { return _cameras.Values; }
    }

    /// <summary>
    /// Returns the motor on this rover with the specified name, or null if
    /// none exists.
    /// </summary>
    public Motor GetMotor(string motorName)
    {
        _motors.TryGetValue(motorName, out Motor motor);
        return motor;
    }

    /// <summary>
    /// Returns the camera on this rover with the specified name, nor null if
    /// none exists.
    /// </summary>
    public RoverCamera GetCamera(string cameraName)
    {
        _cameras.TryGetValue(cameraName, out RoverCamera camera);
        return camera;
    }

    private void OnEnable()
    {
        _motors = new Dictionary<string, Motor>();
        foreach (Motor motor in transform.GetComponentsInChildren<Motor>())
        {
            _motors[motor.MotorName] = motor;
        }

        _cameras = new Dictionary<string, RoverCamera>();
        foreach (RoverCamera camera in transform.GetComponentsInChildren<RoverCamera>())
        {
            _cameras[camera.CameraName] = camera;
        }
    }

    public void ResetPos()
    {
        transform.position = new Vector3(0, 19.4f, 0);
        transform.rotation = Quaternion.identity;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
