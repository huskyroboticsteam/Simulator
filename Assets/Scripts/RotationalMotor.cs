using UnityEngine;

/// <summary>
/// A motor on the rover that modifies its transform's rotation to turn.
/// </summary>
public class RotationalMotor : Motor
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
                    newPower *= Mathf.Abs(remainingDistance) / Smoothing;
                CurrentPower = newPower;
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
        _currentAngle = newAngle;
        if (HasEncoder)
            CurrentPosition = _currentAngle;
        if (HasLimitSwitch)
            AtLimit = _currentAngle == _minAngle || _currentAngle == _maxAngle;
    }
}