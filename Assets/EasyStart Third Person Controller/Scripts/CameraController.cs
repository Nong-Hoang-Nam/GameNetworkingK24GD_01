
using UnityEngine;
using Fusion;

/*
    This file has a commented version with details about how each line works. 
    The commented version contains code that is easier and simpler to read. This file is minified.
*/

/// <summary>
/// Camera movement script for third person games.
/// This Script should not be applied to the camera! It is attached to an empty object and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{

    [Tooltip("Enable to move the camera by holding the right mouse button. Does not work with joysticks.")]
    public bool clickToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;
    [Tooltip("Minimum and maximum Field of View for zooming.")]
    public float minFov = 40f;
    public float maxFov = 70f;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 5f;

    [Tooltip("Camera Y rotation limits. The X axis is the maximum it can go up and the Y axis is the maximum it can go down.")]
    public Vector2 cameraLimit = new Vector2(-45, 40);

    [Tooltip("Optional anchor under player to follow (e.g., CameraAnchor). If null, follows player root.")]
    public Transform followTarget;

    Camera _cam;
    bool _isLocal = true;
    float mouseX;
    float mouseY;
    Vector3 initialOffsetWS;

    Transform player;

    void Start()
    {

        // Xác định quyền điều khiển local theo NetworkObject cha (nếu có)
        var netObj = GetComponentInParent<NetworkObject>();
        if (netObj != null)
            _isLocal = netObj.HasInputAuthority;

        // Nếu không phải local player, tắt camera và script để tránh follow nhầm
        if (!_isLocal)
        {
            _cam = GetComponentInChildren<Camera>(true);
            if (_cam) _cam.enabled = false;
            enabled = false;
            return;
        }

        // Camera local: follow đúng player cha
        player = transform.root != null ? transform.root : transform.parent;
        if (player == null)
            player = transform; // fallback an toàn
        // Nếu chưa gán anchor, thử tìm con tên "CameraAnchor"
        if (followTarget == null && player != null)
        {
            var t = player.Find("CameraAnchor");
            if (t != null) followTarget = t;
        }
        var anchor = followTarget != null ? followTarget : player;
        initialOffsetWS = transform.position - anchor.position;

        // Lấy camera con thay vì Camera.main để không bị nhầm khi có nhiều camera (multiplayer)
        _cam = GetComponentInChildren<Camera>(true);

        // Lock and hide cursor with option isn't checked
        if ( ! clickToMoveCamera )
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

    }


    void Update()
    {

        // Follow anchor theo offset world ban đầu
        var anchor = followTarget != null ? followTarget : player;
        if (anchor)
            transform.position = anchor.position + initialOffsetWS;

        // Set camera zoom when mouse wheel is scrolled
        if( canZoom && _cam != null )
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            if (wheel != 0f)
            {
                float fov = _cam.fieldOfView - wheel * sensitivity * 20f;
                _cam.fieldOfView = Mathf.Clamp(fov, minFov, maxFov);
            }
        }

        // Checker for right click to move camera
        if ( clickToMoveCamera )
            if (Input.GetAxisRaw("Fire2") == 0)
                return;
            
        // Calculate new position
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity;
        // Apply camera limts
        mouseY = Mathf.Clamp(mouseY, cameraLimit.x, cameraLimit.y);

        transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

    }
}