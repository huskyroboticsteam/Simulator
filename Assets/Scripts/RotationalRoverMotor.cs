using UnityEngine;

/// <summary>
/// A motor on the rover that modifies its transform's rotation to turn.
/// </summary>
public class RotationalRoverMotor : RoverMotor
{
    /// <summary>
    /// Used to smooth motor movement when approaching a target position.
    /// </summary>
    private const float Smoothing = 15;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private Vector3 _axis;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private float _maxAngle;

    private float _currentAngle;

    private void Update()
    {
        UpdatePower();
    }

    private void FixedUpdate()
    {
        UpdateAngle();
    }

    private void UpdatePower()
    {
        switch (Mode)
        {
            case RunMode.RunWithPower:
                CurrentPower = TargetPower;
                break;
            case RunMode.RunToPosition:
                float remainingDistance = TargetPosition - CurrentPosition;
                float newPower = Mathf.Sign(remainingDistance);
                // Slow down when near target.
                if (Mathf.Abs(remainingDistance) < Smoothing)
                {
                    newPower *= remainingDistance / Smoothing;
                }
                CurrentPower = newPower;
                break;
            case RunMode.RunWithVelocity:
                CurrentPower = Mathf.Clamp(TargetVelocity / _speed, -1, 1);
                break;
            default:
                break;
        }
    }

    private void UpdateAngle()
    {
        float newAngle = _currentAngle + _speed * CurrentPower * Time.fixedDeltaTime;
        newAngle = Mathf.Clamp(newAngle, _minAngle, _maxAngle);
        transform.localRotation = Quaternion.AngleAxis(newAngle, _axis);
        if (HasEncoder)
        {
            CurrentPosition = _currentAngle;
            CurrentVelocity = (newAngle - _currentAngle) / Time.fixedDeltaTime;
        }
        _currentAngle = newAngle;
    }
}
