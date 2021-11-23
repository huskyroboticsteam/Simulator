/// <summary>
/// Message sent from Mission Control to the rover for setting the rover's
/// drive velocity.
/// </summary>
public class DriveMessage
{
    public string type = "drive";
    public float straight;
    public float steer;
}
