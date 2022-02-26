using UnityEngine;

public class HandMotor : PowerDrivenMotor
{
    [SerializeField]
    private GameObject _leftFinger;
    [SerializeField]
    private GameObject _rightFinger;

    protected override void Render()
    {
        float fingerAngle = CurrentPosition / GearRatio;
        if (Reverse)
            fingerAngle = -fingerAngle;
        _leftFinger.transform.localRotation = Quaternion.AngleAxis(-fingerAngle, Vector3.up);
        _rightFinger.transform.localRotation = Quaternion.AngleAxis(fingerAngle, Vector3.up);
    }
}
