using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component that controls the simulator user interface.
/// </summary>
public class UIController : MonoBehaviour
{
    public InputField consoleInput;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (consoleInput.isFocused)
            {
                string input = consoleInput.text;
                string[] tokens = input.Split(' ');
                string commandName = tokens[0];
                string[] args = new string[tokens.Length - 1];
                Array.Copy(tokens, 1, args, 0, args.Length);
                CommandManager.Execute(commandName, args);
                consoleInput.text = "";
            }
            else
            {
                consoleInput.ActivateInputField();
            }
        }
    }
}
