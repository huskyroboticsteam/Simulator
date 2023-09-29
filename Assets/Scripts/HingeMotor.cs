using UnityEngine;

/// <summary>
/// A motor on the rover that controls a hinge by modifying its transform's
/// rotation when turning.
/// </summary>
public class HingeMotor : PowerDrivenMotor
{
    [SerializeField]
    private Vector3 _axis;

    protected override void Start()
    {
        base.Start();
        // Initialize CurrentPosition to the initial position of the rover
        Utilities.SwingTwistDecomposition(transform.localRotation, _axis, out _, out Quaternion twist);
        twist.ToAngleAxis(out float angle, out Vector3 ax);
        // account for rotation axis being in opposite direction as _axis
        angle *= Mathf.Sign(Vector3.Dot(ax, _axis));
        CurrentPosition = angle * GearRatio * (Reverse ? -1 : 1);
    }

    protected override void Render()
    {
        float currentAngle = CurrentPosition / GearRatio;
        if (Reverse)
            currentAngle = -currentAngle;
        transform.localRotation = Quaternion.AngleAxis(currentAngle, _axis);
    }
}
