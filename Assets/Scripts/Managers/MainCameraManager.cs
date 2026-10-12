using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class MainCameraManager : BaseManager
{
    public static MainCameraManager Instance;

    [SerializeField]
    private Camera mainCam;
    private Camera camInstance;
    
    private Vector3 CamOffset;

    private Transform player;
    private bool FollowPlayer;
    private Coroutine followingCoroutine;

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

    private async Awaitable SpawnCamera()
    {
        camInstance = Instantiate(mainCam, transform);
        CamOffset = camInstance.transform.localPosition;
        await Task.CompletedTask;
    }

    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.PlayerSpawnedIn += AttachCamToPlayer;

        await Task.CompletedTask;
    }

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

    private IEnumerator CamFollowPlayer()
    {
        FollowPlayer = true;
        while (FollowPlayer)
        {
            camInstance.transform.position = player.position + CamOffset;
            yield return null;
        }
    }
}
