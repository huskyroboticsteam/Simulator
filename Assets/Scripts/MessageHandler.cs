using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// Contains utilities for handling messages sent from the rover server.
/// </summary>
public static class MessageHandler
{
    public static void Handle(JObject message)
    {
        string type = (string)message["type"];
        switch (type)
        {
            case "simMotorPowerRequest":
                HandleMotorPowerRequest(message);
                break;
            case "simCameraStreamOpenRequest":
                HandleCameraStreamOpenRequest(message);
                break;
            case "simCameraStreamCloseRequest":
                HandleCameraStreamCloseRequest(message);
                break;
        }
    }

    private static void HandleMotorPowerRequest(JObject motorPowerRequest)
    {
        Rover rover = Object.FindObjectOfType<Rover>();
        string motorName = (string)motorPowerRequest["motor"];
        RoverMotor motor = rover.GetMotor(motorName);
        motor.Power = (float)motorPowerRequest["power"];
    }

    private static void HandleCameraStreamOpenRequest(JObject cameraStreamOpenRequest)
    {
        Rover rover = Object.FindObjectOfType<Rover>();
        string cameraName = (string)cameraStreamOpenRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.StreamFps = (float)cameraStreamOpenRequest["fps"];
        camera.StreamWidth = (int)cameraStreamOpenRequest["width"];
        camera.StreamHeight = (int)cameraStreamOpenRequest["height"];
        camera.IsStreaming = true;
    }

    private static void HandleCameraStreamCloseRequest(JObject cameraStreamCloseRequest)
    {
        Rover rover = Object.FindObjectOfType<Rover>();
        string cameraName = (string)cameraStreamCloseRequest["camera"];
        RoverCamera camera = rover.GetCamera(cameraName);
        camera.IsStreaming = false;
    }
}
