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
    private BaseEnemyScriptable enemyData;

    private EnemyAttack attackBehavior;
    private EnemyWalk movementBeahvior;

    [SerializeField, Header("Debug Variables")]
    private int dmgToTake;

    private bool inMeleeRange;
    private bool inRangedRange;

    /// <summary>
    /// Creates a copy of the scriptable object not tied to the inspector, then propagates it other scripts
    /// </summary>
    void Awake()
    {
        attackBehavior = GetComponentInChildren<EnemyAttack>();
        movementBeahvior = GetComponentInChildren<EnemyWalk>();
        enemyData = enemyData.CreateNonRefCopy<BaseEnemyScriptable>();
        PropagateEnemyData();
        if(testWithoutSpawner)
            EnableEnemy();
    }

    private void OnEnable()
    {
        aggroTrigger.EnemyTriggerActivated += EnableEnemyAggro;
    }

    private void EnableEnemyAggro(PlayerController obj)
    {
        Debug.Log(gameObject.name + " is aggro'd");
        aggroTrigger.EnemyTriggerActivated -= EnableEnemyAggro;
        FirstEnemyAction();

    }

    /// <summary>
    /// The first decision that the enemy takes, on whether it follows 
    /// one temporal-based loop or two
    /// </summary>
    private void FirstEnemyAction() 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                break;
            case BaseEnemyScriptable.EnemyType.SingleShooter:
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                StartCoroutine(RepeatedlyCheckShooterRange());
                break;
            case BaseEnemyScriptable.EnemyType.Melee:
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                movementBeahvior.StartFollowingPlayer();
                break;
            case BaseEnemyScriptable.EnemyType.BuffEnemy:
                GetComponentInChildren<EnemyBuff>(true).AttemptBuffingAllies();
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        aggroTrigger.EnemyTriggerActivated -= EnableEnemyAggro;
    }

    /// <summary>
    /// The nth decision that the enemy takes, dictated by which range it is currently present in
    /// </summary>
    public void NextEnemyAction() 
    {
        if (!inRangedRange && !inMeleeRange)
        {
            NotInPlayerZones();
        }
        else if (inRangedRange)
        {
            RangedZoneEntryBehaviors(true);
        }
        else if (inMeleeRange)
        {
            MeleeZoneEntryBehaviors(true);
        }
    }

    /// <summary>
    /// Temporal-based loop that only activates the next decision after a specified amount of time
    /// </summary>
    /// <returns></returns>
    private IEnumerator RepeatedlyCheckShooterRange() 
    {
        NextEnemyAction();
        yield return new WaitForSeconds(enemyData.timeBetweenRangeChecks);
        StartCoroutine(RepeatedlyCheckShooterRange());
    }

    /// <summary>
    /// The decision to track the player (for all but one enemy) when they are not in range
    /// </summary>
    private void NotInPlayerZones() 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.BuffEnemy:
                //Intentionally left blank to avoid default case
                break;
            default:
                movementBeahvior.StartFollowingPlayer();
                break;
        }
    }

    public void SetInRangedArea(bool value) 
    {
        inRangedRange = value;
        //If the player entered
        if(inRangedRange)
            RangedZoneEntryBehaviors(false);
    }

    private void RangedZoneEntryBehaviors(bool activateTimeBasedIntervalActions) 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.SingleShooter:
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                //Entry Behavior
                movementBeahvior.EndPlayerSearch();
                break;
            case BaseEnemyScriptable.EnemyType.Melee:
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                //if(activateTimeBasedIntervalActions)
                //    movementBeahvior.StartFollowingPlayer();
                break;
            default:
                break;
        }
    }

    public void SetInMeleeArea(bool value) 
    {
        inRangedRange = !value;
        inMeleeRange = value;
        //If the player entered
        if (inMeleeRange)
            MeleeZoneEntryBehaviors(false);
    }

    private void MeleeZoneEntryBehaviors(bool activateTimeBasedIntervalActions) 
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.None:
                Debug.LogError("Invalid EnemyType entered Zone!");
                break;
            case BaseEnemyScriptable.EnemyType.SingleShooter:
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                movementBeahvior.EndPlayerSearch();
                //Only do this if a time-based check has passed via RepeatedlyCheckShooterRange
                if(activateTimeBasedIntervalActions)
                    StartCoroutine(movementBeahvior.MoveAwayFromPlayer());
                break;
            case BaseEnemyScriptable.EnemyType.Melee:
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                movementBeahvior.EndPlayerSearch();
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
        SetInMeleeArea(false);
        SetInRangedArea(false);
        StopAllCoroutines();
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

