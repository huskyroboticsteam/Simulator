using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Registers commands with the simulator console.
/// </summary>
public class CommandRegisterer : MonoBehaviour
{
    [SerializeField]
    private ItemSpawn _spawnPoint;
    private SimulatorConsole _console;
    private IList<Command> _commands;

    private void Awake()
    {
        _console = FindObjectOfType<SimulatorConsole>();
    }

    private void OnEnable()
    {
        _commands = new List<Command>() {
            new Command("spawnitem", SpawnItem),
            new Command("reset", Reset)
        };

        foreach (Command command in _commands)
        {
            _console.RegisterCommand(command);
        }
    }

    private void OnDisable()
    {
        foreach (Command command in _commands)
        {
            _console.UnregisterCommand(command);
        }
    }

    private void SpawnItem(string[] args)
    {
        _spawnPoint.DropItem();
    }

    /// <summary>
    /// Resets the simulator by reloading the current scene.
    /// </summary>
    private void Reset(string[] args)
    {
        if (args.Length != 0)
        {
            _console.WriteLine("reset takes no arguments");
            return;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
