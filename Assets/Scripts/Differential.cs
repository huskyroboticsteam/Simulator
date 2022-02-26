using UnityEngine;

public class Differential : MonoBehaviour
{
    [SerializeField]
    private PowerDrivenMotor _leftMotor;
    [SerializeField]
    private PowerDrivenMotor _rightMotor;

    private void FixedUpdate()
    {
        float leftPos = _leftMotor.CurrentPosition / _leftMotor.GearRatio;
        if (_leftMotor.Reverse)
            leftPos = -leftPos;

        float rightPos = _rightMotor.CurrentPosition / _rightMotor.GearRatio;
        if (_rightMotor.Reverse)
            rightPos = -rightPos;

        float pitch = (rightPos + leftPos) / 2;
        float roll = leftPos - rightPos;
        transform.localRotation = Quaternion.Euler(pitch, 0, roll);
    }
}
