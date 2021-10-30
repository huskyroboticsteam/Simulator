using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Text;

/// <summary>
/// Connects to the Mission Control server and listens for requests.
/// </summary>
public class MissionControlListener : MonoBehaviour
{
    public Rover rover;

    private bool listening;
    private TcpClient client;
    private StreamReader reader;

    private void OnEnable()
    {
        // We must connect on a separate thread so that the simulator can
        // continue running in parallel.
        Thread thread = new Thread(Connect);
        thread.Start();
    }

    private void OnDisable()
    {
        Disconnect();
    }

    /// <summary>
    /// Creates a connection to the mission control server.
    /// </summary>
    private void Connect()
    {
        try
        {
            client = new TcpClient("localhost", 3001);
        }
        catch (Exception)
        {
            Debug.Log("Connection to base station failed. Trying again in 3 seconds.");
            Thread.Sleep(3000);
            Connect();
            return;
        }

        listening = true;
        Debug.Log("Connected to base station");
        reader = new StreamReader(client.GetStream());

        StringBuilder jsonBuilder = new StringBuilder();
        int openBraceCount = 0;
        int firstBraceIndex = -1;

        while (listening)
        {
            char ch = (char)reader.Read();
            if (ch == '{')
            {
                if (firstBraceIndex == -1)
                {
                    firstBraceIndex = jsonBuilder.Length;
                }
                openBraceCount++;
            }
            else if (ch == '}')
            {
                openBraceCount--;
            }

            jsonBuilder.Append(ch);
            if (firstBraceIndex != -1 && openBraceCount == 0)
            {
                // end of request
                string request = jsonBuilder.ToString().Substring(firstBraceIndex);
                ProcessRequest(request);

                jsonBuilder = new StringBuilder();
                firstBraceIndex = -1;
            }
        }

        // Clean up.
        reader.Close();
        client.Close();
    }

    /// <summary>
    /// Processes the given json request.
    /// </summary>
    /// <param name="request">the json request in string format</param>
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

    private void Disconnect()
    {
        Debug.Log("Closing connection to Base Station");
        listening = false;
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
