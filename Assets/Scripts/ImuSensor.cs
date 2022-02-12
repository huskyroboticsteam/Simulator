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
    /// <summary>
    /// Standard deviation of Gaussian noise applied to each measured euler
    /// angle in degrees.
    /// </summary>
    [SerializeField]
    private float _noise;

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
        Quaternion rotation = transform.rotation;

        // Apply noise to each euler angle.
        Vector3 eulers = rotation.eulerAngles;
        eulers.x += _noise * Utilities.GaussianRandom();
        eulers.y += _noise * Utilities.GaussianRandom();
        eulers.z += _noise * Utilities.GaussianRandom();
        rotation = Quaternion.Euler(eulers);

        // Convert to rover coordinate system.
        rotation = Utilities.ConvertUnityToRover(rotation);

        JObject orientationReport = new JObject()
        {
            ["type"] = "simImuOrientationReport",
            ["x"] = rotation.x,
            ["y"] = rotation.y,
            ["z"] = rotation.z,
            ["w"] = rotation.w
        };
        _socket.Send(orientationReport);
    }
}
