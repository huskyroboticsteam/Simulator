using UnityEngine;

/// <summary>
/// Component applied to the main camera to make it orbit a pivot.
/// </summary>
public class MainCameraOrbit : MonoBehaviour
{
    [SerializeField]
    private float _mouseSensitivity;
    [SerializeField]
    private float _scrollSensitivity;
    [SerializeField]
    private float _scrollAcceleration;
    [SerializeField]
    private float _movementSmoothing;

    private Transform _pivotTransform;
    private Transform _cameraTransform;

    private Vector3 _pivotRotation;
    private float _cameraDistance;

    void Start()
    {
        _pivotTransform = this.transform.parent;
        _cameraTransform = this.transform;

        _pivotRotation = _pivotTransform.rotation.eulerAngles;
        _cameraDistance = -1f * _cameraTransform.position.z;
    }

    void LateUpdate()
    {
        // Update pivot rotation if left-click is held.
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            if (mouseX != 0f || mouseY != 0f)
            {
                _pivotRotation.x -= mouseY * _mouseSensitivity;
                _pivotRotation.x = Mathf.Clamp(_pivotRotation.x, -20f, 45f);

                _pivotRotation.y += mouseX * _mouseSensitivity;
            }
        }

        // Update camera distance.
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollValue != 0f)
        {
            _cameraDistance -= scrollValue * _scrollSensitivity * _cameraDistance * _scrollAcceleration;
            _cameraDistance = Mathf.Clamp(_cameraDistance, 1.5f, 30f);
        }

        float t = Time.deltaTime * _movementSmoothing;  // The interpolation value

        // Apply changes to pivot transform
        Quaternion currentRotation = _pivotTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(_pivotRotation.x, _pivotRotation.y, 0);
        if (currentRotation != targetRotation)
        {
            _pivotTransform.rotation = Quaternion.Lerp(currentRotation, targetRotation, t);
        }

        // Apply changes to camera transform
        float currentZ = _cameraTransform.localPosition.z;
        float targetZ = -1f * _cameraDistance;
        if (currentZ != targetZ)
        {
            float newZ = Mathf.Lerp(currentZ, targetZ, t);
            _cameraTransform.localPosition = new Vector3(0, 0, newZ);
        }
    }
}
