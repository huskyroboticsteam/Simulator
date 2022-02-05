using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A simulated GPS sensor on the rover which periodically sends the position
/// of the simulated rover to the rover server. Applies Gaussian noise with
/// a standard deviation of _noise meters to longitude and latitude. Assumes
/// that the Unity origin maps to Null Island.
/// </summary>
public class GpsSensor : MonoBehaviour
{
    /// <summary>
    /// Earth's radius in meters at the equator.
    /// </summary>
    private const double EarthRadius = 6_378_137.0;

    /// <summary>
    /// Standard deviation of Gaussian noise in meters.
    /// </summary>
    [SerializeField]
    private float _noise;
    [SerializeField]
    private float _reportPeriod;

    private RoverSocket _socket;

    private void Awake()
    {
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        StartCoroutine(StreamPosition());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator StreamPosition()
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
        double latitude = CartesianToGeographic(transform.position.z + _noise * GaussianRandom());
        double longitude = CartesianToGeographic(transform.position.x + _noise * GaussianRandom());

        JObject positionReport = new JObject()
        {
            ["type"] = "simGpsPositionReport",
            ["latitude"] = latitude,
            ["longitude"] = longitude
        };
        _socket.Send(positionReport);
    }

    /// <summary>
    /// Returns a pseudorandom, Gaussian distributed value with mean 0 and
    /// standard deviation 1.
    /// </summary>
    private float GaussianRandom()
    {
        return Mathf.Sqrt(-2f * Mathf.Log(Random.value)) * Mathf.Sin(2f * Mathf.PI * Random.value);
    }

    private double CartesianToGeographic(float meters)
    {
        return Mathf.Rad2Deg * meters / EarthRadius;
    }
}
