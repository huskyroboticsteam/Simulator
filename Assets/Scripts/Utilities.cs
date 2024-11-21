using System;
using UnityEngine;

public static class Utilities
{
    /// <summary>
    /// Converts the given position from Unity coordinates to standard Husky
    /// Robotics rover coordinates.
    /// </summary>
    public static Vector3 ConvertUnityToRover(Vector3 position)
    {
        // Unity Z -> Rover X
        // Unity -X -> Rover Y
        // Unity Y -> Rover Z
        return new Vector3(position.z, -position.x, position.y);
    }

    /// <summary>
    /// Converts the given rotation from Unity coordinates to standard Husky
    /// Robotics rover coordinates.
    /// </summary>
    public static Quaternion ConvertUnityToRover(Quaternion rotation)
    {
        // Note on quaternions in Unity:
        // Let A be the axis of rotation represented by a quaternion Q.
        // Let theta be the angle of rotation about A represented by Q.
        // Q.x = A.x * sin(theta / 2)
        // Q.y = A.y * sin(theta / 2)
        // Q.z = A.y * sin(theta / 2)
        // Q.w = cos(theta / 2)

        // Unity -Z -> Rover X
        // Unity X -> Rover Y
        // Unity -Y -> Rover Z
        return new Quaternion(-rotation.z, rotation.x, -rotation.y, rotation.w);
    }

    /// <summary>
    /// Converts the given offset (in meters) to GPS coordinates,
    /// with respect to the starting position (in GPS coordinates).
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Geodetic_datum#Earth_reference_ellipsoid"/> 
    /// <see cref="https://en.wikipedia.org/wiki/World_Geodetic_System#WGS_84"/> 
    /// <see cref="https://en.wikipedia.org/wiki/Longitude#Length_of_a_degree_of_longitude"/> 
    /// <see cref="https://en.wikipedia.org/wiki/Latitude#Meridian_distance_on_the_ellipsoid"/> 
    /// <param name="offset">Offset from starting position (in meters), [North, East]</param>
    /// <param name="start">Starting coordinates (in GPS coordinates), defaults to [0 lat, 0 lon]</param>
    /// <returns></returns>
    public static double[] metersToGPS(double[] offset, double[] init = null)
    {
        init = init is null ? new double[] {0.0, 0.0} : init;
        // North is +lat and East is +lon

        // The Earth is not a perfect sphere, so we approximate the Earth's surface with an ellipsoid
        // https://en.wikipedia.org/wiki/Geodetic_datum#Earth_reference_ellipsoid

        // Data taken from WGS 84:
        // https://en.wikipedia.org/wiki/World_Geodetic_System#WGS_84
        double semiMajorAxis = 6378137.0;
        double semiMinorAxis = 6356752.314245;

        // Math from
        // https://en.wikipedia.org/wiki/Longitude#Length_of_a_degree_of_longitude
        // https://en.wikipedia.org/wiki/Latitude#Meridian_distance_on_the_ellipsoid
        double phi = Math.PI * init[0] / 180.0;
        // Square Eccentricity
        double eSq = 1 - (Math.Pow(semiMinorAxis, 2)) / (Math.Pow(semiMajorAxis, 2));
        double var = 1 - eSq * Math.Pow(Math.Sin(phi), 2);
        double metersPerDegLon = (Math.PI * semiMajorAxis * Math.Cos(phi)) / (180.0 * Math.Sqrt(var));
        double metersPerDegLat = (Math.PI * semiMajorAxis * (1 - eSq)) / (180.0 * Math.Pow(var, 1.5));
        double degDiffLat = offset[0] / metersPerDegLat;
        double degDiffLon = offset[1] / metersPerDegLon;
        return new double[] {init[0] + degDiffLat, init[1] + degDiffLon};
    }

    /// <summary>
    /// Perform the swing-twist decomposition of a quaternion around a given axis.
    /// This essentially decomposes a rotation into a rotation around the axis (the twist) and
    /// a rotation around a vector perpendicular to that axis (the swing).
    /// </summary>
    /// <see cref="https://stackoverflow.com/a/22401169/6202029"/>
    /// <param name="rotation">The rotation to decompose</param>
    /// <param name="axis">The axis to perform the decomposition around</param>
    /// <param name="swing">The rotation around a vector perpendicular to the given axis</param>
    /// <param name="twist">The rotation around the given axis</param>
    public static void SwingTwistDecomposition(Quaternion rotation, Vector3 axis, out Quaternion swing, out Quaternion twist) {
        axis = axis.normalized;
        Vector3 rotAxis = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 p = Vector3.Project(rotAxis, axis);
        twist = new Quaternion(p.x, p.y, p.z, rotation.w);
        twist.Normalize();
        swing = rotation * Quaternion.Inverse(twist);
    }

    /// <summary>
    /// Returns a pseudorandom, Gaussian distributed value with mean 0 and
    /// standard deviation 1.
    /// </summary>
    public static float GaussianRandom()
    {
        return Mathf.Sqrt(-2f * Mathf.Log(UnityEngine.Random.value)) * Mathf.Sin(2f * Mathf.PI * UnityEngine.Random.value);
    }
}
