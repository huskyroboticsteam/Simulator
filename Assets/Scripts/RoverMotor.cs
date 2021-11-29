using System;
using UnityEngine;

/// <summary>
/// A motor on the rover. Methods by which this motor interacts with Unity's
/// physics must be implemented in subclasses.
/// </summary>
public abstract class RoverMotor : MonoBehaviour
{
    [SerializeField]
    private string _motorName;
    [SerializeField]
    private float _strength;
    private float _power;

    /// <summary>
    /// The name that identifies this motor.
    /// </summary>
    public string MotorName
    {
        get { return _motorName; }
    }

    /// <summary>
    /// Measure of how powerful this motor is.
    /// </summary>
    public float Strength
    {
        get { return _strength; }
        set { _strength = value; }
    }

    /// <summary>
    /// Current power of this motor in the range [-1, 1].
    /// </summary>
    public float Power
    {
        get { return _power; }
        set
        {
            if (Math.Abs(value) > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "|value| > 1");
            }
            _power = value;
        }
    }

    private void Start()
    {
        _power = 0;
    }
}
