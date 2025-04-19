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
            default:
                SimulatorConsole.WriteLine("Unknown message type: " + type);
                break;
        }
    }

    private static void HandleMotorPowerRequest(Rover rover, JObject motorPowerRequest)
    {
        string motorName = (string)motorPowerRequest["motor"];
        Motor motor = rover.GetMotor(motorName);
        if (motor == null)
        {
            SimulatorConsole.WriteLine("Unknown motor: " + motorName);
            return;
        }
        motor.TargetPower = (float)motorPowerRequest["power"];
        motor.Mode = Motor.RunMode.RunWithPower;
    }

    private static void HandleMotorPositionRequest(Rover rover, JObject motorPositionRequest)
    {
        string motorName = (string)motorPositionRequest["motor"];
        Motor motor = rover.GetMotor(motorName);
        if (motor == null)
        {
            SimulatorConsole.WriteLine("Unknown motor: " + motorName);
            return;
        }
        // Convert from millidegrees to degrees.
        motor.TargetPosition = -(float)motorPositionRequest["position"] * 0.001f;
        motor.Mode = Motor.RunMode.RunToPosition;
    }

    private static void HandleCameraStreamOpenRequest(Rover rover, JObject cameraStreamOpenRequest)
    {
        int cameraID = (int)cameraStreamOpenRequest["cameraID"];
        RoverCamera camera = rover.GetCamera(cameraID);
        if (camera == null)
        {
            SimulatorConsole.WriteLine("Unknown camera ID: " + cameraID);
            return;
        }
        if (camera.IsStreaming)
        {
            SimulatorConsole.WriteLine(
                "Attempted to stream camera that is already streaming: " + camera.CameraName);
            return;
        }
        camera.StreamFps = (float)cameraStreamOpenRequest["fps"];
        camera.StreamWidth = (int)cameraStreamOpenRequest["width"];
        camera.StreamHeight = (int)cameraStreamOpenRequest["height"];
        camera.IsStreaming = true;
    }

    private static void HandleCameraStreamCloseRequest(Rover rover, JObject cameraStreamCloseRequest)
    {
        int cameraID = (int)cameraStreamCloseRequest["cameraID"];
        RoverCamera camera = rover.GetCamera(cameraID);
        if (camera == null)
        {
            SimulatorConsole.WriteLine("Unknown camera ID: " + cameraID);
            return;
        }
        if (!camera.IsStreaming)
        {
            SimulatorConsole.WriteLine(
                "Attempted to close camera that is already closed: " + camera.CameraName);
            return;
        }
        camera.IsStreaming = false;
    }
}
