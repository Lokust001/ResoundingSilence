/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/29/2026
* Summary: This is the base scriptable object for all eneny scriptable objects.
* To Do:   Add more variables as needed.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuff : MonoBehaviour, IEntityDataReceiver, ICustomDisabler
{
    BaseEnemyScriptable enemyData;

    private MeshRenderer buffAuraMesh;
    private MeshCollider buffAuraTrigger;

    private List<Enemy> buffedEnemies;

    private EnemyWalk enemyMovement;

    private Coroutine interruptableCharge;

    /// <summary>
    /// Grabs necessary components and instantiates a new List
    /// </summary>
    private void InitializeVariables()
    {
        buffedEnemies = new List<Enemy>();
        enemyMovement = GetComponentInParent<EnemyWalk>();
        buffAuraMesh = GetComponent<MeshRenderer>();
        buffAuraTrigger = GetComponent<MeshCollider>();
    }

    /// <summary>
    /// Public method that can be activated from any script to start buff behavior and keep tabs on health
    /// </summary>
    public void AttemptBuffingAllies()
    {
        StartCoroutine(MonitorAndRetreat());
        interruptableCharge = StartCoroutine(ChargingBuff());
    }

    /// <summary>
    /// Coroutine that counts up time until the buff collider is active and can be seen
    /// </summary>
    /// <returns></returns>
    public IEnumerator ChargingBuff()
    {
        float timer = 0;
        float goal = enemyData.buffChargeDuration;

        while (timer < goal)
        {
            yield return null;
            timer += Time.deltaTime;
        }

        buffAuraTrigger.enabled = buffAuraMesh.enabled = true;
    }

    /// <summary>
    /// Semi-Looped method that simulates retreat behavior
    /// </summary>
    /// <returns></returns>
    private IEnumerator MonitorAndRetreat()
    {
        float currentHealth = enemyData.enemyHealth;
        int deactivationThreshold = enemyData.GetLostHealthThreshold();

        //Coroutine loops and consistently checks health until it falls below threshold
        while (currentHealth > deactivationThreshold)
        {
            yield return null;

            currentHealth = enemyData.enemyHealth;
        }

        //Stop charing up the buff if it is active
        StopCoroutine(interruptableCharge);

        ClearBuffFromAllies();

        //Wait for the enemy to move away from the player
        yield return StartCoroutine(enemyMovement.MoveAwayFromPlayer());

        //Starts the process all over again
        AttemptBuffingAllies();
    }

    /// <summary>
    /// Makes every ally affected by the buff lose their buff
    /// </summary>
    private void ClearBuffFromAllies()
    {
        //Turn off the aura visual and prevent new enemies from entering it
        buffAuraTrigger.enabled = buffAuraMesh.enabled = false;

        //Make every enemy that was buffed lose their buff
        foreach (Enemy e in buffedEnemies)
        {
            e.LoseBuff();
        }

        //Discard currently tracked buffed enemies since they are no longer buffed
        buffedEnemies.Clear();
    }

    /// <summary>
    /// Give the buff to the enemy that just walked into the aura radius
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.ReceiveBuff(enemyData.GetATKIncreaseMultiplier(), enemyData.GetDMGReductionMultiplier());
            buffedEnemies.Add(enemy);
        }
    }

    /// <summary>
    /// Remove the buff from the enemy that just walked out of the aura radius
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.LoseBuff();
            buffedEnemies.Remove(enemy);
        }
    }

    /// <summary>
    /// Casts the scriptable object into a BaseEnemyScriptable and assigns the data of the enemy to this script
    /// </summary>
    /// <param name="baseScriptable"></param>
    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
        InitializeVariables();
    }

    /// <summary>
    /// Specifies which method to run to disable the functionality of this enemy when it is called to be disabled
    /// </summary>
    public void DisableEntity()
    {
        StopAllCoroutines();
        ClearBuffFromAllies();
    }
}
