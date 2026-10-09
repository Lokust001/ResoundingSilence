/*
* Author: Brenden
* Contributors:
* Last Modified: 09/21/2026
* Summary: Script that spawns in all the small pieces of the map
* To Do:   N/A
*/
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MapSectionSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("This is a list for the purpose of multiple prefabs that can be spawned in different spots, very much can be 1")] 
    private List<GameObject> spawnPoints;
    public List<GameObject> prefabs;
    private List<GameObject> Spawnlist;

    /// <summary>
    /// Spawns in the islands that can we spawned in from this pool and then returns them in a list so that the referances can be send back to map generator
    /// </summary>
    /// <returns></returns>
    public List<GameObject> SpawnIsland()
    {
        Spawnlist = prefabs;
        List<GameObject> worldpoints = new List<GameObject>();
        foreach (GameObject point in spawnPoints)
        {
            int prefabNumber = Random.Range(0, Spawnlist.Count);
            GameObject temp = Instantiate(Spawnlist[prefabNumber], point.transform.position, point.transform.rotation);
            temp.GetComponent<IslandData>().Spawner = this.GetComponent<MapSectionSpawner>();
            temp.GetComponent<IslandData>().SpawnPoint = point;
            worldpoints.Add(temp);
            Spawnlist.Remove(Spawnlist[prefabNumber]);
        }
        return worldpoints;
    }

    public GameObject SpawnSingleIsland(GameObject SpawnPoint, int Layout)
    {
        GameObject temp = Instantiate(prefabs[Layout], SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        temp.GetComponent<IslandData>().Spawner = this;
        temp.GetComponent<IslandData>().SpawnPoint = SpawnPoint;
        return temp;
    }
}
