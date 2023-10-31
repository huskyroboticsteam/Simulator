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
    private double initLat = 47.653749871465955;
    [SerializeField]
    private double initLong = -122.30429294489063;

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
        // Assuming rover initializes facing north, i.e. z+ (forward) is north & x+ (right) is east
        double[] GPS = metersToGPS(new double[] {
            transform.position.z + _noise * Utilities.GaussianRandom(),
            transform.position.x + _noise * Utilities.GaussianRandom()});

        JObject positionReport = new JObject()
        {
            ["type"] = "simGpsPositionReport",
            ["latitude"] = GPS[0],
            ["longitude"] = GPS[1]
        };
        _socket.Send(positionReport);
    }

    // Taken from Resurgence/src/gps/gps_util.cpp
    private double[] metersToGPS(double[] offset)
    {
        // +x is +lat, +y is -lon
        double semiMajorAxis = 6378137.0;
        double semiMinorAxis = 6356752.314245;
        double phi = Math.PI * initLat / 180.0;
        // Square Eccentricity
        double eSq = 1 - (Math.Pow(semiMinorAxis, 2)) / (Math.Pow(semiMajorAxis, 2));
        double var = 1 - eSq * Math.Pow(Math.Sin(phi), 2);
        double metersPerDegLon = (Math.PI * semiMajorAxis * phi) / (180.0 * Math.Sqrt(var));
        double metersPerDegLat = (Math.PI * semiMajorAxis * (1 - eSq)) / (180.0 * Math.Pow(var, 1.5));
        double[] gps = new double[2];
        double degDiffLat = offset[0] / metersPerDegLat;
        double degDiffLon = -offset[1] / metersPerDegLon;
        return new double[] {initLat + degDiffLat, initLong + degDiffLon};
    }
}
