using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A console that a user can enter commands into and view output from within
/// the simulator scene.
/// </summary>
public class SimulatorConsole : MonoBehaviour
{
    private const int MaxDisplayedMessages = 8;

    [SerializeField]
    private Font font;

    private Dictionary<string, Command> commands =
        new Dictionary<string, Command>();
    private List<ConsoleMessage> output;
    private bool focused;
    private string userInput;

    /// <summary>
    /// Registers the commmand with the console, so that it can be invoked by
    /// typing the command's name into the simulator console's input field.
    /// </summary>
    public void RegisterCommand(Command command)
    {
        commands[command.Name] = command;
    }

    /// <summary>
    /// Unregisters the command with the simulator console.
    /// </summary>
    public void UnregisterCommand(Command command)
    {
        commands.Remove(command.Name);
    }

    /// <summary>
    /// Prints the given string to the simulator console.
    /// </summary>
    public void WriteLine(string value)
    {
        // Append the message to the output text.
        ConsoleMessage message = new ConsoleMessage(value, Time.time + 5f);
        output.Add(message);

        // Prevent overflow.
        if (output.Count > 8)
        {
            output.RemoveAt(0);
        }
    }

    private void OnEnable()
    {
        focused = false;
        userInput = "";
        output = new List<ConsoleMessage>();
    }

    private void Update()
    {
        // Check if the user wants to use the console.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            focused = true;
        }
    }

    private void OnGUI()
    {
        // Check if the user wants to escape the console.
        if (Event.current.keyCode == KeyCode.Escape)
        {
            userInput = "";
            focused = false;
        }

        // Check if the user wants to execute the currently-inputted command.
        if (Event.current.keyCode == KeyCode.Return && userInput != "")
        {
            ExecuteCommand();
            // Reset input text field.
            userInput = "";
            // Unfocus the console.
            focused = false;
        }

        // Render the GUI.
        GUI.skin.font = font;
        RenderOutput();
        RenderInput();
    }

    /// <summary>
    /// Renders the console output GUI element.
    /// </summary>
    private void RenderOutput()
    {
        if (focused)
        {
            Rect outputPosition = new Rect(10, 10, Screen.width - 20, 160);
            string outputText = "";

            // Add padding.
            for (int i = 0; i < MaxDisplayedMessages - output.Count; i++)
            {
                outputText += '\n';
            }

            foreach (ConsoleMessage message in output)
            {
                outputText += message.text + '\n';
            }

            GUI.TextArea(outputPosition, outputText);
            GUI.EndScrollView();
        }
        else
        {
            GUI.backgroundColor = Color.clear;
            Rect outputPosition = new Rect(10, 10, Screen.width - 20, 160);
            string outputText = "";

            // Only draw unfaded messages.
            List<ConsoleMessage> unfadedMessages = new List<ConsoleMessage>();
            foreach (ConsoleMessage message in output)
            {
                if (!message.HasFaded())
                {
                    unfadedMessages.Add(message);
                }
            }

            // Add padding.
            for (int i = 0; i < MaxDisplayedMessages - unfadedMessages.Count; i++)
            {
                outputText += '\n';
            }

            foreach (ConsoleMessage message in unfadedMessages)
            {
                if (!message.HasFaded())
                {
                    outputText += message.text + '\n';
                }
            }

            GUI.TextArea(outputPosition, outputText);
            // Prevent output text from being focused.
            GUI.FocusControl(null);
        }
    }

    /// <summary>
    /// Renders the console input GUI element.
    /// </summary>
    private void RenderInput()
    {
        if (focused)
        {
            Rect inputPosition = new Rect(10, 170, Screen.width - 20, 20);
            GUI.color = Color.white;
            GUI.SetNextControlName("ConsoleInput");
            userInput = GUI.TextField(inputPosition, userInput);
            GUI.FocusControl("ConsoleInput");
        }
    }

    /// <summary>
    /// Executes the command typed into the simulator console input field.
    /// </summary>
    private void ExecuteCommand()
    {
        // Tokenize input.
        string[] tokens = userInput.Split(' ');

        // Ensure command exists.
        string commandName = tokens[0];
        if (!commands.ContainsKey(commandName))
        {
            WriteLine("No such command: " + commandName);
            return;
        }

        // Extract args.
        string[] args = new string[tokens.Length - 1];
        Array.Copy(tokens, 1, args, 0, tokens.Length - 1);

        // Execute command.
        Command command = commands[commandName];
        command.Execute(args);
    }

    /// <summary>
    /// Represents a message printed to the simulator console that fades out
    /// after a given time.
    /// </summary>
    private class ConsoleMessage
    {
        public string text;
        public float fadeTime;

        public ConsoleMessage(string text, float fadeTime)
        {
            this.text = text;
            this.fadeTime = fadeTime;
        }

        public bool HasFaded()
        {
            return Time.time >= fadeTime;
        }
    }
}
