/* Author: Dalsten Yan
 * Last Modified: 9/28/26
 * Summary: Supervising enemy script that decides non-physics behaviors & shares enemyData among other scripts
 * TODO: More as needed
 */

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private TriggerObjects aggroTrigger;

    [SerializeField]
    private bool testWithoutSpawner;

    [SerializeField]
    private bool testAlternateShooterBehavior;

    [SerializeField]
    private BaseEnemyScriptable enemyData;

    private EnemyAttack attackBehavior;
    private EnemyWalk movementBeahvior;

    [SerializeField, Header("Debug Variables")]
    private int dmgToTake;

    private bool inMeleeRange;
    private bool inRangedRange;

    private bool firstAction;

    /// <summary>
    /// Creates a copy of the scriptable object not tied to the inspector, then propagates it other scripts
    /// </summary>
    void Awake()
    {
        firstAction = false;
        attackBehavior = GetComponentInChildren<EnemyAttack>();
        movementBeahvior = GetComponentInChildren<EnemyWalk>();
        enemyData = enemyData.CreateNonRefCopy<BaseEnemyScriptable>();
        PropagateEnemyData();
        if(testWithoutSpawner)
            EnableEnemy();
    }

    private void OnEnable()
    {
        if(testAlternateShooterBehavior)
            aggroTrigger.EnemyTriggerActivated += EnableEnemyAggro;
    }

    private void EnableEnemyAggro(PlayerController obj)
    {
        aggroTrigger.EnemyTriggerActivated -= EnableEnemyAggro;
        StartCoroutine(RepeatedlyCheckRange());
        attackBehavior.CustomStartCooldown();

    }

    private void OnDisable()
    {
        if (testAlternateShooterBehavior)
            aggroTrigger.EnemyTriggerActivated -= EnableEnemyAggro;
    }

    public void NextEnemyAction() 
    {
        if (!inRangedRange && !inMeleeRange)
        {
            OutOfRangeBehavior();
        }
        else if (inRangedRange)
        {
            RangedZoneAction();
        }
        else if (inMeleeRange)
        {
            MeleeZoneAction();
        }
    }

    private IEnumerator RepeatedlyCheckRange() 
    {
        yield return new WaitForSeconds(enemyData.timeBetweenRangeChecks);
        NextEnemyAction();
        StartCoroutine(RepeatedlyCheckRange());
    }

    /// <summary>
    /// Dictates what first action the enemy should take when it is not near the player
    /// </summary>
    private void OutOfRangeBehavior() 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.BuffEnemy:
                GetComponentInChildren<EnemyBuff>(true).AttemptBuffingAllies();
                break;
            default:
                movementBeahvior.StartFollowingPlayer();
                break;
        }
    }

    public bool GetTestAlternateBehavior() 
    {
        return testAlternateShooterBehavior;
    }

    public void SetInRangedZone(bool value) 
    {
        inRangedRange = value;
        if (!testAlternateShooterBehavior && !attackBehavior.IsEnemyOnCooldown()) 
        {
            NextEnemyAction();
        }
    }

    private void RangedZoneAction() 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.SingleShooter:
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                movementBeahvior.EndPlayerSearch();
                if (!testAlternateShooterBehavior)
                    attackBehavior.InitiateAttack();
                break;
            default:
                break;
        }
    }

    public void SetInMeleeZone(bool value) 
    {
        inRangedRange = !value;
        inMeleeRange = value;
        if (!testAlternateShooterBehavior && !attackBehavior.IsEnemyOnCooldown())
        {
            NextEnemyAction();
        }

    }

    private void MeleeZoneAction() 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.SingleShooter:
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                movementBeahvior.EndPlayerSearch();
                StartCoroutine(movementBeahvior.MoveAwayFromPlayer());
                break;
            case BaseEnemyScriptable.EnemyType.Melee:
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                attackBehavior.InitiateAttack();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sends the enemyData variable to every script that has the IEntityDataReceiver interface
    /// </summary>
    void PropagateEnemyData() 
    {
        foreach (IEntityDataReceiver entity in GetComponentsInChildren<IEntityDataReceiver>(true))
        {
            entity.SetEntityData(enemyData);
        }
    }

    /// <summary>
    /// Calls the EnableEntity method on all enemy scripts with ICustomEnabler
    /// </summary>
    public void EnableEnemy() 
    {
        foreach (ICustomEnabler entity in GetComponentsInChildren<ICustomEnabler>(true))
        {
            entity.EnableEntity();
        }
        if(!testAlternateShooterBehavior)
            OutOfRangeBehavior();
        
    }

    /// <summary>
    /// Calls the DisableEntity method on all enemy scripts with ICustomDisabler
    /// </summary>
    public void DisableEnemy() 
    {
        foreach (ICustomDisabler entity in GetComponentsInChildren<ICustomDisabler>(true))
        {
            entity.DisableEntity();
        }
        SetInMeleeZone(false);
        SetInRangedZone(false);
    }

    /// <summary>
    /// Increases two of enemyData's variables that control the output of damage and input of received damage
    /// by setting them to their specified values
    /// </summary>
    /// <param name="atkModifier"></param>
    /// <param name="dmgModifier"></param>
    public void ReceiveBuff(float atkModifier, float dmgModifier) 
    {
        enemyData.atkBoostFactor = atkModifier;
        enemyData.dmgReductionFactor = dmgModifier;

        Debug.Log("I've been buffed! My attacks now deal: " + EnemyDealDamage() + "dmg, and if an attack hits me that is " + dmgToTake + " dmg, I only take " + EnemyTakeDamage(dmgToTake));
    }

    /// <summary>
    /// Reduces two of enemyData's variables that control the output of damage and input of received damage
    /// by reducing them to 0, so they have no effect
    /// </summary>
    public void LoseBuff() 
    {
        enemyData.atkBoostFactor = enemyData.dmgReductionFactor = 0;

        Debug.Log("I've lost my buff! My attacks now deal: " + EnemyDealDamage() + "dmg, and if an attack hits me that is " + dmgToTake + " dmg, I only take " + EnemyTakeDamage(dmgToTake));
    }

    /// <summary>
    /// Method that calculates enemy damage taken while simultaenously updating enemyData with those values
    /// </summary>
    /// <param name="damageToTake"></param>
    /// <returns></returns>
    public int EnemyTakeDamage(float damageToTake)
    {
        int finalDmgTaken = Mathf.RoundToInt(damageToTake - (damageToTake * enemyData.dmgReductionFactor));
        enemyData.enemyHealth -= finalDmgTaken;
        return finalDmgTaken;
    }

    /// <summary>
    /// Method that calculates the amount of damage the enemy does
    /// </summary>
    /// <returns></returns>
    public int EnemyDealDamage() 
    {
        return Mathf.RoundToInt(enemyData.enemyATKDmg + (enemyData.enemyATKDmg * enemyData.atkBoostFactor));
    }



    /// <summary>
    /// Debug Attack enemy while attacking doesn't exist yet
    /// </summary>
    [Button("Dmg to Enemy")]
    public void DebugDealDamage() 
    {
        EnemyTakeDamage(dmgToTake);
        
    }
    /// <summary>
    /// Simulates an enemydeath
    /// </summary>

    [Button("EnemyDeath")]
    public void EnemyDeath() 
    {
        Destroy(gameObject);
    }
}

