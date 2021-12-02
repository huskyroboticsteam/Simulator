using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject that provides an interface for
/// accessing virtual hardware components.
/// </summary>
public class Rover : MonoBehaviour
{
    private IDictionary<string, RoverMotor> _motors;
    private IDictionary<string, RoverCamera> _cameras;

    /// <summary>
    /// Returns the motor on this rover with the specified name.
    /// </summary>
    public RoverMotor GetMotor(string motorName)
    {
        if (_motors.TryGetValue(motorName, out RoverMotor motor))
        {
            return motor;
        }
        throw new ArgumentException("No such motor " + motorName);
    }

    /// <summary>
    /// Returns the camera on this rover with the specified name.
    /// </summary>
    public RoverCamera GetCamera(string cameraName)
    {
        if (_cameras.TryGetValue(cameraName, out RoverCamera camera))
        {
            return camera;
        }
        throw new ArgumentException("No such camera " + camera + ".");
    }

    private void OnEnable()
    {
        _motors = new Dictionary<string, RoverMotor>();
        foreach (RoverMotor motor in transform.GetComponentsInChildren<RoverMotor>())
        {
            _motors[motor.MotorName] = motor;
        }

        _cameras = new Dictionary<string, RoverCamera>();
        foreach (RoverCamera camera in transform.GetComponentsInChildren<RoverCamera>())
        {
            _cameras[camera.CameraName] = camera;
        }
    }
}
