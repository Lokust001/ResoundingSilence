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
        None,
        SingleShooter,
        ConeShooter,
        Melee,
        ChargingMelee,
        BuffEnemy
    }
    /// <summary>
    /// Checks if this enemy needs to show/display shooter variables
    /// </summary>
    /// <returns></returns>
    public bool IsShooter()
    {
        return enemyType == EnemyType.SingleShooter || enemyType == EnemyType.ConeShooter;
    }
    /// <summary>
    /// Checks if this enemy needs to show/display melee variables
    /// </summary>
    /// <returns></returns>
    public bool IsMelee()
    {
        return enemyType == EnemyType.Melee || enemyType == EnemyType.ChargingMelee;
    }

    /// <summary>
    /// Checks if this enemy can retreat and show the variables to do so
    /// </summary>
    /// <returns></returns>
    private bool CanRetreat() 
    {
        return enemyType == EnemyType.BuffEnemy || enemyType == EnemyType.SingleShooter || enemyType == EnemyType.ConeShooter;
    }

    /// <summary>
    /// Checks if this enemy approaches the player from different angles
    /// </summary>
    /// <returns></returns>
    private bool CanApproachPlayer() 
    {
        return IsShooter() || IsMelee();
    }

    /// <summary>
    /// Checks if this enemy either approaches or retreats
    /// </summary>
    /// <returns></returns>
    private bool HasMovementVariability() 
    {
        return CanRetreat() || CanApproachPlayer();
    }

    [Header("Enemy Type (Required)")]
    public EnemyType enemyType;


    [Header("General Attributes")]
    public string enemyName;

    public float enemyHealth;
    private float enemyMaxHealth;
    public int enemyATKDmg;
    public float timeBetweenAttacks;

    [Header("Dynamic Movement Attributes")]
    [ShowIf(nameof(HasMovementVariability))]
    public int maxVariabilityDestinationAttempts;

    [Tooltip("The spread in degrees that the enemy could randomly consider as a destination when approaching the player. " +
        "The coverage starts at the front (0 degrees) and spreads out evenly on both sides to cover the full degrees specified")]
    [ShowIf(nameof(CanApproachPlayer))]
    public int approachDestinationVariabilityDegrees;
    [ShowIf(nameof(CanRetreat))]
    public int retreatDestinationVariabilityDegrees;

    [ShowIf(nameof(CanRetreat))]
    public float retreatDistance;
    [ShowIf(nameof(CanRetreat))]
    public float retreatAccuracyThreshold;

    //Buff Enemies modify these values
    [HideInInspector]
    public float atkBoostFactor;
    [HideInInspector]
    public float dmgReductionFactor;
    

    #region Shooter & ConeShooter Variables

    [ShowIf(nameof(IsShooter))]
    [Header("Shooter Enemy Attributes")]
    [Tooltip("How fast the bullet travels in units/second")]
    public float bulletTravelSpeed;

    [ShowIf(nameof(IsShooter))]
    [Tooltip("How long the bullet lasts after being fired")]
    public float bulletLife;

    [ShowIf(nameof(IsShooter))]
    public GameObject bulletPrefab;

    [ShowIf(nameof(IsShooter))]
    public float timeBetweenRangeChecks;

    #endregion

    #region Cone Shooter Variables

    [Header("Cone Shooter Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    [Tooltip("How many bullets to fire in a cone/arc")]
    public int bulletsToFire;

    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    [Tooltip("The radius or \"area\" that the bullets will occupy in degrees ")]
    public float bulletSpreadRadiusDegrees;

    #endregion

    #region Melee & ChargingMelee Variables

    [Header("Melee Enemy Attributes")]
    [ShowIf(nameof(IsMelee))]
    [Tooltip("How fast the bullet travels in units/second")]
    public float attackChargeDuration;

    [ShowIf(nameof(IsMelee))]
    [Tooltip("The materials/colors that will be used for the charge-up attack. " +
        "The attack indicator will linearly interlpolate (transition) between the two colors")]
    public Material earlyChargeMaterial, endChargeMaterial;

    [ShowIf(nameof(enemyType), EnemyType.Melee)]
    public float attackUptimeDuration;

    #endregion

    #region ChargingMelee Variables

    [Header("Charging Melee Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float chargeSpeedForce;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float chargeDestinationAccuracy;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float playerHitKnockbackDuration;

    [ShowIf(nameof(enemyType), EnemyType.ChargingMelee)]
    public float playerHitKnockbackDistance;

    #endregion

    #region Buff Enemy Variables

    [Header("Buff Enemy Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy)]
    public float buffChargeDuration;
    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy), Range(1, 100)]
    [SerializeField]
    private int retreatHealthPercent;

    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy), Range(1, 100)]
    public int allyATKIncreasePercent;

    [ShowIf(nameof(enemyType), EnemyType.BuffEnemy), Range(1, 100)]
    public int allyReducedDmgPercent;

    #endregion

    #region Scaling Variables
    [Header("Scaling Settings")]
    public AnimationCurve healthScaling;
    public AnimationCurve damageScaling;
    #endregion

    /// <summary>
    /// Sets max health as a variable for various health-related functionality.
    /// </summary>
    private void Awake()
    {
        enemyMaxHealth = enemyHealth;
    }

    #region Helper Calculation Methods
    private const decimal PERCENTAGE_DIVISOR = 100m;

    /// <summary>
    /// Returns the health value that the enemy needs to fall below before they change
    /// their behavior
    /// </summary>
    /// <returns></returns>
    public int GetLostHealthThreshold() 
    {
        float truePercentValue = (float)(retreatHealthPercent / PERCENTAGE_DIVISOR);
        int truehealthValue = Mathf.RoundToInt(enemyMaxHealth * truePercentValue);

        return Mathf.RoundToInt(enemyHealth - truehealthValue);
    }

    /// <summary>
    /// Calculate the decimal multiplier of an ATK buff
    /// </summary>
    /// <returns></returns>
    public float GetATKIncreaseMultiplier() 
    {
        return (float)(allyATKIncreasePercent / PERCENTAGE_DIVISOR);
    }

    /// <summary>
    /// Calculate the decimal multiplier of DMG reduction buff
    /// </summary>
    /// <returns></returns>
    public float GetDMGReductionMultiplier() 
    {
        return (float)(allyReducedDmgPercent / PERCENTAGE_DIVISOR);
    }

    #endregion

    #region Enemy Scaling Methods
    /// <summary>
    /// Increases the max HP to match the animation curve's value at curveScaleFactor
    /// </summary>
    /// <param name="curveScaleFactor"></param>
    public void ScaleHP(int curveScaleFactor) 
    {
        enemyMaxHealth = healthScaling.Evaluate(curveScaleFactor);
    }
    /// <summary>
    /// Increases the DMG that the enemy deals to match the animation curve's value at curveScaleFactor
    /// </summary>
    /// <param name="curveScaleFactor"></param>
    public void ScaleDMG(int curveScaleFactor) 
    {
        enemyATKDmg = (int)damageScaling.Evaluate(curveScaleFactor);
    }

    #endregion
}
