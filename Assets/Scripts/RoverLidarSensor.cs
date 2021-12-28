using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class RoverLidarSensor : MonoBehaviour
{
    [SerializeField]
    private float _range;
    [SerializeField]
    private int _resolution;
    [SerializeField]
    private float _scanPeriod;
    [SerializeField]
    private float _noiseIntensity;
    [SerializeField]
    private LayerMask _roverMask;

    private IList<Vector3> _points;
    private RoverSocket _socket;

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        _points = new List<Vector3>();
        StartCoroutine(BeginScanning());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator BeginScanning()
    {
        while (true)
        {
            Scan();
            yield return new WaitForSeconds(_scanPeriod);
        }
    }

    private void Scan()
    {
        _points.Clear();
        for (int i = 0; i < _resolution; i++)
        {
            float angle = i * 360f / _resolution;
            Vector3 direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, _range, ~_roverMask))
                _points.Add(ApplyNoise(hit.point - transform.position));
        }

        JArray jsonPoints = new JArray();
        foreach (Vector3 point in _points)
            jsonPoints.Add(new JArray(point.x, point.y, point.z));
        JObject lidarReport = new JObject()
        {
            ["type"] = "simLidarReport",
            ["points"] = jsonPoints
        };
        _socket.Send(lidarReport);
    }

    private Vector3 ApplyNoise(Vector3 point)
    {
        return point + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * _noiseIntensity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (Vector3 point in _points)
        {
            Gizmos.DrawSphere(transform.position + point, 0.05f);
        }
    }
}
