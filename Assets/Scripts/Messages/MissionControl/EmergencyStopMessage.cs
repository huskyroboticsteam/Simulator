/// <summary>
/// Message sent from Mission Control to the rover for enabling or disabling
/// emergency stop on the rover.
/// </summary>
public class EmergencyStopMessage
{
    public string type = "emergencyStop";
    public bool stop;
}
