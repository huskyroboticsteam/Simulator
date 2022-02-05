using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

/// <summary>
/// A camera on the rover which may stream to the rover server.
/// </summary>
[RequireComponent(typeof(Camera))]
public class RoverCamera : MonoBehaviour
{
    [SerializeField]
    private string _cameraName;
    private bool _isStreaming;
    private Camera _camera;
    private RoverSocket _socket;

    /// <summary>
    /// The name that identifies this camera.
    /// </summary>
    public string CameraName
    {
        get { return _cameraName; }
    }

    /// <summary>
    /// The frames per second at which this camera streams.
    /// </summary>
    public float StreamFps { get; set; }

    /// <summary>
    /// The width of this camera's stream.
    /// </summary>
    public int StreamWidth { get; set; }

    /// <summary>
    /// The height of this camera's stream.
    /// </summary>
    public int StreamHeight { get; set; }

    /// <summary>
    /// Whether this camera is streaming to the rover server.
    /// </summary>
    public bool IsStreaming
    {
        get { return _isStreaming; }
        set
        {
            if (_isStreaming == value)
            {
                return;
            }
            _isStreaming = value;
            if (_isStreaming)
            {
                StartCoroutine(StreamFrames());
            }
            else
            {
                StopAllCoroutines();
            }
        }
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _socket = FindObjectOfType<RoverSocket>();
    }

    private void OnEnable()
    {
        if (IsStreaming)
        {
            StartCoroutine(StreamFrames());
        }
    }

    private void OnDisable()
    {
        if (IsStreaming)
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator StreamFrames()
    {
        while (true)
        {
            ReportFrame();
            yield return new WaitForSeconds(1 / StreamFps);
        }
    }

    private void ReportFrame()
    {
        RenderTexture renderTexture = new RenderTexture(StreamWidth, StreamHeight, 24);
        RenderTexture.active = renderTexture;

        _camera.targetTexture = renderTexture;
        _camera.Render();

        Texture2D frame = new Texture2D(StreamWidth, StreamHeight, TextureFormat.RGB24, false);
        frame.ReadPixels(new Rect(0, 0, StreamWidth, StreamHeight), 0, 0);

        byte[] bytes = frame.EncodeToJPG();
        string streamData = Convert.ToBase64String(bytes);

        // Clean up to prevent memory leaks.
        _camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(frame);

        JObject cameraStreamReport = new JObject()
        {
            ["type"] = "simCameraStreamReport",
            ["camera"] = CameraName,
            ["data"] = streamData
        };
        _socket.Send(cameraStreamReport);
    }
}
