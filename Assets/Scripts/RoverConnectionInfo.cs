using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the connection status of the rover.
/// </summary>
[RequireComponent(typeof(Text))]
public class RoverConnectionInfo : MonoBehaviour
{
    [SerializeField]
    private RoverSocket _socket;

    private Text _text;

    private void Start()
    {
        _text = GetComponent<Text>();
    }

    private void Update()
    {
        if (_socket.IsConnected)
        {
            _text.text = "Rover Connected";
            _text.color = Color.green;
        }
        else
        {
            _text.text = "Rover Disconnected";
            _text.color = Color.red;
        }
    }
}
