/* Author: Dalsten Yan
 * Contributors:
 * Last Modified: 9/28/26
 * Summary: Handles most of the player attacking logic for each type of enemy
 * TODO: More as needed
 */
using System.Collections;
using UnityEngine;
using static BaseEnemyScriptable;

public class EnemyAttack : MonoBehaviour, IEntityDataReceiver, ICustomEnabler, ICustomDisabler
{

    [SerializeField]
    private TriggerObjects[] onTriggerEnterObjects;

    [SerializeField]
    private GameObject attackProjectilePrefab;

    [SerializeField]
    private GameObject aoeAttack;

    [SerializeField]
    private GameObject attackIndicatorArea;

    private SphereCollider sphereTrigger;
    private CapsuleCollider chargingCapCollider;
    private EnemyWalk enemyMovement;
    private Enemy baseEnemyBehavior;
    private Transform playerTransform;
    private Vector3 fireBulletDirection;
    private Coroutine activeATKorCooldown;
    private Coroutine enemyCharging;

    private IEnumerator queuedMeleeAttack;

    private bool inAttackRange;
    private bool isAggro;

    private BaseEnemyScriptable enemyData;
    /// <summary>
    /// Grabs the sphere collider for the search radius and enemy movement script
    /// </summary>
    private void InitializeVariables() 
    {
        sphereTrigger = GetComponent<SphereCollider>();
        enemyMovement = GetComponentInParent<EnemyWalk>();
        baseEnemyBehavior = GetComponentInParent<Enemy>();
        chargingCapCollider = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// For each child trigger, make HandleVariousAttackTriggers a listener to handle the event
    /// </summary>
    private void OnEnable()
    {
        foreach (var trigger in onTriggerEnterObjects) 
        {
            trigger.EnemyTriggerActivated += HandleVariousAttackTriggers;
        }
    }

    /// <summary>
    /// For each child trigger, remove HandleVariousAttackTriggers as a listener
    /// </summary>
    private void OnDisable()
    {
        foreach (var trigger in onTriggerEnterObjects)
        {
            trigger.EnemyTriggerActivated -= HandleVariousAttackTriggers;
        }
    }

    /// <summary>
    /// Makes the player take damage after running into the triggers
    /// (can be changed in the future)
    /// </summary>
    /// <param name="other"></param>
    private void HandleVariousAttackTriggers(PlayerController player) 
    {
        player.TakeDamage();
    }

    /// <summary>
    /// Start following the player when they enter aggro range
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            playerTransform = other.transform;
        }
    }

    /// <summary>
    /// Mark that the player is no longer in attack range
    /// </summary>
    /// <param name="other"></param>
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.GetComponent<PlayerController>())
    //    {
    //        inAttackRange = false;
    //    }
    //}



    /// <summary>
    /// Method to handle the different types of attacks that various enemies can do or different enemy types in general alongside priming cooldowns
    /// </summary>
    public void InitiateAttack() 
    {
        //If there is already a cooldown or ability active, don't start another one
        if (activeATKorCooldown != null) return;

        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.SingleShooter:
                FireBullet(Quaternion.identity, true);
                break;
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                FireBulletsCone();
                break;
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                float chargingMeleeDistance = attackIndicatorArea.transform.localPosition.z;
                queuedMeleeAttack = enemyMovement.ChargeTowardsLocation(chargingMeleeDistance);
                activeATKorCooldown = StartCoroutine(AtkIndicatorWindupAndAction());
                return;
            case BaseEnemyScriptable.EnemyType.Melee:
                queuedMeleeAttack = PerformMeleeAttack();
                activeATKorCooldown = StartCoroutine(AtkIndicatorWindupAndAction());
                return;
            default:
                break;
        }
        //However, if the action was near instantaneous or didn't need a coroutine, start a catch-all cooldown for them instead
        activeATKorCooldown ??= StartCoroutine(AttackCooldownCoroutine());
    }

    public void CustomStartCooldown() 
    {
        StartCoroutine(AttackCooldownCoroutine());
    }


    /// <summary>
    /// Universal coroutine method called that handles cooldowns or stationary logic after an attack for each enemy.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCooldownCoroutine() 
    {
        Debug.Log("Started Cooldown");
        //Wait for the time between attacks, then clear the active coroutine
        yield return new WaitForSeconds(enemyData.timeBetweenAttacks);
        activeATKorCooldown = null;
        Debug.Log("Ended Cooldown");

        AfterCooldownDecision();
        
    }

    private void AfterCooldownDecision() 
    {
        switch (enemyData.enemyType)
        {
            case EnemyType.None:
                break;
            case EnemyType.SingleShooter:
            case EnemyType.ConeShooter:
                InitiateAttack();
                break;
            case EnemyType.Melee:
            case EnemyType.ChargingMelee:
                baseEnemyBehavior.NextEnemyAction();
                break;
            case EnemyType.BuffEnemy:
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    /// Fires a bullet in the direction of the player, with its angle modified by a provided bulletRotation
    /// </summary>
    /// <param name="bulletRotation"></param>
    /// <param name="isSingleShot"></param>
    private void FireBullet(Quaternion bulletRotation, bool isSingleShot = false) 
    {
        
        //Calculate direction of movement and normalize it
        Vector3 bulletTravelDirection = bulletRotation * (isSingleShot ? CalculateBulletDirection() : fireBulletDirection);

        //Instantiate bullet with the direction, travelspeed, lifetime, and give it the enemy position (can be changed later)
        Instantiate(attackProjectilePrefab).GetComponentInChildren<BulletBehavior>().Init(bulletTravelDirection, enemyData.bulletTravelSpeed, enemyData.bulletLife, transform.position);
    }
    /// <summary>
    /// Helper method to provide the Vector3 direction that the player is at
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateBulletDirection() 
    {
        return (playerTransform.position - transform.position).normalized;
    }

    /// <summary>
    /// Fires multiple bullets in a cone/arc shape around the enemy. Can handle even or odd bullet counts.
    /// </summary>
    private void FireBulletsCone() 
    {
        fireBulletDirection = CalculateBulletDirection();

        //Divvy up desired spread radius and desired bullets to fire into sections
        float currentRotationDegrees = enemyData.bulletSpreadRadiusDegrees / 2.0f;
        //Bool variable for ternary operator clarity
        bool isEven = enemyData.bulletsToFire % 2 == 0;

        //How big the sections will encompass. Even bullets will cover the entire radius. Odd bullets will grab half of that.
        float sectionRadius = enemyData.bulletSpreadRadiusDegrees / (isEven ? 1 : 2.0f);

        //How many total sections will be divvied up. Even bullets takes n - 1 sections for no middle bullet and proper distribution.
        //Odd bullets will grab sections equal to half of their count rounded down.
        float totalSections = isEven ? enemyData.bulletsToFire - 1 : Mathf.FloorToInt(enemyData.bulletsToFire / 2.0f);

        //Degrees that each bullet will deviate from each other
        //For odd bullet counts, it will be half the radius divided by the half the number of bullets rounded down so that a middle bullet can be produced
        //For even bullet counts, it will be the entire radius divided by the full number of bullets
        float bulletDegreeStep = sectionRadius / totalSections;


        for (int i = 0; i < enemyData.bulletsToFire; i++) 
        {
            //Modify the original straight direction the bullet is going to be travlled by a Euler rotation
            Quaternion currentBulletRotation = Quaternion.Euler(0, currentRotationDegrees, 0);

            //Create the bullet with the modified travel direction
            FireBullet(currentBulletRotation);

            //Use a new degree direction by decrementing the current rotation by the nearly uniform step angle
            currentRotationDegrees -= bulletDegreeStep;
        }
    }

    /// <summary>
    /// A melee attack Coroutine that handles both the visual and logical side of a charging attack. The enemy will charge up first
    /// via an indicator that extends. The indicator then disappears and the enemy will launch itself very fast in that direction.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AtkIndicatorWindupAndAction() 
    {
        //Retrieve the MeshRenderer from the attack indicator
        MeshRenderer indicatorRenderer = attackIndicatorArea.GetComponent<MeshRenderer>();

        //Get the parent of the attack indicator as it scales/extends the indicator
        Transform indicatorParentScaler = attackIndicatorArea.transform.parent;

        //Local cache variables for performance boost and code readability
        float countupTimer = 0;
        float goalTime = enemyData.attackChargeDuration;

        Material startMat = enemyData.earlyChargeMaterial;
        Material endMat = enemyData.endChargeMaterial;

        Transform enemyTransform = enemyMovement.transform;

        //Count-up timer to LERP for both the material and parent-scale
        while (countupTimer < goalTime) 
        {
            //Timer tick & associated time values
            countupTimer += Time.deltaTime;
            float tValue = countupTimer / goalTime;

            //Lock-on to player direction while charging up
            enemyTransform.LookAt(playerTransform);
            //Transition between two colors/mats
            indicatorRenderer.material.Lerp(startMat, endMat, tValue);
            //Extend the visual attack indicator through the parent as it is the pivot point
            indicatorParentScaler.localScale = new Vector3(1, 1, tValue);

            yield return new WaitForFixedUpdate();
        }
        //The parent's scale is mostly irrelevant, it acts as the pivot for the actual indicator.
        //Setting the z scale to 0 makes the indicator invisible, and increasing the z scale
        //over time makes the actual indicator "grow" over time.
        ResetIndicatorParent();

        //Pass the Charging behavior towards enemyMovement to handle the sudden burst of movement
        enemyCharging = StartCoroutine(queuedMeleeAttack);
        yield return enemyCharging;
        enemyCharging = null;

        //Start a Cooldown Coroutine
        activeATKorCooldown = StartCoroutine(AttackCooldownCoroutine());
    }

    /// <summary>
    /// Resets the parent indicator scale so that that the indicator isn't showing
    /// </summary>
    private void ResetIndicatorParent() 
    {
        if(attackIndicatorArea != null)
            attackIndicatorArea.transform.parent.localScale = new(1, 1, 0);
        if (aoeAttack != null)
            aoeAttack.SetActive(false);

    }

    /// <summary>
    /// Sets the AOE object to active for the spcified amount of time in enemyData, then turns it off
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformMeleeAttack() 
    {
        yield return null;
        aoeAttack.SetActive(true);

        float timer = 0;
        float goalDuration = enemyData.attackUptimeDuration;
        while (timer < goalDuration)
        {
            yield return new WaitForFixedUpdate();

            timer += Time.fixedDeltaTime;
        }

        aoeAttack.SetActive(false);
    }

    /// <summary>
    /// Attack method that activates and pushes an object attached to the enemy a short distance forward over time
    /// </summary>
    /// <param name="attackTravelDistance"></param>
    /// <returns></returns>
    private IEnumerator PerformTravelingProjectileAttack(float attackTravelDistance) 
    {
        attackProjectilePrefab.SetActive(true);

        Transform meleeAtkTransform = attackProjectilePrefab.transform;

        Vector3 initialAOEOffsetPosition = meleeAtkTransform.localPosition;

        Vector3 attackGoalPosition = (attackTravelDistance * Vector3.forward) + initialAOEOffsetPosition;

        float timer = 0;
        float goalDuration = enemyData.attackUptimeDuration;

        float evenSpeed = attackTravelDistance / goalDuration;


        while (timer < goalDuration) 
        {
            yield return new WaitForFixedUpdate();

            timer += Time.fixedDeltaTime;

            meleeAtkTransform.localPosition = Vector3.MoveTowards(meleeAtkTransform.localPosition, attackGoalPosition, evenSpeed * Time.fixedDeltaTime);
        }

        meleeAtkTransform.localPosition = attackGoalPosition;
        yield return null;
        attackProjectilePrefab.SetActive(false);
        meleeAtkTransform.localPosition = initialAOEOffsetPosition;
    }

    public bool IsEnemyOnCooldown() 
    {
        return activeATKorCooldown != null;
    }

    /// <summary>
    /// Helper method for ChargingEnemies, indicates if they are actively running down their charged attack
    /// </summary>
    /// <returns></returns>
    public bool IsEnemyCurrentlyCharging() 
    {
        return enemyCharging != null;
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
    /// Specifies whcih triggers and colliders to enable when this enemy is enabled
    /// </summary>
    public void EnableEntity()
    {
        sphereTrigger.enabled = true;
        if(chargingCapCollider != null)
            chargingCapCollider.enabled = true;
    }

    /// <summary>
    /// Specifies which methods, coroutines, and triggers/colliders to disable when this enemy is disabled
    /// </summary>
    public void DisableEntity()
    {
        StopAllCoroutines();
        ResetIndicatorParent();
        activeATKorCooldown = enemyCharging = null;
        sphereTrigger.enabled = false;

        if (chargingCapCollider != null)
            chargingCapCollider.enabled = false;
    }

    
}
