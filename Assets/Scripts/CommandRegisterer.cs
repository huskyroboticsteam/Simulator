using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    public Rover rover;
    [SerializeField] ItemSpawn spawnPoint;

    private List<Command> commands;

    private void OnEnable()
    {
        commands = new List<Command>();

        Command setSpeed = new Command("setspeed", SetSpeed);
        commands.Add(setSpeed);

        Command spawnItem = new Command("spawnitem", SpawnItem);
        commands.Add(spawnItem);

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
}
