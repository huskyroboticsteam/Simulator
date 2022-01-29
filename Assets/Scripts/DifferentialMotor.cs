using System;
using UnityEngine;

public class DifferentialMotor : Motor
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    public Differential _differential;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private float _maxAngle;

    private void Update()
    {
        if (!HasEncoder)
            throw new Exception("Differential motors need encoders");
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
        float newPosition = CurrentPosition + _speed * CurrentPower * Time.fixedDeltaTime;
        newPosition = Mathf.Clamp(newPosition, _minAngle, _maxAngle);
        if (HasEncoder)
        {
            CurrentVelocity = (newPosition - CurrentPosition) / Time.fixedDeltaTime;
            CurrentPosition = newPosition;
        }
        CurrentPosition = newPosition;
    }
}
