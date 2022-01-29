using UnityEngine;

public class HandMotor : Motor
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private GameObject _leftFinger;
    [SerializeField]
    private GameObject _rightFinger;

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
                CurrentPower = newPower;
                break;
            default:
                break;
        }
    }

    private void UpdateAngle()
    {
        float newAngle = _currentAngle + CurrentPower * _speed * Time.fixedDeltaTime;
        newAngle = Mathf.Clamp(newAngle, _minAngle, _maxAngle);
        _leftFinger.transform.localRotation = Quaternion.AngleAxis(newAngle, Vector3.up);
        _rightFinger.transform.localRotation = Quaternion.AngleAxis(-newAngle, Vector3.up);
        if (HasEncoder)
        {
            CurrentPosition = _currentAngle;
            CurrentVelocity = (newAngle - _currentAngle) / Time.fixedDeltaTime;
        }
        _currentAngle = newAngle;
    }
}
