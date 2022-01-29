using UnityEngine;

public class Differential : MonoBehaviour
{
    [SerializeField]
    private DifferentialMotor _leftMotor;
    [SerializeField]
    private DifferentialMotor _rightMotor;

    private void FixedUpdate()
    {
        float leftPos = _leftMotor.CurrentPosition;
        float rightPos = _rightMotor.CurrentPosition;
        float pitch = (rightPos + leftPos) / 2;
        float roll = rightPos - leftPos;
        transform.localRotation = Quaternion.Euler(pitch, 0, roll);
    }
}
