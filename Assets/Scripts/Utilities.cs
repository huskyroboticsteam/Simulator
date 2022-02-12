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
    /// Returns a pseudorandom, Gaussian distributed value with mean 0 and
    /// standard deviation 1.
    /// </summary>
    public static float GaussianRandom()
    {
        return Mathf.Sqrt(-2f * Mathf.Log(Random.value)) * Mathf.Sin(2f * Mathf.PI * Random.value);
    }
}
