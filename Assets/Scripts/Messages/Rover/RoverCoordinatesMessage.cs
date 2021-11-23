/// <summary>
/// Message sent from the rover to Mission Control for reporting the Rover's
/// coordinates.
/// </summary>
public class RoverCoordinatesMessage
{
    public string type = "roverCoordinates";
    public float latitude;
    public float longitude;
}
