using UnityEngine;

/// <summary>
/// Component applied to the main camera to make it follow a target GameObject.
/// </summary>
public class MainCameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;
    private Vector3 _offset;

    private void OnEnable()
    {
        _offset = transform.position - _target.transform.position;
    }

    private void Update()
    {
        transform.position = _target.transform.position + _offset;
    }
}
