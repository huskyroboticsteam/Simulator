using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// Connects to the Mission Control client and listens for commands.
/// </summary>
public class Server : MonoBehaviour
{
    /// <summary>
    /// This thread-safe queue stores messages sent by the Mission Control
    /// client. Messages are added to this queue in a WebSocketBehavior thread
    /// so they can later be processed in the Unity thread because Unity API
    /// calls are not thread-safe.
    /// </summary>
    private ConcurrentQueue<string> incomingMessages = new ConcurrentQueue<string>();
    /// <summary>
    /// This thread-safe queue stores messages to be sent to the Mission
    /// Control client. Messages are added to this queue in the Unity thread so
    /// they can later be sent in the WebSocket server thread because sending
    /// messages in the Unity thread reduces performance.
    /// </summary>
    private ConcurrentQueue<ServerMessage> outgoingMessages = new ConcurrentQueue<ServerMessage>();

    private WebSocketServer server;
    private Rover rover;

    private void Awake()
    {
        rover = FindObjectOfType<Rover>();
    }

    private void OnEnable()
    {
        Thread serverThread = new Thread(RunServer);
        serverThread.Start();
    }

    /// <summary>
    /// Start the simulator web socket server.
    /// </summary>
    private void RunServer()
    {
        server = new WebSocketServer("ws://localhost:3001");
        server.AddWebSocketService<MissionControlService>("/mission-control", service => service.Initialize(incomingMessages));
        server.KeepClean = false;
        server.Start();

        while (server.IsListening)
        {
            while (outgoingMessages.TryDequeue(out ServerMessage message))
            {
                server.WebSocketServices[message.PathTo].Sessions.Broadcast(message.Message);
            }
        }
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
        // Process messages enqueued from the WebSocketBehavior thread.
        while (incomingMessages.TryDequeue(out string message))
        {
            ProcessMessage(message);
        }
    }

    /// <summary>
    /// Processes the given JSON message received from Mission Control.
    /// </summary>
    private void ProcessMessage(string message)
    {
        int typeStartIndex = message.IndexOf("type") + 7;
        int typeEndIndex = message.Substring(typeStartIndex).IndexOf("\"");
        string type = message.Substring(typeStartIndex, typeEndIndex);
        dynamic deserializedMessage = JsonUtility.FromJson<dynamic>(message);
        switch (type)
        {
            case "drive":
                DriveMessage driveMessage = JsonUtility.FromJson<DriveMessage>(message);
                rover.SetVelocity(driveMessage.straight, driveMessage.steer);
                break;
            case "emergencyStop":
                EmergencyStopMessage emergencyStopMessage = JsonUtility.FromJson<EmergencyStopMessage>(message);
                rover.setEmergencyStopped(emergencyStopMessage.stop);
                break;
            case "motor":
                MotorPowerMessage motorPowerMessage = JsonUtility.FromJson<MotorPowerMessage>(message);
                rover.SetMotorPower(motorPowerMessage.motor, motorPowerMessage.power);
                break;
            default:
                Debug.LogError("Unknown command: " + message);
                break;
        }
    }

    public void Broadcast(string pathTo, string message)
    {
        outgoingMessages.Enqueue(new ServerMessage(pathTo, message));
    }

    /// <summary>
    /// Provides WebSocket server behavior for processing data sent from
    /// Mission Control.
    /// </summary>
    private class MissionControlService : WebSocketBehavior
    {
        private ConcurrentQueue<string> incomingCommands;

        public void Initialize(ConcurrentQueue<string> incomingCommandsRef)
        {
            incomingCommands = incomingCommandsRef;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            incomingCommands.Enqueue(e.Data);
        }
    }

    private class ServerMessage
    {
        public ServerMessage(string pathTo, string message)
        {
            PathTo = pathTo;
            Message = message;
        }

        public string PathTo { get; }
        public string Message { get; }
    }
}
