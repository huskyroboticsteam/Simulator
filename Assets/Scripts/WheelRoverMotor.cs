using UnityEngine;

/// <summary>
/// A motor on the rover that uses a WheelCollider to interact with Unity's
/// physics engine.
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class WheelRoverMotor : RoverMotor
{
    [SerializeField]
    private float _torqueMultiplier;
    [SerializeField]
    private GameObject _display;

    private WheelCollider _wheel;

    protected override void Awake()
    {
        base.Awake();
        _wheel = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        UpdatePower();
        if (HasEncoder)
            UpdateEncoder();
        UpdateModel();
    }

    private void FixedUpdate()
    {
        _wheel.motorTorque = CurrentPower * _torqueMultiplier;
    }

    private void UpdatePower()
    {
        switch (Mode)
        {
            case RunMode.RunWithPower:
                CurrentPower = TargetPower;
                break;
            case RunMode.RunToPosition:
                CurrentPower = Mathf.Sign(TargetPosition - CurrentPosition);
                break;
            default:
                break;
        }
    }

    private void UpdateEncoder()
    {
        CurrentVelocity = _wheel.rpm * 360 / 60;
        CurrentPosition += CurrentVelocity * Time.deltaTime;
    }

    private void UpdateModel()
    {
        _wheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
        _display.transform.position = pos;
        _display.transform.rotation = rot;
    }
}
