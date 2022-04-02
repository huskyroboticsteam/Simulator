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
            new Command("reset", Reset)
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
}
