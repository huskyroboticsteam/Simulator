using UnityEngine;

public class RoverDifferential : MonoBehaviour
{
    [SerializeField]
    private RoverDifferentialMotor _leftMotor;
    [SerializeField]
    private RoverDifferentialMotor _rightMotor;

    private void FixedUpdate()
    {
        // TODO: THIS DOES NOT WORK
        float leftPos = _leftMotor.CurrentPosition;
        float rightPos = _rightMotor.CurrentPosition;
        float x = leftPos;
        float z = rightPos;
        transform.localRotation = Quaternion.Euler(x, 0, z);
    }
}
