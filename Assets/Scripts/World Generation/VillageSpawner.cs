/*
* Author: Brenden
* Contributors:
* Last Modified: 09/18/2026
* Summary: Procederally Generates villages when they are spawned in so that they don't all look the same using a tile system
* To Do:   N/A
*/

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class VillageSpawner : MonoBehaviour
{
    [SerializeField, Tooltip("Where all the tile prefabs that can make up a village goes")] 
    private List<GameObject> villageTiles;
    [SerializeField, Tooltip("All the spawn locations where village tiles can spawn, rotation of these points will matter")]
    private List<GameObject> villageSpawnPoints;
    [HideInInspector] public List<GameObject> SpawnedTiles;

    private void Start()
    {
        SpawnVillage();
    }

    /// <summary>
    /// This gets called by the world spawner when a village gets spawned so it for sure gets called in the right order
    /// </summary>
    public void SpawnVillage()
    {
        foreach(GameObject point in villageSpawnPoints)
        {
            GameObject Tile = villageTiles[Random.Range(0, villageTiles.Count)];
            GameObject TileInstance = Instantiate(Tile, point.transform.position, point.transform.rotation);
            SpawnedTiles.Add(TileInstance);
            villageTiles.Remove(Tile);
        }
    }
}
