using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField]
    private float enemyBulletSpeed;
    [SerializeField]
    private int bulletsToFire;
    [SerializeField]
    private float bulletSpreadRadiusDegrees;
    [SerializeField]
    private float bulletLifetime;

    [SerializeField]
    private float attackSpeed;

    [SerializeField]
    private GameObject bulletPrefab;

    private SphereCollider sphereTrigger;
    private EnemyWalk seekingBehavior;
    private Transform playerTransform;
    

    public BaseEnemyScriptable enemyScriptableObject;
    void Start()
    {
        enemyScriptableObject = enemyScriptableObject.CreateNonRefCopy<BaseEnemyScriptable>();
        sphereTrigger = GetComponent<SphereCollider>();
        seekingBehavior = GetComponentInParent<EnemyWalk>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null) 
        {
            seekingBehavior.EndPlayerSearch();
            playerTransform = transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            InitiateAttack();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            seekingBehavior.StartPlayerSearch();
        }
    }

    private void InitiateAttack() 
    {
        //Cooldown logic here

        //End Cooldown logic
        switch (enemyScriptableObject.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.SingleShooter:
                FireBullet();
                break;
            case BaseEnemyScriptable.EnemyType.ConeShooter:
                FireBulletsCone();
                break;
            default:
                break;
        }
    }

    private void FireBullet() 
    {
        Vector3 originalTravelDirection = (playerTransform.position - transform.position).normalized;
        Instantiate(bulletPrefab).GetComponent<BulletBehavior>().Init(originalTravelDirection, enemyBulletSpeed, bulletLifetime, transform.position);
        
    }

    private void FireBulletsCone() 
    {
        Vector3 originalTravelDirection = (playerTransform.position - transform.position).normalized;

        //Divvy up desired spread radius and desired bullets to fire into sections
        float currentRotationDegrees = bulletSpreadRadiusDegrees / 2.0f;
        //Bool variable for ternary operator clarity
        bool isEven = bulletsToFire % 2 == 0;

        //How big the sections will encompass. Even bullets will cover the entire radius. Odd bullets will grab half of that.
        float sectionRadius = bulletSpreadRadiusDegrees / (isEven ? 1 : 2.0f);

        //How many total sections will be divvied up. Even bullets takes n - 1 sections for no middle bullet and proper distribution.
        //Odd bullets will grab sections equal to half of their count rounded down.
        float totalSections = isEven ? bulletsToFire - 1 : Mathf.FloorToInt(bulletsToFire / 2.0f);

        //Degrees that each bullet will deviate from each other
        //For odd bullet counts, it will be half the radius divided by the half the number of bullets rounded down so that a middle bullet can be produced
        //For even bullet counts, it will be the entire radius divided by the full number of bullets
        float bulletDegreeStep = sectionRadius / totalSections;


        for (int i = 0; i < bulletsToFire; i++) 
        {
            //Modify the original straight direction the bullet is going to be travlled by a Euler rotation
            Vector3 modifiedTravelDirection = Quaternion.Euler(0, currentRotationDegrees, 0) * originalTravelDirection;
            
            //Create the bullet with the modified travel direction
            Instantiate(bulletPrefab).GetComponent<BulletBehavior>().Init(modifiedTravelDirection, enemyBulletSpeed, bulletLifetime, transform.position);

            //Use a new degree direction by decrementing the current rotation by the nearly uniform step angle
            currentRotationDegrees -= bulletDegreeStep;
        }
    }




}
