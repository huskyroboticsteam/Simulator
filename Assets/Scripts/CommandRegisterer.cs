using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    [SerializeField]
    private Rover _rover;
    private IList<Command> _commands;

    private void OnEnable()
    {
        _commands = new List<Command>() {
            new Command("reset", Reset),
            new Command("run", RunMotor),
            new Command("swerve", Swerve),
            new Command("waypoint", GetWaypoint),
            new Command("waypoints", ListWaypoints),
            new Command("help", PrintInstructions)
        };

        foreach (Command command in _commands)
        {
            SimulatorConsole.RegisterCommand(command);
        }
    }

    private void OnDisable()
    {
        foreach (Command command in _commands)
        {
            SimulatorConsole.UnregisterCommand(command);
        }
    }

    /// <summary>
    /// Prints all the commands and how to use them.
    /// </summary>
    /// <param name="args"></param>
    private void PrintInstructions(string[] args)
    {
        SimulatorConsole.WriteLine("-----Console Commands-----");
        SimulatorConsole.WriteLine("reset: Reloads the scene");
        SimulatorConsole.WriteLine("waypoints: Lists all waypoints");
        SimulatorConsole.WriteLine("waypoint <name>: Copies GPS coordinates of waypoint");
    }

    /// <summary>
    /// Resets the simulator by reloading the current scene.
    /// </summary>
    private void Reset(string[] args)
    {
        if (args.Length != 0)
        {
            SimulatorConsole.WriteLine("reset takes no arguments");
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RunMotor(string[] args)
    {
        if(args.Length < 1 || args.Length > 3) {
            SimulatorConsole.WriteLine("bad");
            return;
        }
        if(args[1] == "to") {
            if(args.Length != 3) {
                SimulatorConsole.WriteLine("bad");
                return;
            }
            MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPositionRequest",
                ["motor"] = args[0],
                ["position"] = args[2]
            });
            SimulatorConsole.WriteLine("run "+args[0]+" "+args[1]+" "+args[2]);
        } else {
            if(args.Length != 2) {
                SimulatorConsole.WriteLine("bad");
                return;
            }
            MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPowerRequest",
                ["motor"] = args[0],    
                ["power"] = args[1]
            });
            SimulatorConsole.WriteLine("run "+args[0]+" "+args[1]);
        }
    }

    private void Swerve(string[] args)
    {
        if(args.Length != 1)
        {
            SimulatorConsole.WriteLine("bad arguments");
            return;
        }
        else
        {
            switch(args[0]) {
                case "normal":
                    Swerve(0);
                    break;
                case "tip":
                case "turn-in-place":
                    Swerve(45);
                    break;
                case "crab":
                    Swerve(90);
                    break;
                default:
                    if(float.TryParse(args[0], out float angle) && 0 <= angle && angle <= 90) {
                        Swerve(angle);
                    } else {
                        SimulatorConsole.WriteLine("not a valid angle");
                    }
                    return;
            }
        }
    }

    private void Swerve(float angle) {
        MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPositionRequest",
                ["motor"] = "frontLeftSwerve",    
                ["position"] = angle
        });
        MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPositionRequest",
                ["motor"] = "rearLeftSwerve",    
                ["position"] = -angle
        });
        MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPositionRequest",
                ["motor"] = "frontRightSwerve",    
                ["position"] = -angle
        });
        MessageHandler.Handle(_rover, new JObject(){
                ["type"] = "simMotorPositionRequest",
                ["motor"] = "rearRightSwerve",    
                ["position"] = angle
        });
        // _rover.GetMotor("frontLeftWheel").GetComponent<WheelCollider>().steerAngle = angle;
        // _rover.GetMotor("rearLeftWheel").GetComponent<WheelCollider>().steerAngle = -angle;
        // _rover.GetMotor("frontRightWheel").GetComponent<WheelCollider>().steerAngle = -angle;
        // _rover.GetMotor("rearRightWheel").GetComponent<WheelCollider>().steerAngle = angle;
    }

    /// <summary>
    /// Copies the GPS coordinates of the specified waypoint (if it exists) to the clipboard.
    /// </summary>
    private void GetWaypoint(string[] args)
    {
        if(args.Length == 0)
        {
            SimulatorConsole.WriteLine("argument required");
            return;
        }
        else
        {
            GameObject waypoint = GameObject.Find("Waypoints/" + args[0]);
            if(waypoint == null)
            {
                SimulatorConsole.WriteLine("Cannot find that waypoint");
            }
            else
            {
                double[] GPS = Utilities.metersToGPS(new double[] {waypoint.transform.position.z, waypoint.transform.position.x});
                string lat = GPS[0].ToString("0." + new string('#', 8));
                string lon = GPS[1].ToString("0." + new string('#', 8));
                GUIUtility.systemCopyBuffer = lat + ", " + lon;
                SimulatorConsole.WriteLine(waypoint.name + "->(lat:" + lat + ", lon:" + lon + ")");
                SimulatorConsole.WriteLine("Copied " + waypoint.name + " to clipboard");
            }
        }
    }

    /// <summary>
    /// Lists all of the waypoints and their GPS coordinates on the Simulator Console.
    /// </summary>
    private void ListWaypoints(string[] args)
    {
        if (args.Length != 0)
        {
            SimulatorConsole.WriteLine("waypoints takes no arguments");
            return;
        }
        else
        {
            foreach (Transform child in GameObject.Find("Waypoints").transform)
            {
                double[] GPS = Utilities.metersToGPS(new double[] {child.transform.position.z, child.transform.position.x});
                string lat = GPS[0].ToString("0." + new string('#', 8));
                string lon = GPS[1].ToString("0." + new string('#', 8));
                SimulatorConsole.WriteLine(child.name + "->(lat:" + lat + ", lon:" + lon + ")");
            }
        }
    }
}
