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
            case "simMotorVelocityRequest":
                HandleMotorVelocityRequest(rover, message);
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
        RoverMotor motor = rover.GetMotor(motorName);
        motor.TargetPower = (float)motorPowerRequest["power"];
        motor.Mode = RoverMotor.RunMode.RunWithPower;
    }

    private static void HandleMotorPositionRequest(Rover rover, JObject motorPositionRequest)
    {
        string motorName = (string)motorPositionRequest["motor"];
        RoverMotor motor = rover.GetMotor(motorName);
        motor.TargetPosition = (float)motorPositionRequest["position"];
        motor.Mode = RoverMotor.RunMode.RunToPosition;
    }

    private static void HandleMotorVelocityRequest(Rover rover, JObject motorVelocityRequest)
    {
        string motorName = (string)motorVelocityRequest["motor"];
        RoverMotor motor = rover.GetMotor(motorName);
        motor.TargetVelocity = (float)motorVelocityRequest["velocity"];
        motor.Mode = RoverMotor.RunMode.RunWithVelocity;
    }

    private static void HandleCameraStreamOpenRequest(Rover rover, JObject cameraStreamOpenRequest)
    {
        string cameraName = (string)cameraStreamOpenRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.StreamFps = (float)cameraStreamOpenRequest["fps"];
        camera.StreamWidth = (int)cameraStreamOpenRequest["width"];
        camera.StreamHeight = (int)cameraStreamOpenRequest["height"];
        camera.IsStreaming = true;
    }

    private static void HandleCameraStreamCloseRequest(Rover rover, JObject cameraStreamCloseRequest)
    {
        string cameraName = (string)cameraStreamCloseRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.IsStreaming = false;
    }
}
