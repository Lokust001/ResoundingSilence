using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MapManager : BaseManager
{
    public static MapManager Instance;

    [SerializeField]
    private List<MapGenerator> maps;

    
    public MapGenerator spawnedMap;

    /// <summary>
    /// initializes the player inputs
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

    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.AllManagersInitialized += SpawnMapLayout;

        await Task.CompletedTask;
    }

    private void OnDestroy()
    {
        GenericPublicEvents.AllManagersInitialized -= SpawnMapLayout;
    }

    private void SpawnMapLayout()
    {
        int rand = Random.Range(0, maps.Count);
        spawnedMap = Instantiate(maps[rand], transform);
    }
}
