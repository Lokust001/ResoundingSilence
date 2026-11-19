/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/19/2026
* Summary: The base spawner behavior shared by multiple types of enemy spwaners
* To Do:   
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    protected bool debugEnemyHealth;
    [SerializeField]
    private GameObject enemySpawnsParent;


    [Header("[Debug] Don't edit these fields")]
    public List<Enemy> activeEnemies;
    [SerializeField]
    protected List<Transform> spawnLocations;


    /// <summary>
    /// Sets up the spawn locations and generated enemies container
    /// </summary>
    private void Awake()
    {
        activeEnemies = new();
        enemySpawnsParent = enemySpawnsParent != null ? enemySpawnsParent : gameObject;
        spawnLocations = enemySpawnsParent.GetComponentsInChildren<Transform>().ToList();
        spawnLocations.Remove(enemySpawnsParent.transform);
    }

    /// <summary>
    /// Tells all current and future spawners to start making enemies debug their health info
    /// </summary>
    public void EnableEnemyHealthDebugging()
    {
        debugEnemyHealth = true;
        foreach (var existingEnemy in activeEnemies)
        {
            existingEnemy.debuggingBehavior.enabled = true;
        }
    }
}
