using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    [SerializeField]
    private Rover rover;
    [SerializeField]
    private ItemSpawn spawnPoint;

    private List<Command> commands;

    private void OnEnable()
    {
        commands = new List<Command>();

        Command setSpeed = new Command("setspeed", SetSpeed);
        commands.Add(setSpeed);

        Command spawnItem = new Command("spawnitem", SpawnItem);
        commands.Add(spawnItem);

        Command reset = new Command("reset", Reset);
        commands.Add(reset);

        foreach (Command command in commands)
        {
            SimulatorConsole.RegisterCommand(command);
        }
    }

    private void OnDisable()
    {
        foreach (Command command in commands)
        {
            SimulatorConsole.UnregisterCommand(command);
        }
    }

    private void SetSpeed(string[] args)
    {
        if (args.Length != 2)
        {
            SimulatorConsole.WriteLine("Usage: setspeed [linearSpeed] [angularSpeed]");
            return;
        }

        float linearSpeed;
        if (!float.TryParse(args[0], out linearSpeed))
        {
            SimulatorConsole.WriteLine("Not a valid float: " + args[0]);
        }

        float angularSpeed;
        if (!float.TryParse(args[1], out angularSpeed))
        {
            SimulatorConsole.WriteLine("Not a valid float: " + args[1]);
        }

        rover.linearSpeed = linearSpeed;
        rover.angularSpeed = angularSpeed;
    }

    private void SpawnItem(string[] args)
    {
        spawnPoint.DropItem();
    }

    private void Reset(string[] args)
    {
        if (args.Length != 0)
        {
            SimulatorConsole.WriteLine("reset takes no arguments");
            return;
        }
        // Note: Unity does not rebake lighting in the editor when loading a
        // scene this way, but this issue doesn't arise in production builds.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
