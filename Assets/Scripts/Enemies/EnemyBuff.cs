using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBuff : MonoBehaviour, IEntityDataReceiver
{
    BaseEnemyScriptable enemyData;

    private MeshRenderer buffAuraMesh;
    private MeshCollider buffAuraTrigger;

    private List<Enemy> buffedEnemies;

    private EnemyWalk enemyMovement;

    private Coroutine interruptableCharge;

    private void Awake()
    {
        buffedEnemies = new List<Enemy>();
        enemyMovement = GetComponentInParent<EnemyWalk>();
        buffAuraMesh = GetComponent<MeshRenderer>();
        buffAuraTrigger = GetComponent<MeshCollider>();
    }

    public void ChargeBuff() 
    {
        StartCoroutine(CheckHealthThreshold());
        interruptableCharge = StartCoroutine(BuffFieldTimer());
    }

    public IEnumerator BuffFieldTimer() 
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

    private IEnumerator CheckHealthThreshold() 
    {
        float currentHealth = enemyData.enemyHealth;
        int deactivationThreshold = enemyData.GetLostHealthThreshold();


        while (currentHealth > deactivationThreshold)
        {
            //Debug.Log(currentHealth + " / " + deactivationThreshold);

            yield return null;

            currentHealth = enemyData.enemyHealth;
        }

        StopCoroutine(interruptableCharge);

        buffAuraTrigger.enabled = buffAuraMesh.enabled = false;

        foreach (Enemy e in buffedEnemies)
        {
            e.LoseBuff();
        }

        buffedEnemies.Clear();

        yield return StartCoroutine(enemyMovement.MoveAwayFromPlayer());

        ChargeBuff();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy)) 
        {
            enemy.ReceiveBuff(enemyData.GetATKIncreaseMultiplier(), enemyData.GetDMGReductionMultiplier());
            buffedEnemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.LoseBuff();
            buffedEnemies.Remove(enemy);
        }
    }




    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
    }
}
