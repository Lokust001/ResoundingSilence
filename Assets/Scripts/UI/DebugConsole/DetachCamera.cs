using System.Collections;
using UnityEngine;

public class DetachCamera : MonoBehaviour
{
    public bool isActive;
    Vector2 inputs;
    int rise;

    public MainCameraManager CameraManager;
    public GameObject camera;
    public Transform cameraLookOffset;
    [SerializeField] float speed;

    private Coroutine CameraPivotInstance;

    /// <summary>
    /// Allows the script to listen for player input
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.AbilityOnePressed += RaiseCamera;
        InputPublicEvents.AbilityOneReleased += noCameraZoom;
        InputPublicEvents.AbilityTwoPressed += LowerCamera;
        InputPublicEvents.AbilityTwoReleased += noCameraZoom;
        InputPublicEvents.MovePressed += PlayerInputStarted;
        InputPublicEvents.MoveReleased += PlayerInputEnded;
    }


    /// <summary>
    /// Stops listening when the object is destroyed. 
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.AbilityOnePressed -= RaiseCamera;
        InputPublicEvents.AbilityOneReleased -= noCameraZoom;
        InputPublicEvents.AbilityTwoPressed -= LowerCamera;
        InputPublicEvents.AbilityTwoReleased -= noCameraZoom;
        InputPublicEvents.MovePressed -= PlayerInputStarted;
        InputPublicEvents.MoveReleased -= PlayerInputEnded;
    }

    /// <summary>
    /// sets the input amount to how much the player is moving.
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputStarted(Vector2 moveDirection)
    {
        inputs = moveDirection;
        if (rise == 0 && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// if no inputs are happening sets it to 0 and stops the coroutine if no other unputs are happening
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputEnded()
    {
        inputs = Vector2.zero;
        if (rise == 0 && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
    }

    /// <summary>
    /// sets the raise vaule to -1 to lower the camera
    /// </summary>
    void LowerCamera()
    {
        rise = -1;
        if (rise == 0 && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// sets the raise vaule to 1 to raise the camera 
    /// </summary>
    void RaiseCamera()
    {
        rise = 1;
        if (rise == 0 && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// sets the raise to 0 and stops the coroutine if there is no other input happening
    /// </summary>
    void noCameraZoom()
    {
        rise = 0;
        if (inputs == Vector2.zero && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
    }

    /// <summary>
    /// Coroutine that continuously runs and moves the player unless disabled by external factors.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveCamera()
    {
        while (isActive)
        {
            MoveCamera(inputs, rise);

            yield return new WaitForFixedUpdate();

        }
    }

    /// <summary>
    /// changes where the camera is positioned around the player
    /// </summary>
    /// <param name="moveDirection"></param>
    /// <param name="rise"></param>
    void MoveCamera(Vector2 moveDirection = default, int rise = 0)
    {
        if (camera == null)
        {
            camera = CameraManager.camInstance.gameObject;
        }
        if (cameraLookOffset == null)
        {
            cameraLookOffset = GameObject.Find("CameraLookOffset").transform;
        }
        if (isActive)
        {
            Vector3 current = CameraManager.CamOffset;
            camera.transform.position = camera.transform.position + (new Vector3(moveDirection.x, rise, moveDirection.y) * speed);
        }
    }
}
