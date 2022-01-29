using System;
using UnityEngine;

public class DifferentialMotor : Motor
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    public Differential _differential;
    [SerializeField]
    private float _minPosition;
    [SerializeField]
    private float _maxPosition;

    private float _currentPositionInternal;

    protected override void Start()
    {
        base.Start();
        _currentPositionInternal = 0;
    }

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
        _currentPositionInternal += _speed * CurrentPower * Time.fixedDeltaTime;
        _currentPositionInternal = Mathf.Clamp(_currentPositionInternal, _minPosition, _maxPosition);
        if (HasEncoder)
            CurrentPosition = _currentPositionInternal;
        if (HasLimitSwitch)
            AtLimit = _currentPositionInternal == _minPosition || CurrentPosition == _maxPosition;
    }
}
