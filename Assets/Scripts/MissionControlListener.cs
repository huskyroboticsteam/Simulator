using System.Collections.Concurrent;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// Connects to the Mission Control client and listens for requests.
/// </summary>
public class MissionControlListener : MonoBehaviour
{
    /// <summary>
    /// This thread-safe queue stores requests made by the Mission Control
    /// client. Requests are added to this queue in a WebSocketBehavior thread
    /// so they can later be processed in the Unity main thread because Unity
    /// API calls are not thread-safe.
    /// </summary>
    private static ConcurrentQueue<string> requests = new ConcurrentQueue<string>();

    public Rover rover;

    private WebSocketServer server;

    private void OnEnable()
    {
        StartServer();
    }

    /// <summary>
    /// Start the simulator web socket server.
    /// </summary>
    private void StartServer()
    {
        server = new WebSocketServer("ws://localhost:3001");
        server.AddWebSocketService<MissionControlService>("/");
        server.KeepClean = false;
        server.Start();
    }

    private void OnDisable()
    {
        StopServer();
    }

    /// <summary>
    /// Stop the simulator web socket server.
    /// </summary>
    private void StopServer()
    {
        server.Stop();
    }

    private void Update()
    {
        // Process requests enqueued form the WebSocketBehavior thread.
        while (!requests.IsEmpty)
        {
            string request;
            requests.TryDequeue(out request);
            ProcessRequest(request);
        }
    }

    /// <summary>
    /// Processes the given json request received from Mission Control.
    /// </summary>
    private void ProcessRequest(string request)
    {
        int typeStartIndex = request.IndexOf("type") + 7;
        int typeEndIndex = request.Substring(typeStartIndex).IndexOf("\"");
        string type = request.Substring(typeStartIndex, typeEndIndex);
        switch (type)
        {
            case "drive":
                DriveRequest driveRequest = JsonUtility.FromJson<DriveRequest>(request);
                rover.SetVelocity(driveRequest.forward_backward, driveRequest.left_right);
                break;
            case "estop":
                EStopRequest eStopRequest = JsonUtility.FromJson<EStopRequest>(request);
                rover.EStop(eStopRequest.release);
                break;
            case "motor":
                // Need to convert key name to a valid field name
                request = request.Replace("PWM target", "PWM_target");
                MotorRequest motorRequest = JsonUtility.FromJson<MotorRequest>(request);
                rover.SetMotorPower(motorRequest.motor, motorRequest.PWM_target);
                break;
            default:
                Debug.LogError("Unknown request type: " + request);
                break;
        }
    }

    /// <summary>
    /// Provides websocket server behavior for processing data sent from
    /// Mission Control.
    /// </summary>
    private class MissionControlService : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            requests.Enqueue(e.Data);
        }
    }

    [System.Serializable]
    private struct DriveRequest
    {
        public string type;
        public float forward_backward;
        public float left_right;
    }

    [System.Serializable]
    private struct MotorRequest
    {
        public string type;
        public string motor;
        public string mode;
        public float PWM_target;
    }

    [System.Serializable]
    private struct EStopRequest
    {
        public string type;
        public bool release;
    }
}
