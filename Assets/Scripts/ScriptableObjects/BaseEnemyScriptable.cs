/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/18/2026
* Summary: This is the base scriptable object for all eneny scriptable objects.
* To Do:   Add more variables as needed.
*/
using UnityEngine;
using NaughtyAttributes;
[CreateAssetMenu(fileName = "BaseEnemyScriptable", menuName = "Scriptables/BaseEnemyScriptable")]
public class BaseEnemyScriptable : BaseScriptableObject
{
    public enum EnemyType
    {
        SingleShooter,
        ConeShooter,
        Melee,
        ChargingMelee,
        BuffEnemy
    }
    [SerializeField]
    public EnemyType enemyType;
    private bool isShooter() 
    {
        return enemyType == EnemyType.SingleShooter || enemyType == EnemyType.ConeShooter;
    }
    private bool isMelee() 
    {
        return enemyType == EnemyType.Melee || enemyType == EnemyType.ChargingMelee;
    }


    [SerializeField, Header("General Attributes")]
    public string enemyName;
    [SerializeField]
    public float enemyHealth;
    [SerializeField]
    public float timeBetweenAttacks;

    [ShowIf(nameof(isShooter))]
    [Header("Shooter Enemy Attributes")]
    public float bulletTravelSpeed;
    [ShowIf(nameof(isShooter))]
    [SerializeField]
    public float bulletLife;
    [ShowIf(nameof(isShooter))]
    public GameObject bulletPrefab;

    [Header("Cone Shooter Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    public int bulletsToFire;

    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    public float bulletSpreadRadiusDegrees;

    [Header("Melee Enemy Attributes")]
    [ShowIf(nameof(isMelee))]
    public float attackChargeDuration;

    [ShowIf(nameof(isMelee))]
    [Tooltip("The materials/colors that will be used for the charge-up attack. " +
        "The attack indicator will linearly interlpolate (transition) between the two colors")]
    public Material earlyChargeMaterial, endChargeMaterial;

    [ShowIf(nameof(enemyType), EnemyType.Melee)]
    public float attackUptimeDuration;

    [Header("Charging Melee Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float chargeSpeedForce;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float chargeDestinationAccuracy;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float playerHitKnockbackDuration;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float playerHitKnockbackDistance;

    [Header("Buff Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy)]
    public float buffChargeDuration;
    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy), Range(0.01f, 1)]
    public float buffPercentHealthLost;
    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy)]
    public float moveAwayDistance;



}
