﻿using UnityEngine;

/// <summary>
/// A motor whose change in position depeneds primarily on its current power,
/// as opposed to its interaction with the physics engine.
/// </summary>
public class PowerDrivenMotor : Motor
{
    [SerializeField]
    private float _openLoopSpeed;
    [SerializeField]
    private float _closedLoopSpeed;
    [SerializeField]
    private float _gearRatio;

    /// <summary>
    /// How fast this motor can turn in degrees per second.
    /// </summary>
    public float Speed
    {
        get { return _openLoopSpeed; }
    }

    /// <summary>
    /// How many motor revolutions correspond to one axle rotation.
    /// </summary>
    public float GearRatio
    {
        get { return _gearRatio; }
    }

    private void FixedUpdate()
    {
        UpdatePower();
        UpdatePosition();
        Render();
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
                if (Mathf.Abs(remainingDistance) < _closedLoopSpeed * Time.fixedDeltaTime) {
                    newPower = remainingDistance / (_closedLoopSpeed * Time.fixedDeltaTime);
                }
                CurrentPower = newPower;
                break;
            default:
                break;
        }
    }

    private void UpdatePosition()
    {
        float speed = Mode switch
        {
            RunMode.RunToPosition => _closedLoopSpeed,
            _ => _openLoopSpeed,
        };
        float newPosition = CurrentPosition + speed * CurrentPower * Time.fixedDeltaTime;
        if (newPosition <= MinLimitPosition)
        {
            if (MinLimitSwitch == LimitSwitch.KillAndReport)
            {
                newPosition = MinLimitPosition;
                CurrentPower = 0f;
            }
            if (newPosition != CurrentPosition &&
                (MinLimitSwitch == LimitSwitch.Report ||
                MinLimitSwitch == LimitSwitch.KillAndReport))
                ReportLimitSwitchTriggered(Limit.Minimum);
        }
        if (newPosition >= MaxLimitPosition)
        {
            if (MinLimitSwitch == LimitSwitch.KillAndReport)
            {
                newPosition = MaxLimitPosition;
                CurrentPower = 0f;
            }
            if (newPosition != CurrentPosition &&
                (MaxLimitSwitch == LimitSwitch.Report ||
                MaxLimitSwitch == LimitSwitch.KillAndReport))
                ReportLimitSwitchTriggered(Limit.Maximum);
        }
        CurrentPosition = newPosition;
    }

    /// <summary>
    /// Optionally override this method to render this motor's position in
    /// subclasses.
    /// </summary>
    protected virtual void Render() { }
}
