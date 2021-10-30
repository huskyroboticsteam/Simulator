using System.Collections.Generic;

/// <summary>
/// Static class that provides a way to register and excecute simulator commands.
/// </summary>
public static class CommandManager
{
    private static Dictionary<string, Command> commands =
        new Dictionary<string, Command>();

    /// <summary>
    /// Registers the given command so that it can be executed with
    /// CommandManager.Execute().
    /// </summary>
    public static void Register(Command command)
    {
        commands.Add(command.Name, command);
    }

    /// <summary>
    /// Execute the command with the given name and the given arguments. The
    /// command must have been previously registered with
    /// CommandManager.Register(). Returns true if the command exists, false
    /// otherwise.
    /// </summary>
    public static bool Execute(string commandName, string[] args)
    {
        if (!commands.ContainsKey(commandName))
        {
            return false;
        }
        Command command = commands[commandName];
        command.Execute(args);
        return true;
    }
}
