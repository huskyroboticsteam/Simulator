using System;
using UnityEngine;

public class RoverDifferentialMotor : RoverMotor
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    public RoverDifferential _differential;

    private void Update()
    {
        if (!HasEncoder)
            throw new Exception("Differential motors need encoders");
        UpdatePower();
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
        CurrentPosition += _speed * CurrentPower * Time.deltaTime;
    }
}
