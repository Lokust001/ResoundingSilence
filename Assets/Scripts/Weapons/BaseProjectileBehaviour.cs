/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/11/2026
 * Brief: Handles projectile collision and lifetime
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseProjectileBehaviour : MonoBehaviour
{
    BurstData attackRef;
    int comboIndex;
    int pierceDamageIndex;
    int pierceLifetimeIndex;

    int myDamage;
    float myLifetime;
    int baseDamage;
    float baseLifetime;

    int ricochetCount;
    bool hitEnemy;

    [SerializeField] List<GameObject> enemiesInRange = new List<GameObject>();
    [SerializeField] List<GameObject> enemiesNotHit = new List<GameObject>();
    List<GameObject> enemiesHit = new List<GameObject>();

    LayerMask enemyLayer;

    /// <summary>
    /// Sets it's own variables to the scriptable object copy ref
    /// </summary>
    /// <param name="dataRef"></param> Ref to the SO with the up to date weapon stats
    /// <param name="combo"></param> Defaults to 0 if there is no combo
    public void SetData(BurstData dataRef, int damage, LayerMask lm, int combo = 0)
    {
        Debug.Log(gameObject.name);
        attackRef = dataRef;
        comboIndex = combo;

        myDamage = damage;
        myLifetime = dataRef.BurstRangedAttack.AttackProjectiles[combo].ProjectileLifetime;

        baseDamage = myDamage;
        baseLifetime = myLifetime;

        pierceDamageIndex = 0; pierceLifetimeIndex = 0;

        ricochetCount = 0;

        enemyLayer = lm;

        StartCoroutine(ProjectileLifetime());

        if(attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].HasSizeChange)
        {
            StartCoroutine(ChangeSize());
        }
    }

    /// <summary>
    /// Destroys the projectile when its lifetime runs out.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ProjectileLifetime()
    {
        float elapsedTime = 0;
        Vector3 endPoint = Vector3.zero;
        float prevDistance = 0; float newDistance = 0;
        float lifetime = attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].ProjectileLifetime;

        if (attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].LifetimeIsRange)
        {
            endPoint = transform.position + (transform.forward * attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].ProjectileRange);
            prevDistance = newDistance = Vector3.Distance(transform.position, endPoint);
            lifetime = 30;
        }
        while (lifetime > elapsedTime)
        {
            if(attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].LifetimeIsRange)
            {
                prevDistance = newDistance;
                newDistance = Vector3.Distance(transform.position, endPoint);

                if(prevDistance < newDistance)
                {
                    break;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        Destroy(this.gameObject);
    }

    /// <summary>
    /// Handles what the projectile should do when it touches something.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerEnter(Collider other)
    {
        if (!enemiesHit.Contains(other.gameObject))
        {
            enemiesHit.Add(other.gameObject);
        }

        hitEnemy = true;

        //Replace dummy check with enemy collision check
        if (other.GetComponent<DummyBehaviour>() || other.GetComponent<Enemy>())
        {
            //replace with the enemy taking damage
            other.GetComponent<Enemy>().EnemyTakeDamage(myDamage);

            if(attackRef.HasLifesteal)
            {
                int healValue = Mathf.CeilToInt(myDamage * ConvertToPercentage(attackRef.WeaponLifeSteal.LifestealPercent));

                //TODO Replace with heal player
                FindAnyObjectByType<MidRunDataManager>().ChangePlayerHealth(healValue);
            }

            //Don't want to destroy projectile if it pierces
            if(attackRef.BurstRangedAttack.HasPierce)
            {
                Debug.Log(Mathf.CeilToInt(baseDamage * ConvertToPercentage(attackRef.BurstRangedAttack.BurstPierce.PierceAmount)));

                myDamage = myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(attackRef.BurstRangedAttack.BurstPierce.PierceDamageFalloffPercent[pierceDamageIndex]))
                    >= (myDamage * attackRef.BurstRangedAttack.BurstPierce.MinPierceDamagePercent) ? myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(attackRef.BurstRangedAttack.BurstPierce.PierceDamageFalloffPercent[pierceDamageIndex]))
                    : (myDamage * attackRef.BurstRangedAttack.BurstPierce.MinPierceDamagePercent);

                myLifetime -= (baseLifetime * ConvertToPercentage(attackRef.BurstRangedAttack.BurstPierce.PierceLifetimeFalloffPercent[pierceLifetimeIndex]));

                //Destroy when pierce projectile stops doing damage
                if(myDamage <= 0)
                {
                    Destroy(this.gameObject);
                }
                
                //If stat changes from piercing aren't linear, update what stat change will be
                pierceDamageIndex = pierceDamageIndex + 1 < attackRef.BurstRangedAttack.BurstPierce.PierceDamageFalloffPercent.Count ? ++pierceDamageIndex : pierceDamageIndex;
                pierceLifetimeIndex = pierceLifetimeIndex + 1 < attackRef.BurstRangedAttack.BurstPierce.PierceLifetimeFalloffPercent.Count ? ++pierceLifetimeIndex : pierceLifetimeIndex;
            }

            if(attackRef.BurstRangedAttack.HasRicochet)
            {
                RicochetProjectile(other.gameObject);
            }

            if(attackRef.HasAoe)
            {
                SpawnAOE(other.ClosestPoint(transform.position));
            }

            if(!attackRef.BurstRangedAttack.HasPierce && !attackRef.BurstRangedAttack.HasRicochet)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.Log(other.gameObject.name);
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Finds if there are enemies in range and ricochets to the closest one.s
    /// </summary>
    protected void RicochetProjectile(GameObject hitEnemy)
    {
        if (ricochetCount < attackRef.BurstRangedAttack.BurstRicochet.RicochetCount)
        {
            ++ricochetCount;
            enemiesInRange.Clear();
            enemiesNotHit.Clear();

            RaycastHit[] enemies = Physics.SphereCastAll(transform.position, attackRef.BurstRangedAttack.BurstRicochet.RicochetRange, transform.forward, 0, enemyLayer);

            if (enemies.Length <= 1)
            {
                Destroy(this.gameObject);
                return;
            }

            foreach (RaycastHit e in enemies)
            {
                if (e.collider.gameObject != hitEnemy)
                {
                    enemiesInRange.Add(e.collider.gameObject);

                    if (!enemiesHit.Contains(e.collider.gameObject))
                    {
                        enemiesNotHit.Add(e.collider.gameObject);
                    }
                }
            }

            StartCoroutine(TrackEnemy(FindNearestEnemy(enemiesNotHit.Count > 0 ? enemiesNotHit : enemiesInRange)));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Finds the closest enemy to the projectile
    /// </summary>
    protected GameObject FindNearestEnemy(List<GameObject> targets)
    {
        GameObject nearest = targets[0];
        List<GameObject> equidistantEnemies = new List<GameObject>();
        equidistantEnemies.Add(targets[0]);

        for(int i = 1; i < targets.Count; ++i)
        {
            if(Vector3.Distance(transform.position, targets[i].transform.position) < Vector3.Distance(transform.position, nearest.transform.position))
            {
                equidistantEnemies.Clear();
                nearest = targets[i];
            }
            else if(Vector3.Distance(transform.position, targets[i].transform.position) == Vector3.Distance(transform.position, nearest.transform.position))
            {
                if(!equidistantEnemies.Contains(nearest))
                {
                    equidistantEnemies.Add(nearest);
                }
                equidistantEnemies.Add(targets[i]);
            }
        }

        if(equidistantEnemies.Count > 1)
        {
            int rIndex = Random.Range(0, equidistantEnemies.Count);
            nearest = equidistantEnemies[rIndex];
        }

        Debug.Log(nearest.name);
        return nearest;
    }

    /// <summary>
    /// Moves the projectile to a locked on enemy
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    protected IEnumerator TrackEnemy(GameObject enemy)
    {
        hitEnemy = false;
        float breakoutTime = 0;
        while(!hitEnemy)
        {
            if(breakoutTime > 10)
            {
                break;
            }

            GetComponent<Rigidbody>().linearVelocity = (enemy.transform.position - transform.position).normalized * attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].ProjectileSpeed;
            transform.LookAt(enemy.transform.position);

            yield return new WaitForFixedUpdate();
            breakoutTime += .01f;
        }
    }

    /// <summary>
    /// Creates an AOE at the designated location.
    /// </summary>
    public void SpawnAOE(Vector3 spawnPos)
    {
        GameObject spawnedAOE = Instantiate(attackRef.WeaponAoe.AoePrefab, spawnPos, Quaternion.identity);
        spawnedAOE.GetComponent<BaseAOEBehaviour>().SetReference(attackRef.WeaponAoe, myDamage, enemyLayer, comboIndex);
    }

    /// <summary>
    /// Changes the projectile's size over time
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ChangeSize()
    {
        SizeChangeData sData = attackRef.BurstRangedAttack.AttackProjectiles[comboIndex].ProjectileSizeChange;
        if (sData.InfinitelyChangeSize)
        {

        }
        else
        {
            Vector3 startSize = transform.localScale;
            Vector3 endSize = transform.localScale * sData.NewSize;
            float elapsedTime = 0;

            do
            {
                Vector3 newSize = Vector3.Lerp(startSize, endSize, (elapsedTime / sData.TimeTillNewSize));

                if(sData.ConstXSize)
                {
                    newSize.x = startSize.x;
                }
                if(sData.ConstYSize)
                {
                    newSize.y = startSize.y;
                }
                if(sData.ConstZSize)
                {
                    newSize.z = startSize.z;
                }
                transform.localScale = newSize;

                yield return new WaitForFixedUpdate();
                elapsedTime += Time.deltaTime;
            } while (elapsedTime < sData.TimeTillNewSize);
        }
    }

/// <summary>
/// Converts the inspector variable to it's percentage value if it isn't already a percent
/// </summary>
/// <param name="value"></param>
/// <returns></returns>
protected float ConvertToPercentage(float value)
    {
        return value >= 1 ? value / 100 : value;
    }
}
