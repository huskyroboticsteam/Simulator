using UnityEngine;

public class RotationalRoverMotor : RoverMotor
{
    [SerializeField]
    private Vector3 _axis;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private float _maxAngle;

    private float _currentAngle;

    private void FixedUpdate()
    {
        _currentAngle += Strength * Power * Time.fixedDeltaTime;
        _currentAngle = Mathf.Clamp(_currentAngle, _minAngle, _maxAngle);
        transform.localRotation = Quaternion.AngleAxis(_currentAngle, _axis);
    }
}
