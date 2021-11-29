using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;

/// <summary>
/// Manages connection to the rover WebSocket server.
/// </summary>
public class SimulatorSocket : MonoBehaviour
{
    /// <summary>
    /// This thread-safe queue stores messages sent from the rover server to
    /// the simulator. Messages are added to this queue in the WebSocket thread
    /// so they can later be processed in the Unity thread because Unity API
    /// calls are not thread-safe.
    /// </summary>
    private ConcurrentQueue<JObject> _incomingMessages;
    /// <summary>
    /// This thread-safe queue stores messages to be sent from the simulator to
    /// the rover server. Messages are added to this queue in the Unity thread
    /// so they can later be sent in a separate thread because sending
    /// messages in the Unity thread reduces performance.
    /// </summary>
    private ConcurrentQueue<JObject> _outgoingMessages;
    private WebSocket _socket;
    private bool _shouldReconnect;

    private void OnEnable()
    {
        _incomingMessages = new ConcurrentQueue<JObject>();
        _outgoingMessages = new ConcurrentQueue<JObject>();
        _socket = new WebSocket("ws://localhost:3001/simulator");
        _socket.OnOpen += (sender, e) =>
        {
            IsConnected = true;
            MessageServer();
        };
        _socket.OnMessage += (sender, e) => _incomingMessages.Enqueue(JObject.Parse(e.Data));
        _socket.OnClose += (sender, e) =>
        {
            IsConnected = false;
            if (_shouldReconnect)
            {
                Connect();
            }
        };
        _shouldReconnect = true;
        Connect();
    }

    /// <summary>
    /// Whether the socket is connected to the rover server.
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Attempts to connect to the rover server.
    /// </summary>
    private void Connect()
    {
        // Don't connect on the Unity thread because it causes lag.
        new Thread(() => _socket.Connect()).Start();
    }

    /// <summary>
    /// Begins messaging the rover server.
    /// </summary>
    private void MessageServer()
    {
        new Thread(() =>
        {
            while (_socket.IsAlive)
            {
                while (_outgoingMessages.TryDequeue(out JObject message))
                {
                    _socket.Send(message.ToString());
                }
            }
        }).Start();
    }

    private void OnDisable()
    {
        _shouldReconnect = false;
        _socket.Close();
    }

    private void Update()
    {
        // Process messages enqueued from the WebSocketBehavior thread.
        while (_incomingMessages.TryDequeue(out JObject message))
        {
            MessageHandler.Handle(message);
        }
    }

    /// <summary>
    /// Sends a message from the simulator to the rover server.
    /// </summary>
    public void Send(JObject message)
    {
        _outgoingMessages.Enqueue(message);
    }
}
