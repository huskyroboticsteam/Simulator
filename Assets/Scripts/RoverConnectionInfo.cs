using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RoverConnectionInfo : MonoBehaviour
{
    [SerializeField]
    private SimulatorSocket _socket;

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
