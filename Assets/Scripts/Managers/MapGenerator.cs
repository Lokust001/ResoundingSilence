/*
* Author: Brenden
* Contributors:
* Last Modified: 09/21/2026
* Summary: Holds and stores the data of the whole map
* To Do:   N/A
*/
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] List<MapSectionSpawner> Spawners;
    private List<GameObject> islands = new List<GameObject>();
    private List<GameObject> POISpots = new List<GameObject>();

    /// <summary>
    /// Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        spawnMap();
    }

    /// <summary>
    /// Loops though the spawners and keeps track of all the islands that are spawned
    /// </summary>
    public void spawnMap()
    {
        foreach(MapSectionSpawner spawner in  Spawners)
        {
            List<GameObject> tempList = spawner.SpawnIsland();
            foreach(GameObject temp in tempList)
            {
                islands.Add(temp);
            }
        }
    }

    /// <summary>
    /// This used to populate the list of for the spawnpoints of the points of interets and buff structures
    /// </summary>
    public void GetPOIspawns()
    {
        foreach(GameObject island in islands)
        {
            foreach(GameObject spawn in island.GetComponent<IslandData>().POISpawns)
            {
                POISpots.Add(spawn);
            }
        }
    }
}
