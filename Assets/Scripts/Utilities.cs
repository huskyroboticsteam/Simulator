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
        return Mathf.Sqrt(-2f * Mathf.Log(Random.value)) * Mathf.Sin(2f * Mathf.PI * Random.value);
    }
}
