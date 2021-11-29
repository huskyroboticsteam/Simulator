using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main component of the rover GameObject.
/// </summary>
public class Rover : MonoBehaviour
{
    private IDictionary<string, RoverMotor> _motors;
    private IDictionary<string, RoverCamera> _cameras;
    private bool _emergencyStopped;
    private SimulatorConsole _console;

    /// <summary>
    /// Whether emergency stop is engaged.
    /// </summary>
    public bool EmergencyStopped
    {
        get { return _emergencyStopped; }
        set
        {
            _emergencyStopped = value;
            if (_emergencyStopped)
            {
                foreach (RoverMotor motor in _motors.Values)
                {
                    motor.Power = 0;
                }
                _console.WriteLine("Emergency stop engaged.");
            }
            else
            {
                _console.WriteLine("Emergency stop disengaged.");
            }
        }
    }

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

    private void Awake()
    {
        _console = FindObjectOfType<SimulatorConsole>();
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

        _emergencyStopped = false;
    }
}
