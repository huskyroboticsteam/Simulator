using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A lidar sensor on the rover which sends lidar data to the rover server.
/// </summary>
public class RoverLidarSensor : MonoBehaviour
{
    public struct LidarPoint
    {
        public LidarPoint(float r, float theta)
        {
            R = r;
            Theta = theta;
        }

        public float R { get; }

        public float Theta { get; }
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

    private RoverSocket _socket;

    public LidarPoint[] Points { get; private set; }

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
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
            Report();
            yield return new WaitForSeconds(_scanPeriod);
        }
    }

    private void Scan()
    {
        Points = new LidarPoint[_resolution];
        for (int i = 0; i < _resolution; i++)
        {
            float theta = i / (float)_resolution * 360;
            Vector3 direction = Quaternion.AngleAxis(theta, transform.up) * transform.forward;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, _range, ~_roverMask))
            {
                Vector3 cartesian = hit.point - transform.position;
                float r = cartesian.magnitude + Random.Range(-_noiseIntensity, _noiseIntensity);
                // Convert from Unity coordinates to our coordinates.
                theta = (360 - theta) % 360;
                LidarPoint point = new LidarPoint(r, theta);
                Points[i] = point;
            }
        }
    }

    private void Report()
    {
        JObject[] jPoints = new JObject[_resolution];
        for (int i = 0; i < _resolution; i++)
        {
            LidarPoint point = Points[i];
            JObject jPoint = new JObject()
            {
                ["r"] = point.R,
                ["theta"] = point.Theta
            };
            jPoints[i] = jPoint;
        }
        JObject lidarReport = new JObject()
        {
            ["type"] = "simLidarReport",
            ["points"] = new JArray(jPoints)
        };
        _socket.Send(lidarReport);
    }

    private void OnDrawGizmos()
    {
        if (_showGizmos && Points != null)
        {
            Gizmos.color = Color.green;
            foreach (LidarPoint point in Points)
            {
                float r = (float)point.R;
                float theta = (float)point.Theta;
                // Convert from our coordinates to Unity coordinates.
                theta = 360 - theta;
                Vector3 cartesianPoint = transform.position + Quaternion.AngleAxis(theta, transform.up) * transform.forward * r;
                Gizmos.DrawSphere(cartesianPoint, 0.02f);
            }
        }
    }
}
