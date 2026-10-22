/*
* Author: Tyler
* Contributors: Brenden
* Last Modified: 10/22/2026
* Summary: This service controls the main camera
* To Do:   N/A
*/

using System.Collections;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class MainCameraManager : BaseManager
{
    public static MainCameraManager Instance;

    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private CinemachineCamera FollowPlayercinemachine;
    [HideInInspector] public Camera camInstance;
    [HideInInspector] public CinemachineCamera followPlayerCinemachineInstance;
    
    [HideInInspector] public Vector3 CamOffset;

    private Transform player;
    private bool FollowPlayer;
    private Coroutine followingCoroutine;

    /// <summary>
    /// initializes the manager
    /// </summary>
    /// <returns></returns>
    public async override Awaitable InitManager()
    {
        await base.InitManager();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        await SetUpPublicEvents();
        await SpawnCamera();
    }

    /// <summary>
    /// spawns in the camera object and sets the offset
    /// </summary>
    /// <returns></returns>
    private async Awaitable SpawnCamera()
    {
        camInstance = Instantiate(mainCam, transform);
        followPlayerCinemachineInstance = Instantiate(FollowPlayercinemachine, transform).GetComponent<CinemachineCamera>(); ;
        CamOffset = followPlayerCinemachineInstance.transform.localPosition;

        if (AppManager.Instance.EnableTestingMode && FindAnyObjectByType<PlayerController>() != null)
        {
            AttachCamToPlayer(FindAnyObjectByType<PlayerController>());
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// enables the public events
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.PlayerSpawnedIn += AttachCamToPlayer;

        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.PlayerSpawnedIn -= AttachCamToPlayer;
    }

    /// <summary>
    /// sets the camera to move with the player
    /// </summary>
    /// <param name="player"></param>
    /// <exception cref="System.Exception"></exception>
    private void AttachCamToPlayer(PlayerController player)
    {
        if (player == null)
        {
            throw new System.Exception("Cant follow player if theres no player");
        }

        this.player = player.transform;

        if (followingCoroutine == null)
        {
            followingCoroutine = StartCoroutine(CamFollowPlayer());
        }
    }

    /// <summary>
    /// actually moves the camera to follow the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator CamFollowPlayer()
    {
        FollowPlayer = true;
        while (FollowPlayer)
        {
            followPlayerCinemachineInstance.transform.position = player.position + CamOffset;
            yield return null;
        }
    }

    public void stopFollow()
    {
        FollowPlayer = false;
        StopCoroutine(followingCoroutine);
        followingCoroutine = null;
    }

    public void startFollow()
    {
        AttachCamToPlayer(player.gameObject.GetComponent<PlayerController>());
    }
}
