/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/30/2026
* Summary: Spawns and caches enemies when player is nearby
* To Do:   
*/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private int dangerPoints;

    private float distanceFromSpawn;

    [SerializeField]
    private float enemyGenerationTime;

    private bool generationReadyFlag;

    [SerializeField]
    Transform[] spawnLocations;

    [SerializeField]
    List<SpawnTier> enemyGroupTiers;

    List<GameObject> generatedEnemies;

    /// <summary>
    /// Instantiates generatedEnemies so that it isn't null and allows the first generation to happen
    /// </summary>
    private void Awake()
    {
        generatedEnemies = new List<GameObject>();
        generationReadyFlag = true;
    }

    /// <summary>
    /// Start spwaning enemies when player is about to be nearby
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController p))
        {
            SpawnEnemies();
        }
    }

    /// <summary>
    /// Start despawning enemies when player is about to leave the location
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController p))
        {
            DespawnEnemies();
        }
    }


    /// <summary>
    /// Spawns a group of enemies at random spawn locations.
    /// If a group of enemies has not been chosen yet, choose one.
    /// </summary>
    private void SpawnEnemies() 
    {
        //If there is no chosen enemy picked, generate some
        if (generatedEnemies.Count == 0 && generationReadyFlag)
        {
            generationReadyFlag = false;
            GenerateEnemyGroup();
        }

        PlaceEnemiesAtTransforms();
    }

    /// <summary>
    /// Makes new enemies or unfinishing existing enmies appear at random spawn locations
    /// </summary>
    private void PlaceEnemiesAtTransforms() 
    {
        foreach (var enemy in generatedEnemies)
        {
            enemy.SetActive(true);
            enemy.transform.position = GetRandomSpawnPosition();
        }
    }

    /// <summary>
    /// Finds a random spawn position for the enemy to start on that hasn't been taken up yet
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomSpawnPosition() 
    {
        //TODO: Check that an enemy isn't already there
        int randomIndex = Random.Range(0, spawnLocations.Length + 1);
        return spawnLocations[randomIndex].position;
    }


    /// <summary>
    /// If any enemies are still alive, cache the enemies for future spawning.
    /// </summary>
    private void DespawnEnemies() 
    {
        //If they've finished off all enemies, don't run this method
        if (generatedEnemies.Count < 1)
            return;

        foreach (var enemy in generatedEnemies) 
        {
            enemy.SetActive(false);
        }
    }

    /// <summary>
    /// Calculates and assigns a tier of enemies based on the number of danger points
    /// </summary>
    private void GenerateEnemyGroup() 
    {
        //Logic to decide Tier goes here after discussion

        //Debug tier decision
        GameObject[] calculatedEnemyPool = enemyGroupTiers[0].enemyPool;

        foreach (var enemy in calculatedEnemyPool) 
        {
            GameObject createdEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
            createdEnemy.SetActive(false);
            generatedEnemies.Add(createdEnemy);
        }
    }

    private IEnumerator NextGenerationCooldown()
    {
        yield return new WaitForSeconds(enemyGenerationTime);
        generationReadyFlag = true;
    }


}
