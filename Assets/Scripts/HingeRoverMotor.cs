using UnityEngine;

/// <summary>
/// A motor on the rover that uses a HingeJoint to interact with Unity's
/// physics engine.
/// </summary>
[RequireComponent(typeof(HingeJoint))]
public class HingeRoverMotor : RoverMotor
{
    private HingeJoint _joint;

    private void Awake()
    {
        _joint = GetComponent<HingeJoint>();
    }

    private void FixedUpdate()
    {
        // TODO
    }
}
