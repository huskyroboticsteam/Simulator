using Newtonsoft.Json.Linq;

/// <summary>
/// Contains utilities for handling messages sent from the rover server.
/// </summary>
public static class MessageHandler
{
    public static void Handle(Rover rover, JObject message)
    {
        string type = (string)message["type"];
        switch (type)
        {
            case "simMotorPowerRequest":
                HandleMotorPowerRequest(rover, message);
                break;
            case "simMotorPositionRequest":
                HandleMotorPositionRequest(rover, message);
                break;
            case "simCameraStreamOpenRequest":
                HandleCameraStreamOpenRequest(rover, message);
                break;
            case "simCameraStreamCloseRequest":
                HandleCameraStreamCloseRequest(rover, message);
                break;
        }
    }

    private static void HandleMotorPowerRequest(Rover rover, JObject motorPowerRequest)
    {
        string motorName = (string)motorPowerRequest["motor"];
        Motor motor = rover.GetMotor(motorName);
        motor.TargetPower = (float)motorPowerRequest["power"];
        motor.Mode = Motor.RunMode.RunWithPower;
    }

    private static void HandleMotorPositionRequest(Rover rover, JObject motorPositionRequest)
    {
        string motorName = (string)motorPositionRequest["motor"];
        Motor motor = rover.GetMotor(motorName);
        // Convert from millidegrees to degrees.
        motor.TargetPosition = (float)motorPositionRequest["position"] * 0.001f;
        motor.Mode = Motor.RunMode.RunToPosition;
    }

    private static void HandleCameraStreamOpenRequest(Rover rover, JObject cameraStreamOpenRequest)
    {
        string cameraName = (string)cameraStreamOpenRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.StreamFps = (float)cameraStreamOpenRequest["fps"];
        camera.StreamWidth = (int)cameraStreamOpenRequest["width"];
        camera.StreamHeight = (int)cameraStreamOpenRequest["height"];
        camera.IsStreaming = true;
        if (cameraStreamOpenRequest.ContainsKey("intrinsicParameters"))
        {
            camera.IntrinsicParameters = ((JArray)cameraStreamOpenRequest["intrinsicParameters"])
                .ToObject<float[]>();
        }
    }

    private static void HandleCameraStreamCloseRequest(Rover rover, JObject cameraStreamCloseRequest)
    {
        string cameraName = (string)cameraStreamCloseRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.IsStreaming = false;
    }
}
