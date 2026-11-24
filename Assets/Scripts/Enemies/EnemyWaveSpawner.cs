/*
* Author: Dalsten Yan
* Contributors: 
* Last Modified: 11/20/2026
* Summary: Spawner for spawning enemies in waves
*/
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyWaveSpawner : BaseSpawner
{
    [SerializeField]
    private float timeBetweenWaves = 0.5f;

    [SerializeField]
    private float timeBetweenEnemySpawns = 0.15f;

    [SerializeField]
    private int enemyScaleValue;

    [SerializeField]
    private GameObject finalEncounterWalls;

    [SerializeField]
    private List<EnemyGroup> enemyWaves;

    private Stack<EnemyGroup> enemyWavesStack;

    [SerializeField]
    private Canvas displayCanvas;

    [SerializeField]
    private TMP_Text enemiesLeftText;

    private bool playerInInteractRange;

    private Coroutine endEncounterRef;

    //[SerializeField, Header("[Debug] Current Wave Enemies")]
    //private List<GameObject> waveEnemies;

    /// <summary>
    /// Preliminary checking and grabbing of variables
    /// </summary>
    private void Start()
    {

        //For stack functionality
        enemyWaves.Reverse();
        enemyWavesStack = new Stack<EnemyGroup>(enemyWaves);

        InputPublicEvents.InteractPressed += AttemptEndEncounterInteraction;

    }

    /// <summary>
    /// Set range flag to true when player is nearby
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerInInteractRange = true;
        }
    }

    /// <summary>
    /// Set range flag to false when player isn't nearby
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerInInteractRange = false;
        }
    }

    /// <summary>
    /// Called when the player presses the interact key, only calls end encounter under specific circumstances
    /// </summary>
    public void AttemptEndEncounterInteraction()
    {
        if (playerInInteractRange && endEncounterRef == null)
        {
            playerInInteractRange = false;
            StartFinalEncounter();
        }
    }

    [Button("Manual Start Final Encounter")]

    /// <summary>
    /// Method to start the final encounter
    /// </summary>
    public void StartFinalEncounter()
    {
        if (finalEncounterWalls)
            finalEncounterWalls.SetActive(true);
        displayCanvas.enabled = true;
        endEncounterRef = StartCoroutine(SpawnNextWave());
    }

    /// <summary>
    /// Coroutine that repeatedly spawns enemies, ensuring to stagger them
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnNextWave()
    {
        var currentWaveList = enemyWavesStack.Pop().enemies;

        enemiesLeftText.text = "Enemies Left:\n" + currentWaveList.Count;

        for (int i = 0; i < currentWaveList.Count; i++)
        {
            Enemy currentEnemy = Instantiate(currentWaveList[i], spawnLocations[i].position, Quaternion.identity).GetComponent<Enemy>();
            activeEnemies.Add(currentEnemy);
            if (debugEnemyHealth)
                currentEnemy.debuggingBehavior.enabled = true;
            currentEnemy.ScaleEnemy(enemyScaleValue);
            currentEnemy.destroyCancellationToken.Register(() =>
            {
                activeEnemies.Remove(currentEnemy);
                enemiesLeftText.text = "Enemies Left:\n" + activeEnemies.Count;

                if (activeEnemies.Count <= 0 && this != null)
                {
                    StartCoroutine(CurrentWaveFinished());
                }
            });
            currentEnemy.EnableEnemy();

            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }
    }

    /// <summary>
    /// Handles loop logic for the enemy waves
    /// </summary>
    /// <returns></returns>
    private IEnumerator CurrentWaveFinished()
    {
        if (enemyWavesStack.Count <= 0)
        {
            EndGame();
            yield break;
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnNextWave());
    }

    /// <summary>
    /// End game method
    /// </summary>
    public void EndGame()
    {
        MidRunDataManager.Instance.KillPlayer();
    }
}
