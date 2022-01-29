using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A motor on the rover. The manner in which this motor interacts with Unity's
/// physics must be implemented in subclasses by reading Mode, TargetPower,
/// and TargetPosition. Subclasses are responsible setting CurrentPower,
/// CurrentPosition, and CurrentVelocity. Positions and velocities can only be
/// accessed on motors with encoders.
/// </summary>
public abstract class Motor : MonoBehaviour
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
        RunToPosition
    }

    [SerializeField]
    private string _motorName;
    [SerializeField]
    private bool _hasEncoder;
    [SerializeField]
    private bool _hasLimitSwitch;
    [SerializeField]
    private float _statusReportPeriod;

    private RunMode _mode;
    private float _targetPower;
    private float _currentPower;
    private float _targetPosition;
    private float _currentPosition;
    private bool _atLimit;
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
    /// Whether this motor has a limit switch.
    /// </summary>
    public bool HasLimitSwitch
    {
        get { return _hasLimitSwitch; }
    }

    /// <summary>
    /// The manner in which this motor behaves.
    /// </summary>
    public RunMode Mode
    {
        get { return _mode; }
        set
        {
            if (value == RunMode.RunToPosition)
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
    /// Whether this motor is triggering its limit switch. Only available if
    /// this motor has a limit switch.
    /// </summary>
    public bool AtLimit
    {
        get
        {
            EnsureLimitSwitch();
            return _atLimit;
        }
        protected set
        {
            EnsureLimitSwitch();
            if (_atLimit == value)
                return;
            _atLimit = value;
            if (_atLimit)
            {
                JObject limitSwitchReport = new JObject()
                {
                    ["type"] = "simMotorLimitSwitchReport",
                    ["motor"] = MotorName
                };
                _socket.Send(limitSwitchReport);
            }
            // In the future, we may also want to send a message when the motor
            // leaves its limit position. Hardware currently does not support
            // this, however.
        }
    }

    protected virtual void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(StreamStatus());
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
    /// Throws an exception if this motor does not have a limit switch.
    /// </summary>
    private void EnsureLimitSwitch()
    {
        if (!_hasLimitSwitch)
            throw new InvalidOperationException(MotorName + " has no limit switch");
    }

    /// <summary>
    /// Periodically sends status reports to the rover.
    /// </summary>
    private IEnumerator StreamStatus()
    {
        while (true)
        {
            ReportStatus();
            yield return new WaitForSeconds(_statusReportPeriod);
        }
    }

    /// <summary>
    /// Sends a status report to the rover.
    /// </summary>
    private void ReportStatus()
    {
        JObject statusReport = new JObject()
        {
            ["type"] = "simMotorStatusReport",
            ["motor"] = MotorName,
            ["power"] = CurrentPower
        };
        if (HasEncoder)
            statusReport["position"] = CurrentPosition;
        else
            statusReport["position"] = null;
        _socket.Send(statusReport);
    }
}
