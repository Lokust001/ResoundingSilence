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
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private int dangerPoints;
    [SerializeField]
    private LayerMask spawningTriggerLayer;

    [SerializeField]
    private int pointsPerSecondPassed;

    [SerializeField]
    private float enemyGenerationTime;
    [SerializeField]
    private GameObject enemySpawnsParent;

    [SerializeField]
    private int timeBetweenScalingEnemies = 60;

    [Header("Trigger Objects")]
    [SerializeField]
    private TriggerObjects spawnDespawnTrigger;
    [SerializeField]
    private TriggerObjects deAggroTrigger;

    private int spawnerEnemyScale;

    private bool generationReadyFlag;

    

    [SerializeField]
    SpawnTier[] enemyGroupTiers;


    [Header("[Debug] Don't edit these fields"), SerializeField]
    List<Enemy> generatedEnemies;

    [SerializeField]
    List<Transform> spawnLocations;

    /// <summary>
    /// Instantiates generatedEnemies so that it isn't null and allows the first generation to happen
    /// </summary>
    private void Awake()
    {
        generatedEnemies = new List<Enemy>();
        generationReadyFlag = true;

        //Retrieves all spawn transforms and removes the one on the parent gameobject
        spawnLocations = enemySpawnsParent.GetComponentsInChildren<Transform>().ToList<Transform>();
        spawnLocations.Remove(enemySpawnsParent.transform);

    }
    /// <summary>
    /// Add listener for Player enter or exiting the spawn & de-aggro trigger
    /// </summary>
    private void OnEnable()
    {
        spawnDespawnTrigger.PlayerEnterOrExit += SpawnOrDespawnHandler;
        deAggroTrigger.PlayerEnterOrExit += DeAggroHandler;
    }

    /// <summary>
    /// Remove listener for Player enter or exiting the spawn & de-aggro trigger
    /// </summary>
    private void OnDisable()
    {
        spawnDespawnTrigger.PlayerEnterOrExit -= SpawnOrDespawnHandler;
        deAggroTrigger.PlayerEnterOrExit-= DeAggroHandler;
    }

    private void SpawnOrDespawnHandler(bool playerEnters) 
    {
        if (playerEnters)
        {
            SpawnEnemies();
        }
        else 
        {
            DespawnEnemies();
        }
    }


    public void AggroAllEnemies() 
    {
        foreach (var enemy in generatedEnemies) 
        {
            enemy.AggroEnemy();
        }
    }

    private void DeAggroHandler(bool playerEnters)
    {
        //If player is leaving De-Aggro Trigger
        if (!playerEnters)
        {
            Debug.Log("De-aggro-ing enemies");
            foreach (var enemy in generatedEnemies)
            {
                enemy.EnemyDeAggro();
            }
        }
        //If player goes into de-aggro range, re-enable aggro triggers
        else 
        {
            foreach (var enemy in generatedEnemies)
            {
                enemy.EnableAggroFunctionality();
            }
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

        bool scaleThisTime = EnemyNeedsScaling();

        foreach (var enemy in generatedEnemies)
        {
            //Find a random Vector3 location and then remove that from being re-used
            int randomIndex = Random.Range(0, transformCopy.Count);
            Vector3 spawnLocation = transformCopy[randomIndex].position;
            enemy.transform.position = spawnLocation;
            transformCopy.RemoveAt(randomIndex);

            //Scale the enemy and enable it
            if(scaleThisTime)
                enemy.ScaleEnemy(spawnerEnemyScale);
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
    /// Grabs a random enemy group from the groupList
    /// </summary>
    /// <param name="groupList"></param>
    /// <returns></returns>
    private List<GameObject> RandomEnemyGroupFromTier(SpawnTier.EnemyGroup[] groupList) 
    {
        int randomGroupIndex = Random.Range(0, groupList.Length);
        return groupList[randomGroupIndex].enemies.ToList<GameObject>();
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
                calculatedEnemyPool = RandomEnemyGroupFromTier(enemyGroupTiers[i].enemyGroups);
                Debug.Log(enemyGroupTiers[i].tierName + " was chosen from calculated danger points");
                break;
            }
        }
        //Do not store result as enemies will be generated so they MUST be scaled,
        //the method will update the variable needed for the scale
        EnemyNeedsScaling();

        foreach (var enemy in calculatedEnemyPool)
        {
            //Create the enemy
            Enemy createdEnemy = Instantiate(enemy, transform.position, Quaternion.identity).GetComponent<Enemy>();
            
            //Scale the enemy's stats based on the calculated level or scale value
            createdEnemy.ScaleEnemy(spawnerEnemyScale);
            createdEnemy.aggroEnemyGroup += AggroAllEnemies;
            createdEnemy.destroyCancellationToken.Register(() =>
            //Inner Method that removes the destroyed enemy from the list,
            //then starts a timer for the next generation interval
            {
                createdEnemy.aggroEnemyGroup -= AggroAllEnemies;
                generatedEnemies.Remove(createdEnemy);
                if (generatedEnemies.Count <= 0 && this != null)
                    StartCoroutine(NextGenerationCooldown());
            });

            generatedEnemies.Add(createdEnemy);
        }
    }

    /// <summary>
    /// Run a timer for a specified amount of time before setting a flag
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextGenerationCooldown()
    {
        yield return new WaitForSeconds(enemyGenerationTime);

        generationReadyFlag = true;
    }

    /// <summary>
    /// Calculates if the enemies need to scale up their stats based on the passed GameTime.
    /// Returns the conditional and also assigns a local variable if enemy scaling has changed.
    /// </summary>
    /// <returns></returns>
    private bool EnemyNeedsScaling() 
    {
        int newScaledValue = Mathf.FloorToInt(GameTimerManager.Instance.GetPreciseGameTime() / timeBetweenScalingEnemies);

        if (newScaledValue > spawnerEnemyScale) 
        {
            spawnerEnemyScale = newScaledValue;
            return true;
        }
        return false;
    }
}
