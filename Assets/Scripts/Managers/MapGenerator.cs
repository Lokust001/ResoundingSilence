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
    private List<GameObject> ObjectiveSpots = new List<GameObject>();
    private List<GameObject> ShopSpots = new List<GameObject>();
    private List<GameObject> BuffSpots = new List<GameObject>();
    [SerializeField] private GameObject shopPrefab;
    [SerializeField] private int shopAmount;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private int chestMin;
    [SerializeField] private int chestMax;
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private int trapMin;
    [SerializeField] private int trapMax;
    [SerializeField] private GameObject terotPrefab;
    [SerializeField] private int terotMin;
    [SerializeField] private int terotMax;
    [SerializeField] private List<GameObject> objectivePrefabs;
    [SerializeField] private int objectiveMin;
    [SerializeField] private int objectiveMax;


    /// <summary>
    /// Temp call for spawnMap until I merege this system into the whole manager spawner
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
        foreach (MapSectionSpawner spawner in Spawners)
        {
            List<GameObject> tempList = spawner.SpawnIsland();
            foreach (GameObject temp in tempList)
            {
                islands.Add(temp);
            }
        }
    }

    /// <summary>
    /// Calls all the data gathering functions
    /// </summary>
    public void getIslandData()
    {
        GetObjectiveSpawns();
        GetShopSpawns();
        GetBuffSpawns();
    }

    /// <summary>
    /// Calls all the world population functions
    /// </summary>
    public void populateIsland()
    {
        foreach(GameObject island  in islands)
        {
            island.GetComponent<IslandData>().ChooseFastTravel();
            island.GetComponent<IslandData>().ChooseEnemySpawns();
        }
        SpawnBuffs();
        SpawnObjectives();
        SpawnShops();
    }


    /// <summary>
    /// This used to populate the list of for the spawnpoints for the objectives around the map
    /// </summary>
    private void GetObjectiveSpawns()
    {
        foreach (GameObject island in islands)
        {
            foreach (GameObject spawn in island.GetComponent<IslandData>().ObjectiveSpawns)
            {
                ObjectiveSpots.Add(spawn);
            }
        }
    }

    /// <summary>
    /// This used to populate the list of for the spawnpoints for the shops around the islands
    /// </summary>
    private void GetShopSpawns()
    {
        foreach (GameObject island in islands)
        {
            foreach (GameObject spawn in island.GetComponent<IslandData>().ShopSpawns)
            {
                ShopSpots.Add(spawn);
            }
        }
    }

    /// <summary>
    /// This used to populate the list of for the spawnpoints for the buffs around the islands
    /// </summary>
    private void GetBuffSpawns()
    {
        foreach (GameObject island in islands)
        {
            foreach (GameObject spawn in island.GetComponent<IslandData>().BuffSpawns)
            {
                BuffSpots.Add(spawn);
            }
        }
    }
    
    /// <summary>
    /// decides where the shop will spawn throughout the map
    /// </summary>
    private void SpawnShops()
    {
        for(int i = 0; i < shopAmount; i++)
        {
            int spawnNumber = Random.Range(0, ShopSpots.Count);
            Instantiate(shopPrefab, ShopSpots[spawnNumber].transform.position, ShopSpots[spawnNumber].transform.rotation);
            ShopSpots.Remove(ShopSpots[spawnNumber]);
        }
    }
    /// <summary>
    /// decides where the objectives will spawn throughout the map
    /// </summary>
    private void SpawnObjectives()
    {
        int objectiveAmount = Random.Range(objectiveMin, objectiveMax);
        for(int i = 0; i < objectiveAmount; i++)
        {
            int spawnNumber = Random.Range(0, ObjectiveSpots.Count);
            Instantiate(objectivePrefabs[Random.Range(0,objectivePrefabs.Count)], ObjectiveSpots[spawnNumber].transform.position, ObjectiveSpots[spawnNumber].transform.rotation);
            ObjectiveSpots.Remove(ObjectiveSpots[spawnNumber]);
        }
    }
    
    /// <summary>
    /// decides where the buffs will spawn throughout the map
    /// </summary>
    private void SpawnBuffs()
    {
        int amountTerot = Random.Range(terotMin, terotMax);
        int amountChest = Random.Range(chestMin, chestMax);
        int amountTrap = Random.Range(trapMin, trapMax);
        int TotalAmount = amountChest + amountTerot + amountTrap;
        for(int i = 0; i < TotalAmount; i++)
        {
            int spawnPoint = Random.Range(0, BuffSpots.Count);
            if(amountTerot > 0)
            {
                Instantiate(terotPrefab, BuffSpots[spawnPoint].transform.position, BuffSpots[spawnPoint].transform.rotation);
                amountTerot--;
            }
            else if (amountChest > 0)
            {
                Instantiate(chestPrefab, BuffSpots[spawnPoint].transform.position, BuffSpots[spawnPoint].transform.rotation);
                amountChest--;
            }
            else
            {
                Instantiate(trapPrefab, BuffSpots[spawnPoint].transform.position, BuffSpots[spawnPoint].transform.rotation);
                amountTrap--;
            }
            BuffSpots.Remove(BuffSpots[spawnPoint]);
        }
    }
}
