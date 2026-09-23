/*
* Author: Brenden
* Contributors:
* Last Modified: 09/21/2026
* Summary: Holds and stores the data of each island
* To Do:   Make the fast travel points mean something
*/
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class IslandData : MonoBehaviour
{
    public List<GameObject> ObjectiveSpawns;
    public List<GameObject> BuffSpawns;
    public List<GameObject> ShopSpawns;
    public List<GameObject> FastTravelPoints;
    [SerializeField] private List<GameObject> enemySpawnPoints;
    [SerializeField] private int howManySpawnPoints;
    public List<GameObject> ActiveEnemySpawnPoints;
    public GameObject realFastTravel;

    /// <summary>
    /// This is for later use when get get fast travel in
    /// </summary>
    public void ChooseFastTravel()
    {
        realFastTravel = FastTravelPoints[Random.Range(0, FastTravelPoints.Count)];
        realFastTravel.SetActive(true);
    }

    /// <summary>
    /// this gets called to pick what spawn points are active, This will be used on map generation, it might be used again when more enemies are spawned but that isn't decided yet
    /// </summary>
    public void ChooseEnemySpawns()
    {
        ActiveEnemySpawnPoints.Clear();
        foreach(GameObject p in enemySpawnPoints)
        {
            p.SetActive(false);
        }
        for(int i = 0;  i < howManySpawnPoints; i++)
        {
            bool found = true;
            while(found)
            {
                int spawnPointNumber = Random.Range(0, FastTravelPoints.Count);
                if (!enemySpawnPoints[spawnPointNumber].gameObject.activeSelf)
                {
                    enemySpawnPoints[spawnPointNumber].SetActive(true);
                    found = false;
                }
                ActiveEnemySpawnPoints.Add(enemySpawnPoints[spawnPointNumber]);
            }
        } 
    }
}
