using System;

/// <summary>
/// Represents a command with a name and execution behavior.
/// </summary>
public class Command
{
    /// <summary>
    /// Constructs a new command with the given name.
    /// </summary>
    public Command(string name, Action<string[]> onExecute)
    {
        Name = name;
        OnExecute = onExecute;
    }

    public string Name { get; }
    private Action<string[]> OnExecute { get; }

    /// <summary>
    /// Invoked by the simulator console when this command is executed.
    /// </summary>
    public void Execute(string[] args)
    {
        OnExecute.Invoke(args);
    }
}
