using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A simulated IMU sensor on the rover which sends the orientation of the
/// simulated rover to the rover server as a quaternion in the standard Husky
/// Robotics software coordinates.
/// </summary>
public class ImuSensor : MonoBehaviour
{
    [SerializeField]
    private float _reportPeriod;

    private RoverSocket _socket;

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        StartCoroutine(StreamOrientation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StreamOrientation()
    {
        while (true)
        {
            ReportOrientation();
            yield return new WaitForSeconds(_reportPeriod);
        }
    }

    private void ReportOrientation()
    {
        // Note on quaternions in Unity:
        // Let A be the axis of rotation represented by a quaternion Q.
        // Let theta be the angle of rotation about A represented by Q.
        // Q.x = A.x * sin(theta / 2)
        // Q.y = A.y * sin(theta / 2)
        // Q.z = A.y * sin(theta / 2)
        // Q.w = cos(theta / 2)
        Quaternion rot = transform.rotation;

        // Convert to our coordinate system.
        // Unity -Y -> Rover Z
        // Unity X -> Rover Y
        // Unity -Z -> Rover X
        JObject orientationReport = new JObject()
        {
            ["type"] = "simImuOrientationReport",
            ["x"] = -rot.z,
            ["y"] = rot.x,
            ["z"] = -rot.y,
            ["w"] = rot.w
        };
        _socket.Send(orientationReport);
    }
}
