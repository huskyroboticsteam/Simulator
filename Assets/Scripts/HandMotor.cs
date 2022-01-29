using UnityEngine;

public class HandMotor : Motor
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _minPosition;
    [SerializeField]
    private float _maxPosition;
    [SerializeField]
    private GameObject _leftFinger;
    [SerializeField]
    private GameObject _rightFinger;

    private float _currentPositionInternal;

    private void Update()
    {
        UpdatePower();
    }

    private void FixedUpdate()
    {
        UpdatePosition();
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

    private void UpdatePosition()
    {
        _currentPositionInternal += CurrentPower * _speed * Time.fixedDeltaTime;
        _currentPositionInternal = Mathf.Clamp(_currentPositionInternal, _minPosition, _maxPosition);
        _leftFinger.transform.localRotation = Quaternion.AngleAxis(_currentPositionInternal, Vector3.up);
        _rightFinger.transform.localRotation = Quaternion.AngleAxis(-_currentPositionInternal, Vector3.up);
        if (HasEncoder)
            CurrentPosition = _currentPositionInternal;
        if (HasLimitSwitch)
            AtLimit = _currentPositionInternal == _minPosition || _currentPositionInternal == _maxPosition;
    }
}
