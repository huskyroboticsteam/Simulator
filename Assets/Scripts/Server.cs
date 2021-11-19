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
    public static Server instance;
    /// <summary>
    /// This thread-safe queue stores commands made by the Mission Control
    /// client. Commands are added to this queue in a WebSocketBehavior thread
    /// so they can later be processed in the Unity main thread because Unity
    /// API calls are not thread-safe.
    /// </summary>
    private static ConcurrentQueue<string> incomingCommands = new ConcurrentQueue<string>();
    private static ConcurrentQueue<ServerMessage> outgoingMessages = new ConcurrentQueue<ServerMessage>();

    public Rover rover;

    private WebSocketServer server;

    private void OnEnable()
    {
        Thread serverThread = new Thread(RunServer);
        serverThread.Start();
        instance = this;
    }

    /// <summary>
    /// Start the simulator web socket server.
    /// </summary>
    private void RunServer()
    {
        server = new WebSocketServer("ws://localhost:3001");
        server.AddWebSocketService<MissionControlService>("/mission-control");
        server.KeepClean = false;
        server.Start();

        while (server.IsListening)
        {
            if (outgoingMessages.TryDequeue(out ServerMessage message))
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
        instance = null;
        server.Stop();
    }

    private void Update()
    {
        // Process commands enqueued form the WebSocketBehavior thread.
        while (!incomingCommands.IsEmpty)
        {
            string command;
            incomingCommands.TryDequeue(out command);
            ProcessCommand(command);
        }
    }

    /// <summary>
    /// Processes the given JSON command received from Mission Control.
    /// </summary>
    private void ProcessCommand(string command)
    {
        int typeStartIndex = command.IndexOf("type") + 7;
        int typeEndIndex = command.Substring(typeStartIndex).IndexOf("\"");
        string type = command.Substring(typeStartIndex, typeEndIndex);
        switch (type)
        {
            case "drive":
                DriveCommand driveCommand = JsonUtility.FromJson<DriveCommand>(command);
                rover.SetVelocity(driveCommand.forward_backward, driveCommand.left_right);
                break;
            case "estop":
                EStopCommand eStopCommand = JsonUtility.FromJson<EStopCommand>(command);
                rover.EStop(eStopCommand.release);
                break;
            case "motor":
                // Need to convert key name to a valid field name
                command = command.Replace("PWM target", "PWM_target");
                MotorCommand motorCommand = JsonUtility.FromJson<MotorCommand>(command);
                rover.SetMotorPower(motorCommand.motor, motorCommand.PWM_target);
                break;
            default:
                Debug.LogError("Unknown command: " + command);
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

    [Serializable]
    private struct DriveCommand
    {
        public string type;
        public float forward_backward;
        public float left_right;
    }

    [Serializable]
    private struct MotorCommand
    {
        public string type;
        public string motor;
        public string mode;
        public float PWM_target;
    }

    [Serializable]
    private struct EStopCommand
    {
        public string type;
        public bool release;
    }
}
