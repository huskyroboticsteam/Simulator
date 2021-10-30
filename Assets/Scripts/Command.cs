/// <summary>
/// Represents a command with a name and execution behavior.
/// </summary>
public abstract class Command
{
    /// <summary>
    /// Constructs a new command with the given name.
    /// </summary>
    public Command(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    /// <summary>
    /// Invoked when this command is executed by the CommandManager.
    /// </summary>
    public abstract void Execute(string[] args);
}
