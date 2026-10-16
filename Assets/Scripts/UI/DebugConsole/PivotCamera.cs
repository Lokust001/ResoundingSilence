using System.Collections;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

public class PivotCamera : MonoBehaviour
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
    /// Helper method to update PlayerVelocity based on user input
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputStarted(Vector2 moveDirection)
    {
        inputs = moveDirection;
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

    /// <summary>
    /// Helper method to update PlayerVelocity based on user input
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputEnded()
    {
        inputs = Vector2.zero;
        if(rise == 0 && CameraPivotInstance != null)
        {
            StopCoroutine(CameraPivotInstance);
        }
    }

    void LowerCamera()
    {
        rise = -1;
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

    void RaiseCamera()
    {
        rise = 1;
        CameraPivotInstance = StartCoroutine(MoveCamera());
    }

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
        if(cameraLookOffset == null)
        {
            cameraLookOffset = GameObject.Find("CameraLookOffset").transform;
        }
        if (isActive)
        {
            Vector3 current = CameraManager.CamOffset;
            CameraManager.CamOffset = current + (new Vector3(moveDirection.x, rise, moveDirection.y) * speed);
            camera.transform.LookAt(cameraLookOffset);
        }
    }
}
