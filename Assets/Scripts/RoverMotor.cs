using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A motor on the rover. The manner in which this motor interacts with Unity's
/// physics must be implemented in subclasses by reading Mode, TargetPower,
/// TargetPosition, and TargetVelocity. Subclasses are responsible setting
/// CurrentPower, CurrentPosition, and CurrentVelocity. Positions and
/// velocities can only be accessed on motors with encoders.
/// </summary>
public abstract class RoverMotor : MonoBehaviour
{
    /// <summary>
    /// Determines the behavior of a motor.
    /// </summary>
    public enum RunMode
    {
        /// <summary>
        /// The motor will run with its target power.
        /// </summary>
        RunWithPower,
        /// <summary>
        /// The motor will try to reach its target position.
        /// </summary>
        RunToPosition,
        /// <summary>
        /// The motor will try to run with its target velocity.
        /// </summary>
        RunWithVelocity
    }

    [SerializeField]
    private string _motorName;
    [SerializeField]
    private bool _hasEncoder;
    [SerializeField]
    private float _statusReportPeriod;

    private RunMode _mode;
    private float _targetPower;
    private float _currentPower;
    private float _targetPosition;
    private float _currentPosition;
    private float _targetVelocity;
    private float _currentVelocity;
    private RoverSocket _socket;

    /// <summary>
    /// The name that identifies this motor.
    /// </summary>
    public string MotorName
    {
        get { return _motorName; }
    }

    /// <summary>
    /// Whether this motor can read its position.
    /// </summary>
    public bool HasEncoder
    {
        get { return _hasEncoder; }
    }

    /// <summary>
    /// The manner in which this motor behaves.
    /// </summary>
    public RunMode Mode
    {
        get { return _mode; }
        set
        {
            if (value == RunMode.RunToPosition || value == RunMode.RunWithVelocity)
                EnsureEncoder();
            _mode = value;
        }
    }

    /// <summary>
    /// The power in [-1, 1] that this motor will try to run with when its mode
    /// is RunWithPower.
    /// </summary>
    public float TargetPower
    {
        get { return _targetPower; }
        set
        {
            if (Math.Abs(value) > 1)
                throw new ArgumentOutOfRangeException(nameof(value), "|value| > 1");
            _targetPower = value;
        }
    }

    /// <summary>
    /// Current power in the range [-1, 1] of this motor.
    /// </summary>
    public float CurrentPower
    {
        get { return _currentPower; }
        protected set
        {
            if (Math.Abs(value) > 1)
                throw new ArgumentOutOfRangeException(nameof(value), "|value| > 1");
            _currentPower = value;
        }
    }

    /// <summary>
    /// The position in degrees that this motor will try to run to when its
    /// mode is RunToPosition. Only available if this motor has an encoder.
    /// </summary>
    public float TargetPosition
    {
        get
        {
            EnsureEncoder();
            return _targetPosition;
        }
        set
        {
            EnsureEncoder();
            _targetPosition = value;
        }
    }

    /// <summary>
    /// Current position in degrees of this motor. Only available if this motor
    /// has an encoder.
    /// </summary>
    public float CurrentPosition
    {
        get
        {
            EnsureEncoder();
            return _currentPosition;
        }
        protected set
        {
            EnsureEncoder();
            _currentPosition = value;
        }
    }

    /// <summary>
    /// The velocity in degrees per second that this motor will try to run with
    /// when its mode is RunWithVelocity. Only available if this motor has an
    /// encoder.
    /// </summary>
    public float TargetVelocity
    {
        get
        {
            EnsureEncoder();
            return _targetVelocity;
        }
        set
        {
            EnsureEncoder();
            _targetVelocity = value;
        }
    }

    /// <summary>
    /// Current velocity of this motor in degrees per second. Only available if
    /// this motor has an encoder.
    /// </summary>
    public float CurrentVelocity
    {
        get
        {
            EnsureEncoder();
            return _currentVelocity;
        }
        protected set
        {
            EnsureEncoder();
            _currentVelocity = value;
        }
    }

    protected virtual void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(SendStatusReports());
    }

    protected virtual void Start()
    {
        Mode = RunMode.RunWithPower;
        TargetPower = 0;
        CurrentPower = 0;
        if (HasEncoder)
        {
            TargetPosition = 0;
            CurrentPosition = 0;
            TargetVelocity = 0;
            CurrentVelocity = 0;
        }
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// Throws an exception if this motor does not have an encoder.
    /// </summary>
    private void EnsureEncoder()
    {
        if (!_hasEncoder)
            throw new InvalidOperationException(MotorName + " has no encoder");
    }

    /// <summary>
    /// Periodically sends status reports to the rover.
    /// </summary>
    private IEnumerator SendStatusReports()
    {
        while (true)
        {
            SendStatusReport();
            yield return new WaitForSeconds(_statusReportPeriod);
        }
    }

    /// <summary>
    /// Sends a status report to the rover.
    /// </summary>
    private void SendStatusReport()
    {
        JObject statusReport = new JObject()
        {
            ["type"] = "simMotorStatusReport",
            ["motor"] = MotorName,
            ["power"] = CurrentPower
        };
        if (HasEncoder)
        {

            statusReport["position"] = CurrentPosition;
            statusReport["velocity"] = CurrentVelocity;
        }
        else
        {
            statusReport["position"] = null;
            statusReport["velocity"] = null;
        }
        _socket.Send(statusReport);
    }
}
