using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A lidar sensor on the rover which sends lidar data to the rover server.
/// </summary>
public class RoverLidarSensor : MonoBehaviour
{
    private struct Polar
    {
        public Polar(float r, float theta)
        {
            R = r;
            Theta = theta;
        }

        public float R { get; set; }

        public float Theta { get; set; }
    }

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
    [SerializeField]
    private bool _showGizmos;

    private IList<Polar> _points;
    private RoverSocket _socket;

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        _points = new List<Polar>();
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
            float theta = i / (float)_resolution * 360;
            Vector3 direction = Quaternion.AngleAxis(theta, transform.up) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, _range, ~_roverMask))
            {
                Vector3 cartesian = hit.point - transform.position;
                float r = cartesian.magnitude + Random.Range(-_noiseIntensity, _noiseIntensity);
                _points.Add(new Polar(r, theta));
            }
        }

        JArray jsonPoints = new JArray();
        foreach (Polar point in _points)
        {
            JObject jsonPoint = new JObject()
            {
                ["r"] = point.R,
                ["theta"] = point.Theta
            };
            jsonPoints.Add(jsonPoint);
        }
        JObject lidarReport = new JObject()
        {
            ["type"] = "simLidarReport",
            ["points"] = jsonPoints
        };
        _socket.Send(lidarReport);
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos && _points != null)
        {
            Gizmos.color = Color.green;
            foreach (Polar point in _points)
            {
                Vector3 cartesianPoint = transform.position + Quaternion.AngleAxis(point.Theta, transform.up) * transform.forward * point.R;
                Gizmos.DrawSphere(cartesianPoint, 0.02f);
            }
        }
    }
}
