using UnityEngine;

/// <summary>
/// A motor on the rover that uses a WheelCollider to interact with Unity's
/// physics engine.
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class WheelRoverMotor : RoverMotor
{
    private WheelCollider _wheel;

    private void Awake()
    {
        _wheel = GetComponent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        _wheel.motorTorque = Power * Strength;
    }
}
