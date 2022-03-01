using UnityEngine;

/// <summary>
/// Animates a suspension on a rover.
/// </summary>
public class Suspension : MonoBehaviour
{
    [SerializeField]
    private WheelCollider _frontWheel;
    [SerializeField]
    private WheelCollider _rearWheel;

    private void FixedUpdate()
    {
        _frontWheel.GetWorldPose(out Vector3 frontGlobalPos, out _);
        _rearWheel.GetWorldPose(out Vector3 rearGlobalPos, out _);

        Quaternion roverRotInverse = Quaternion.Inverse(transform.parent.rotation);
        Vector3 wheelsOffset = roverRotInverse * (frontGlobalPos - rearGlobalPos);

        float angle = -Mathf.Rad2Deg * Mathf.Atan2(wheelsOffset.y, wheelsOffset.z);
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.right);
    }
}
