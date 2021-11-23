using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    [SerializeField]
    private ItemSpawn spawnPoint;

    private SimulatorConsole console;
    private Rover rover;
    private List<Command> commands;

    private void Awake()
    {
        console = FindObjectOfType<SimulatorConsole>();
        rover = FindObjectOfType<Rover>();
    }

    private void OnEnable()
    {
        commands = new List<Command>() {
            new Command("setspeed", SetSpeed),
            new Command("spawnitem", SpawnItem),
            new Command("reset", Reset)
        };

        foreach (Command command in commands)
        {
            console.RegisterCommand(command);
        }
    }

    private void OnDisable()
    {
        foreach (Command command in commands)
        {
            console.UnregisterCommand(command);
        }
    }

    private void SetSpeed(string[] args)
    {
        if (args.Length != 2)
        {
            console.WriteLine("Usage: setspeed [linearSpeed] [angularSpeed]");
            return;
        }

        float linearSpeed;
        if (!float.TryParse(args[0], out linearSpeed))
        {
            console.WriteLine("Not a valid float: " + args[0]);
        }

        float angularSpeed;
        if (!float.TryParse(args[1], out angularSpeed))
        {
            console.WriteLine("Not a valid float: " + args[1]);
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
            console.WriteLine("reset takes no arguments");
            return;
        }
        // Note: Unity does not rebake lighting in the editor when loading a
        // scene this way, but this issue doesn't arise in production builds.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
