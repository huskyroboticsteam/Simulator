/// <summary>
/// Message sent from the rover to Mission Control for reporting a frame
/// recorded by a camera.
/// </summary>
public class CameraStreamFrameMessage
{
    public string type = "cameraStreamFrame";
    public string cameraName;
    public string data;
}
