using UnityEngine;

/// <summary>
/// This script offers a simple and convenient method for controlling the rover without running Mission Control.
/// </summary>
public class RoverController : MonoBehaviour
{
    private Rover rover;

    private void Awake()
    {
        rover = FindObjectOfType<Rover>();
    }

    private void Update()
    {
        float forwardBackward = Input.GetAxis("Vertical");
        float leftRight = -Input.GetAxis("Horizontal");
        rover.SetVelocity(forwardBackward, leftRight);
    }
}
