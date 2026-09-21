/*
* Author: Brenden
* Contributors:
* Last Modified: 09/21/2026
* Summary: Script that spawns in all the small pieces of the map
* To Do:   N/A
*/
using System.Collections.Generic;
using UnityEngine;

public class MapSectionSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("This is a list for the purpose of multiple prefabs that can be spawned in different spots, very much can be 1")] 
    private List<GameObject> spawnPoints;
    [SerializeField] private List<GameObject> prefabs;

    /// <summary>
    /// Spawns in the islands that can we spawned in from this pool and then returns them in a list so that the referances can be send back to map generator
    /// </summary>
    /// <returns></returns>
    public List<GameObject> SpawnIsland()
    {
        List<GameObject> worldpoints = new List<GameObject>();
        foreach (GameObject point in spawnPoints)
        {
            int prefabNumber = Random.Range(0, prefabs.Count);
            GameObject temp = Instantiate(prefabs[prefabNumber], point.transform.position, point.transform.rotation);
            worldpoints.Add(temp);
            prefabs.Remove(prefabs[prefabNumber]);
        }
        return worldpoints;
    }
}
