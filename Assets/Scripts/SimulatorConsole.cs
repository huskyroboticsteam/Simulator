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
    private Font _font;

    private IDictionary<string, Command> _commands = new Dictionary<string, Command>();
    private IList<ConsoleMessage> _output;
    private bool _focused;
    private string _userInput;

    /// <summary>
    /// Registers the given commmand with this console, so that it can be
    /// invoked by typing the command's name into the simulator console's input
    /// field.
    /// </summary>
    public void RegisterCommand(Command command)
    {
        _commands[command.Name] = command;
    }

    /// <summary>
    /// Unregisters the given command with this console.
    /// </summary>
    public void UnregisterCommand(Command command)
    {
        _commands.Remove(command.Name);
    }

    /// <summary>
    /// Prints the given string to this console.
    /// </summary>
    public void WriteLine(string value)
    {
        // Append the message to the output text.
        ConsoleMessage message = new ConsoleMessage(value, Time.time + 5f);
        _output.Add(message);

        // Prevent overflow.
        if (_output.Count > 8)
        {
            _output.RemoveAt(0);
        }
    }

    private void OnEnable()
    {
        _focused = false;
        _userInput = "";
        _output = new List<ConsoleMessage>();
    }

    private void Update()
    {
        // Check if the user wants to use the console.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _focused = true;
        }
    }

    private void OnGUI()
    {
        // Check if the user wants to escape the console.
        if (Event.current.keyCode == KeyCode.Escape)
        {
            _userInput = "";
            _focused = false;
        }

        // Check if the user wants to execute the currently-inputted command.
        if (Event.current.keyCode == KeyCode.Return && _userInput != "")
        {
            ExecuteCommand();
            // Reset input text field.
            _userInput = "";
            // Unfocus the console.
            _focused = false;
        }

        // Render the GUI.
        GUI.skin.font = _font;
        RenderOutput();
        RenderInput();
    }

    /// <summary>
    /// Renders the console output GUI element.
    /// </summary>
    private void RenderOutput()
    {
        if (_focused)
        {
            Rect outputPosition = new Rect(10, 10, Screen.width - 20, 160);
            string outputText = "";

            // Add padding.
            for (int i = 0; i < MaxDisplayedMessages - _output.Count; i++)
            {
                outputText += '\n';
            }

            foreach (ConsoleMessage message in _output)
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
            foreach (ConsoleMessage message in _output)
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
        if (_focused)
        {
            Rect inputPosition = new Rect(10, 170, Screen.width - 20, 20);
            GUI.color = Color.white;
            GUI.SetNextControlName("ConsoleInput");
            _userInput = GUI.TextField(inputPosition, _userInput);
            GUI.FocusControl("ConsoleInput");
        }
    }

    /// <summary>
    /// Executes the command typed into the simulator console input field.
    /// </summary>
    private void ExecuteCommand()
    {
        // Tokenize input.
        string[] tokens = _userInput.Split(' ');

        // Ensure command exists.
        string commandName = tokens[0];
        if (!_commands.ContainsKey(commandName))
        {
            WriteLine("No such command: " + commandName);
            return;
        }

        // Extract args.
        string[] args = new string[tokens.Length - 1];
        Array.Copy(tokens, 1, args, 0, tokens.Length - 1);

        // Execute command.
        Command command = _commands[commandName];
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
