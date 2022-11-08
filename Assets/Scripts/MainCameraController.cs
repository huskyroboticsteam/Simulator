using UnityEngine;

/// <summary>
/// Component applied to the main camera pivot to make it follow a target.
/// </summary>
public class MainCameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    private Vector3 _offset;

    private void OnEnable()
    {
        _offset = transform.position - _target.position;
    }

    private void Update()
    {
        transform.position = _target.position + _offset;
    }
}
