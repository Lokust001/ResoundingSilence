/*
* Author: Tyler
* Contributors:
* Last Modified: 10/12/2026
* Summary: This service stores all the player's data. May be merged with the midrundatamanager if we feel its redundant
* To Do:   N/A
*/

using System.Threading.Tasks;
using UnityEngine;

public class PlayerManager : BaseManager
{
    public static PlayerManager Instance;

    [SerializeField]
    private PlayerController playerPrefab;

    [HideInInspector]
    public PlayerController player;

    /// <summary>
    /// inits the manager and sets up public events
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
    }

    /// <summary>
    /// subscribes to all the public events 
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.StartGamePressed += SpawnPlayer;
        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.StartGamePressed -= SpawnPlayer;
    }

    /// <summary>
    /// spawns in the player prefab
    /// </summary>
    private void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, 
                             MapManager.Instance.spawnedMap.PlayerSpawnpoint.position, 
                             Quaternion.identity, 
                             transform);
        GenericPublicEvents.PlayerSpawnedIn?.Invoke(player);
    }
}
