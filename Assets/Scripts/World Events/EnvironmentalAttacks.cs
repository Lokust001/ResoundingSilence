/*
* Author: Brenden
* Contributors:
* Last Modified: 09/23/2026
* Summary: Spawns in the environmental attacks that are in the final fight
* To Do:   
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalAttacks : MonoBehaviour
{
    [SerializeField] private GameObject AttackPrefab;
    [SerializeField] private List<GameObject> VerticalSpawnLoactaions;
    [SerializeField] private List<GameObject> HorizontalSpawnLoactaions;
    [SerializeField] private float TimeInbetweenAttacks;
    private bool horizonatal = true;
    public Coroutine attackCoroutineInstance;

    /// <summary>
    /// temp call to start the attack coroutine
    /// </summary>
    private void Start()
    {
        attackCoroutineInstance = StartCoroutine(AttacksStart());
    }

    /// <summary>
    /// runs this forever and spawns attacks that the player has to avoid
    /// </summary>
    /// <returns></returns>
    public IEnumerator AttacksStart()
    {
        while (true)
        {
            if (horizonatal)
            {
                int spawnPoint = Random.Range(0, HorizontalSpawnLoactaions.Count);
                GameObject temp = Instantiate(AttackPrefab, HorizontalSpawnLoactaions[spawnPoint].transform.position, HorizontalSpawnLoactaions[spawnPoint].transform.rotation);
                StartCoroutine(temp.GetComponent<WorldAttack>().windup());
            }
            else
            {
                int spawnPoint = Random.Range(0, HorizontalSpawnLoactaions.Count);
                GameObject temp = Instantiate(AttackPrefab, VerticalSpawnLoactaions[spawnPoint].transform.position, VerticalSpawnLoactaions[spawnPoint].transform.rotation);
                StartCoroutine(temp.GetComponent<WorldAttack>().windup());
            }
            horizonatal = !horizonatal;
            yield return new WaitForSeconds(TimeInbetweenAttacks);

        }
    }
}
