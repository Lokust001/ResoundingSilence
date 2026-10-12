using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : BaseManager
{
    public static PlayerManager Instance;

    [SerializeField]
    private PlayerController playerPrefab;

    [HideInInspector]
    public PlayerController player;

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
    }

    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.StartGamePressed += SpawnPlayer;
        await Task.CompletedTask;
    }

    private void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, 
                             MapManager.Instance.spawnedMap.PlayerSpawnpoint.position, 
                             Quaternion.identity, 
                             transform);
        GenericPublicEvents.PlayerSpawnedIn?.Invoke(player);
    }
}
