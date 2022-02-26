using UnityEngine;

/// <summary>
/// A motor on the rover that controls a hinge by modifying its transform's
/// rotation when turning.
/// </summary>
public class HingeMotor : PowerDrivenMotor
{
    [SerializeField]
    private Vector3 _axis;

    protected override void Render()
    {
        float currentAngle = CurrentPosition / GearRatio;
        if (Reverse)
            currentAngle = -currentAngle;
        transform.localRotation = Quaternion.AngleAxis(currentAngle, _axis);
    }
}
