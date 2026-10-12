/*
* Author: Tyler
* Contributors:
* Last Modified: 10/7/2026
* Summary: This service spawns in a new map when the level starts. 
* NOTE: May be obsolete - the map generator may take this over but idk
* To Do:   N/A
*/

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

    /// <summary>
    /// subscribes to the needed public events
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        GenericPublicEvents.AllManagersInitialized += SpawnMapLayout;

        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from the public events
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.AllManagersInitialized -= SpawnMapLayout;
    }

    /// <summary>
    /// spawns in a map
    /// </summary>
    private void SpawnMapLayout()
    {
        int rand = Random.Range(0, maps.Count);
        spawnedMap = Instantiate(maps[rand], transform);
    }
}
