using UnityEngine;

public class HandMotor : RoverMotor
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

    private float _angle;

    private void Update()
    {
        UpdatePower();
        if (HasEncoder)
            UpdateEncoder();
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
        _angle += CurrentPower * _speed * Time.fixedDeltaTime;
        _angle = Mathf.Clamp(_angle, _minAngle, _maxAngle);
        _leftFinger.transform.localRotation = Quaternion.AngleAxis(_angle, Vector3.up);
        _rightFinger.transform.localRotation = Quaternion.AngleAxis(-_angle, Vector3.up);
    }

    private void UpdateEncoder()
    {

    }
}
