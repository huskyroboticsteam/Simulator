using UnityEngine;

/// <summary>
/// A motor on the rover that uses a WheelCollider to interact with Unity's
/// physics engine.
/// </summary>
[RequireComponent(typeof(WheelCollider))]
public class WheelMotor : Motor
{
    [SerializeField]
    private float _torqueMultiplier;
    [SerializeField]
    private GameObject _display;

    private WheelCollider _wheel;

    [SerializeField]
    private float brakeTorque;

    protected override void Awake()
    {
        base.Awake();
        _wheel = GetComponent<WheelCollider>();
    }

    private void FixedUpdate()
    {
        UpdatePower();
        UpdatePosition();
        Render();
        if (Mathf.Abs(CurrentPower) <= 0.05) {
            _wheel.brakeTorque = brakeTorque;
        } else {
            _wheel.brakeTorque = 0f;
        }
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

    private void UpdatePosition()
    {
        CurrentPosition += _wheel.rpm * 360f / 60f * Time.deltaTime;
    }

    private void Render()
    {
        _wheel.GetWorldPose(out Vector3 pos, out Quaternion rot);
        _display.transform.position = pos;
        _display.transform.rotation = rot;
    }
}
