using System;
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
    [SerializeField]
    private double initLat = 0.0;
    [SerializeField]
    private double initLon = 0.0;

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
        // z+ is north, x+ is east
        double[] GPS = Utilities.metersToGPS(new double[] {
            transform.position.z + _noise * Utilities.GaussianRandom(),
            transform.position.x + _noise * Utilities.GaussianRandom()},
            new double[] {initLat, initLon});

        JObject positionReport = new JObject()
        {
            ["type"] = "simGpsPositionReport",
            ["latitude"] = GPS[0],
            ["longitude"] = GPS[1]
        };
        _socket.Send(positionReport);
    }
}
