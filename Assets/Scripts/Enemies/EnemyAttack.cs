using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour, IEntityDataReceiver
{

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private GameObject chargingATKParent;

    private SphereCollider sphereTrigger;
    private EnemyWalk enemyMovement;
    private Transform playerTransform;
    private Vector3 fireBulletDirection;
    private Coroutine activeATKorCooldown;

    private bool inAttackRange;

    public BaseEnemyScriptable enemyData;
    void Start()
    {
        sphereTrigger = GetComponent<SphereCollider>();
        enemyMovement = GetComponentInParent<EnemyWalk>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null) 
        {
            enemyMovement.EndPlayerSearch();
            inAttackRange = true;
            playerTransform = other.transform;
            InitiateAttack();

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inAttackRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inAttackRange = false;
        }
    }

    

    /// <summary>
    /// Method to handle the different types of attacks that various enemies can do or different enemy types in general alongside priming cooldowns
    /// </summary>
    private void InitiateAttack() 
    {
        
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.SingleShooter:
                FireBullet(Quaternion.identity, true);
                break;
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                FireBulletsCone();
                break;
            case BaseEnemyScriptable.EnemyType.ChargingMelee:
                activeATKorCooldown ??= StartCoroutine(WindupChargerAttack());
                break;
            case BaseEnemyScriptable.EnemyType.Melee:
                break;
            default:
                break;
        }
        //If there is already a cooldown or ability active, don't start another one
        if (activeATKorCooldown != null) return;
        //However, if the action was near instantaneous or didn't need a coroutine, start a catch-all cooldown for them instead
        activeATKorCooldown = StartCoroutine(AttackCooldownCoroutine());
    }

    /// <summary>
    /// Universal coroutine method called that handles cooldowns or stationary logic after an attack for each enemy.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackCooldownCoroutine() 
    {
        //Wait for the time between attacks, then clear the active coroutine
        yield return new WaitForSeconds(enemyData.timeBetweenAttacks);
        activeATKorCooldown = null;

        //If player is not in attack range, start searching for it
        if (!inAttackRange)
        {
            Debug.Log("Not in range, starting search");
            enemyMovement.StartPlayerSearch();
        }

        //Otherwise, initiate another attack
        else 
        {
            Debug.Log("Still in range, initiating attack");
            InitiateAttack();
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
        Instantiate(bulletPrefab).GetComponentInChildren<BulletBehavior>().Init(bulletTravelDirection, enemyData.bulletTravelSpeed, enemyData.bulletLife, transform.position);
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
    private IEnumerator WindupChargerAttack() 
    {
        //Retrieve the MeshRenderer from the attack indicator from having access to its parent
        MeshRenderer chargeIndicator = chargingATKParent.GetComponentInChildren<MeshRenderer>();
        //Retriever the innate/design-specified length of the chargeIndicator to influence the charging distance
        float chargeIndicatorZlength = chargeIndicator.transform.localPosition.z;

        //Local cache variables for performance boost and code readability
        float countupTimer = 0;
        float goalTime = enemyData.chargingDirectionTime;

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
            chargeIndicator.material.Lerp(startMat, endMat, tValue);
            //Extend the visual attack indicator
            chargingATKParent.transform.localScale = new Vector3(1, 1, tValue);

            yield return new WaitForFixedUpdate();
        }
        //The parent's scale is mostly irrelevant, it acts as the pivot for the actual indicator.
        //Setting the z scale to 0 makes the indicator invisible, and increasing the z scale
        //over time makes the actual indicator "grow" over time.
        chargingATKParent.transform.localScale = new(1, 1, 0);

        //Pass the Charging behavior towards enemyMovement to handle the sudden burst of movement
        yield return StartCoroutine(enemyMovement.ChargeTowardsLocation(chargeIndicatorZlength));

        inAttackRange = false;

        //Start a Cooldown Coroutine
        activeATKorCooldown = StartCoroutine(AttackCooldownCoroutine());
    }

    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
    }
}
