using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A motor on the rover. The manner in which this motor interacts with Unity's
/// physics must be implemented in subclasses by reading Mode, TargetPower,
/// and TargetPosition. Subclasses are responsible setting CurrentPower and
/// CurrentPosition.
/// </summary>
public abstract class Motor : MonoBehaviour
{
    /// <summary>
    /// How many seconds a motor should be powered when it receives a power
    /// request before its power is zeroed to simulate watchdog timers.
    /// </summary>
    private const float PowerDuration = 1f;

    /// <summary>
    /// Determines how a motor will be able to read and report its position.
    /// </summary>
    public enum PositionSensorType
    {
        /// <summary>
        /// A motor cannot read its position.
        /// </summary>
        None,
        /// <summary>
        /// A motor can read its position relative to its start position.
        /// </summary>
        Encoder,
        /// <summary>
        /// A motor can read its position absolutely.
        /// </summary>
        Potentiometer
    }

    /// <summary>
    /// Determines the behavior of a motor's limit switch.
    /// </summary>
    public enum LimitSwitch
    {
        /// <summary>
        /// The motor has no limit switch.
        /// </summary>
        None,
        /// <summary>
        /// The motor should report when it has triggered its limit switch, but
        /// continue moving.
        /// </summary>
        Report,
        /// <summary>
        /// The motor should stop and report when it has triggered its limit
        /// switch.
        /// </summary>
        KillAndReport
    }

    /// <summary>
    /// Describes the position of a limit switch.
    /// </summary>
    public enum Limit
    {
        /// <summary>
        /// Corresponds to a limit switch at the minimum motor position in degrees.
        /// </summary>
        Minimum,
        /// <summary>
        /// Corresponds to a limit switch at the maximum motor position in degrees.
        /// </summary>
        Maximum
    }

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
    private bool _reverse;
    [SerializeField]
    private PositionSensorType _positionSensor;
    [SerializeField]
    private LimitSwitch _minLimitSwitch;
    [SerializeField]
    private float _minLimitPosition;
    [SerializeField]
    private LimitSwitch _maxLimitSwitch;
    [SerializeField]
    private float _maxLimitPosition;
    [SerializeField]
    private float _potentiometerOffset;
    [SerializeField]
    private float _statusReportPeriod;

    private RunMode _mode;
    private float _targetPower;
    private float _currentPower;
    private float _targetPosition;
    private float _currentPosition;
    private float _killPowerTime;
    private RoverSocket _socket;

    /// <summary>
    /// The name that identifies this motor.
    /// </summary>
    public string MotorName
    {
        get { return _motorName; }
    }

    /// <summary>
    /// Whether this motor's direction is reversed.
    /// </summary>
    public bool Reverse
    {
        get { return _reverse; }
    }

    /// <summary>
    /// This motor's position sensor type.
    /// </summary>
    public PositionSensorType PositionSensor
    {
        get { return _positionSensor; }
    }

    /// <summary>
    /// The lower limit switch on this motor.
    /// </summary>
    public LimitSwitch MinLimitSwitch
    {
        get { return _minLimitSwitch; }
    }

    /// <summary>
    /// The position in degrees at which the lower limit switch on this motor
    /// is triggered.
    /// </summary>
    public float MinLimitPosition
    {
        get { return _minLimitPosition; }
    }

    /// <summary>
    /// The upper limit switch on this motor.
    /// </summary>
    public LimitSwitch MaxLimitSwitch
    {
        get { return _maxLimitSwitch; }
    }

    /// <summary>
    /// The position in degrees at which the upper limit switch on this motor
    /// is triggered.
    /// </summary>
    public float MaxLimitPosition
    {
        get { return _maxLimitPosition; }
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
                EnsurePositionSensor();
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
            _killPowerTime = Time.time + PowerDuration;
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
    /// mode is RunToPosition. Only available if this motor has a position
    /// sensor.
    /// </summary>
    public float TargetPosition
    {
        get
        {
            EnsurePositionSensor();
            return _targetPosition;
        }
        set
        {
            EnsurePositionSensor();
            _targetPosition = value;
        }
    }

    /// <summary>
    /// Current position in degrees of this motor.
    /// </summary>
    public float CurrentPosition
    {
        get
        {
            return _currentPosition;
        }
        protected set
        {
            _currentPosition = value;
        }
    }

    /// <summary>
    /// Sends a limit switch report for this motor to the rover server.
    /// </summary>
    protected void ReportLimitSwitchTriggered(Limit limit)
    {
        JObject limitSwitchReport = new JObject()
        {
            ["type"] = "simLimitSwitchReport",
            ["motor"] = MotorName,
            ["limit"] = limit.ToString().ToLower()
        };
        _socket.Send(limitSwitchReport);
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
        TargetPower = 0f;
        CurrentPower = 0f;
        CurrentPosition = 0f;
        if (PositionSensor != PositionSensorType.None)
            TargetPosition = 0f;
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void Update()
    {
        if (Time.time > _killPowerTime && Mode == RunMode.RunWithPower)
            TargetPower = 0f;
    }

    /// <summary>
    /// Throws an exception if this motor does not have a position sensor.
    /// </summary>
    private void EnsurePositionSensor()
    {
        if (PositionSensor == PositionSensorType.None)
            throw new InvalidOperationException(MotorName + " has no position sensor");
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
        if (PositionSensor != PositionSensorType.None)
            // Convert from degrees to millidegrees (remember to invert)
            statusReport["position"] = (int)(-CurrentPosition * 1000f);
        else
            statusReport["position"] = null;
        _socket.Send(statusReport);
    }
}
