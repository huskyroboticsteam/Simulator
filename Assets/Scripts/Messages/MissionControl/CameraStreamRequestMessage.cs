/// <summary>
/// Message sent from Mission Control to the rover for requesting that camera
/// stream data be sent from the rover to Mission Control.
/// </summary>
public class CameraStreamRequestMessage
{
    public string type = "cameraStreamRequest";
    public string cameraName;
    public bool stream;
    public float fps;
    public int width;
    public int height;
}
