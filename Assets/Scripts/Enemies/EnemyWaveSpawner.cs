using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenWaves = 0.5f;

    [SerializeField]
    private float timeBetweenEnemySpawns = 0.15f;

    [SerializeField]
    private int enemyScaleValue;

    [SerializeField]
    private List<EnemyGroup> enemyWaves;

    private Stack<EnemyGroup> enemyWavesStack;

    [SerializeField, Header("[Debug] Detected Transforms")]
    private List<Transform> spawnTransforms;

    //[SerializeField, Header("[Debug] Current Wave Enemies")]
    //private List<GameObject> waveEnemies;

    /// <summary>
    /// Preliminary checking and grabbing of variables
    /// </summary>
    private void Start()
    {
        //Get all child transforms and remove the one on this parent object
        spawnTransforms = GetComponentsInChildren<Transform>().ToList<Transform>();
        spawnTransforms.Remove(transform);

        //For stack functionality
        enemyWaves.Reverse();
        enemyWavesStack = new Stack<EnemyGroup>(enemyWaves);

    }

    [Button("Manual Start Spawner")]
    public void StartFinalEncounter() 
    {
        StartCoroutine(SpawnNextWave());
    }

    private IEnumerator SpawnNextWave() 
    {
        var currentWaveList = enemyWavesStack.Pop().enemies;

        for (int i = 0; i < currentWaveList.Count; i++)
        {
            Enemy currentEnemy = Instantiate(currentWaveList[i], spawnTransforms[i].position, Quaternion.identity).GetComponent<Enemy>();
            currentWaveList[i] = currentEnemy.gameObject;
            currentEnemy.ScaleEnemy(enemyScaleValue);
            currentEnemy.destroyCancellationToken.Register(() =>
            {
                Debug.Log("ENEMY DIED");
                currentWaveList.Remove(currentEnemy.gameObject);
                if (currentWaveList.Count <= 0 && this != null) 
                {
                    StartCoroutine(CurrentWaveFinished());
                }
            });
            currentEnemy.EnableEnemy();

            yield return new WaitForSeconds(timeBetweenEnemySpawns);
        }
    }

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

    public void EndGame() 
    {
        Debug.Log("YOU WON!");
    }
}
