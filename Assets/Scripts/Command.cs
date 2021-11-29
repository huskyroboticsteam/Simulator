using System;

/// <summary>
/// Represents a command with a name and execution behavior that can be
/// registered with a SimulatorConsole.
/// </summary>
public class Command
{
    private readonly Action<string[]> _onExecute;

    /// <summary>
    /// Constructs a new command with the given name.
    /// </summary>
    public Command(string name, Action<string[]> onExecute)
    {
        Name = name;
        _onExecute = onExecute;
    }

    /// <summary>
    /// The name of this command. This is what the user types in a
    /// SimulatorConsole to invoke it.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Invoked by the simulator console when this command is executed.
    /// </summary>
    public void Execute(string[] args)
    {
        _onExecute.Invoke(args);
    }
}
