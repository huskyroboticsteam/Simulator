using UnityEngine;

public class RoverCamera : MonoBehaviour
{
    public GameObject rover;

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - rover.transform.position;
    }

    private void Update()
    {
        transform.position = rover.transform.position + offset;
    }
}
