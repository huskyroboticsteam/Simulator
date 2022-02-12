using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// Periodically reports the true pose of the rover.
/// </summary>
public class RoverPoseReporter : MonoBehaviour
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
        StartCoroutine(StreamPose());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StreamPose()
    {
        while (true)
        {
            ReportPose();
            yield return new WaitForSeconds(_reportPeriod);
        }
    }

    private void ReportPose()
    {
        Vector3 position = Utilities.ConvertUnityToRover(transform.position);
        JObject jPosition = new JObject()
        {
            ["x"] = position.x,
            ["y"] = position.y,
            ["z"] = position.z
        };

        Quaternion rotation = Utilities.ConvertUnityToRover(transform.rotation);
        JObject jRotation = new JObject()
        {
            ["x"] = rotation.x,
            ["y"] = rotation.y,
            ["z"] = rotation.z,
            ["w"] = rotation.w
        };

        JObject poseReport = new JObject()
        {
            ["type"] = "simRoverTruePoseReport",
            ["position"] = jPosition,
            ["rotation"] = jRotation
        };

        _socket.Send(poseReport);
    }
}
