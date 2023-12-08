using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    private IList<Command> _commands;

    private void OnEnable()
    {
        _commands = new List<Command>() {
            new Command("reset", Reset),
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
                GUIUtility.systemCopyBuffer = GPS[0].ToString("0." + new string('#', 339)) + ", "
                                            + GPS[1].ToString("0." + new string('#', 339));
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
                SimulatorConsole.WriteLine(child.name + " (" +
                    GPS[0].ToString("0." + new string('#', 339)) + ", " +
                    GPS[1].ToString("0." + new string('#', 339)) + ")");
            }
        }
    }
}
