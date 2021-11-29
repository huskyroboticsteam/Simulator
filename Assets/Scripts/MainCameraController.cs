using UnityEngine;

/// <summary>
/// Component applied to the main camera to make it follow the rover.
/// </summary>
public class MainCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _rover;
    private Vector3 _offset;

    private void OnEnable()
    {
        _offset = transform.position - _rover.transform.position;
    }

    private void Update()
    {
        transform.position = _rover.transform.position + _offset;
    }
}
