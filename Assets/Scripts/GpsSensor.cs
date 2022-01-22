using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A simulated GPS sensor on the rover which periodically sends the position
/// of the simulated rover to the rover server. Assumes that the Unity origin
/// maps to Null Island.
/// </summary>
public class GpsSensor : MonoBehaviour
{
    /// <summary>
    /// Earth's radius in meters at the equator.
    /// </summary>
    private const double EarthRadius = 6_378_137.0;

    [SerializeField]
    private float _reportPeriod;

    private RoverSocket _socket;

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        StartCoroutine(BeginReportingPosition());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator BeginReportingPosition()
    {
        while (true)
        {
            ReportPosition();
            yield return new WaitForSeconds(_reportPeriod);
        }
    }

    private void ReportPosition()
    {
        // Use double precision since geographic degrees are very large.
        double latitude = CartesianToGeographic(transform.position.z);
        double longitude = CartesianToGeographic(transform.position.x);

        JObject positionReport = new JObject()
        {
            ["type"] = "simGpsPositionReport",
            ["latitude"] = latitude,
            ["longitude"] = longitude
        };
        _socket.Send(positionReport);
    }

    private double CartesianToGeographic(float meters)
    {
        return Mathf.Rad2Deg * meters / EarthRadius;
    }
}
