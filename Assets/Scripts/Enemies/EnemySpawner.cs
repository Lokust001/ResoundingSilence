/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/30/2026
* Summary: Spawns and caches enemies when player is nearby
* To Do:   
*/


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private int dangerPoints;

    [SerializeField]
    private int pointsPerSecondPassed;

    [SerializeField]
    private float enemyGenerationTime;

    private bool generationReadyFlag;

    [SerializeField]
    List<Transform> spawnLocations;

    [SerializeField]
    SpawnTier[] enemyGroupTiers;

    [Header("[Debug] Don't edit these fields"), SerializeField]
    List<Enemy> generatedEnemies;

    /// <summary>
    /// Instantiates generatedEnemies so that it isn't null and allows the first generation to happen
    /// </summary>
    private void Awake()
    {
        generatedEnemies = new List<Enemy>();
        generationReadyFlag = true;

        //Retrieves all spawn transforms and removes the one on the parent gameobject
        spawnLocations = GetComponentsInChildren<Transform>().ToList<Transform>();
        spawnLocations.Remove(transform);

    }

    /// <summary>
    /// Start spwaning enemies when player is about to be nearby
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        //If there is ONLY a PlayerController in the parent object and not the current object
        //(which prevents the player model from double activation)
        if (other.GetComponentInParent<PlayerController>() != null && other.GetComponent<PlayerController>() == null)
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
        if (other.GetComponentInParent<PlayerController>() != null && other.GetComponent<PlayerController>() == null)
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
        List<Transform> transformCopy = spawnLocations.ToList<Transform>();

        if (transformCopy.Count < generatedEnemies.Count) 
        {
            Debug.LogWarning("An enemy spawner has less transforms than the amount of enemies it needs to spawn, re-using some transforms");
        }


        foreach (var enemy in generatedEnemies)
        {
            int randomIndex = Random.Range(0, transformCopy.Count);
            Vector3 spawnLocation = transformCopy[randomIndex].position;
            enemy.transform.position = spawnLocation;
            transformCopy.RemoveAt(randomIndex);
            enemy.EnableEnemy();
            
        }
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
            enemy.DisableEnemy();
        }
    }


    /// <summary>
    /// Calculates and assigns a tier of enemies based on the number of danger points
    /// </summary>
    private void GenerateEnemyGroup() 
    {
        //Initialization
        List<GameObject> calculatedEnemyPool = new();

        dangerPoints = GameTimerManager.Instance.GetRoundedGameTime() * pointsPerSecondPassed;
        Debug.Log("Danger Points calculated to be: " + dangerPoints + " pts, from " + GameTimerManager.Instance.GetRoundedGameTime() + "secs x " + pointsPerSecondPassed + 
            " points per second passed");

        //Loop through the tiers starting from the highest tier
        //If this spawner's danger points matches any tier,
        //assign the enemypool and then stop this loop
        for (int i = enemyGroupTiers.Length - 1; i >= 0; i--) 
        {
            if (dangerPoints >= enemyGroupTiers[i].minimumPoints) 
            {
                calculatedEnemyPool = enemyGroupTiers[i].enemyPool.ToList<GameObject>();
                Debug.Log(enemyGroupTiers[i].tierName + " was chosen from calculated danger points");
                break;
            }
        }

        foreach (var enemy in calculatedEnemyPool) 
        {
            Enemy createdEnemy = Instantiate(enemy, transform.position, Quaternion.identity).GetComponent<Enemy>();
            createdEnemy.DisableEnemy();
            createdEnemy.destroyCancellationToken.Register(() => 
            //Inner Method that removes the destroyed enemy from the list,
            //then starts a timer for the next generation interval
            {
                
                generatedEnemies.Remove(createdEnemy);
                if (generatedEnemies.Count <= 0 && this != null)
                    StartCoroutine(NextGenerationCooldown());
            });
            
            generatedEnemies.Add(createdEnemy);
        }
    }

    private IEnumerator NextGenerationCooldown()
    {
        yield return new WaitForSeconds(enemyGenerationTime);

        generationReadyFlag = true;
    }


}
