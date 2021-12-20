using UnityEngine;

public class RotationalRoverMotor : RoverMotor
{
    [SerializeField]
    private Vector3 axis;
    [SerializeField]
    private float minAngle;
    [SerializeField]
    private float maxAngle;

    private float currentAngle;

    private void FixedUpdate()
    {
        currentAngle += Strength * Power * Time.fixedDeltaTime;
        currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
        transform.localRotation = Quaternion.AngleAxis(currentAngle, axis);
    }
}
