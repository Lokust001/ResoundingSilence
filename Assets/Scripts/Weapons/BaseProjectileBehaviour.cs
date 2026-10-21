/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/20/2026
 * Brief: Handles projectile collision and lifetime
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseProjectileBehaviour : MonoBehaviour
{
    BaseWeaponScriptable weaponRef;
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

    /// <summary>
    /// Sets it's own variables to the scriptable object copy ref
    /// </summary>
    /// <param name="dataRef"></param> Ref to the SO with the up to date weapon stats
    /// <param name="combo"></param> Defaults to 0 if there is no combo
    public void SetData(BaseWeaponScriptable dataRef, int combo = 0)
    {
        weaponRef = dataRef;
        comboIndex = combo;

        myDamage = dataRef.WeaponDamage[combo];
        myLifetime = dataRef.ProjectileLifetime[combo];

        baseDamage = myDamage;
        baseLifetime = myLifetime;

        pierceDamageIndex = 0; pierceLifetimeIndex = 0;

        ricochetCount = 0;

        StartCoroutine(ProjectileLifetime());
    }

    /// <summary>
    /// Destroys the projectile when its lifetime runs out.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ProjectileLifetime()
    {
        float elapsedTime = 0;

        while(weaponRef.ProjectileLifetime[comboIndex] > elapsedTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
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
            other.GetComponent<DummyBehaviour>().TakeDamage(myDamage);

            if(weaponRef.HasLifesteal)
            {
                int healValue = Mathf.CeilToInt(myDamage * ConvertToPercentage(weaponRef.LifestealAmount[pierceDamageIndex]));

                //TODO Replace with heal player
                FindAnyObjectByType<DummyPlayerBehaviour>().Heal(healValue);
            }

            //Don't want to destroy projectile if it pierces
            if(weaponRef.HasPierce)
            {
                Debug.Log(Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex])));

                myDamage = myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex]))
                    >= weaponRef.MinPierceDamage[pierceDamageIndex] ? myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex]))
                    : weaponRef.MinPierceDamage[pierceDamageIndex];

                myLifetime -= (baseLifetime * ConvertToPercentage(weaponRef.PierceLifetimeFalloff[pierceLifetimeIndex]));

                //Destroy when pierce projectile stops doing damage
                if(myDamage <= 0)
                {
                    Destroy(this.gameObject);
                }
                
                //If stat changes from piercing aren't linear, update what stat change will be
                pierceDamageIndex = pierceDamageIndex + 1 < weaponRef.PierceDamageFalloff.Count ? ++pierceDamageIndex : pierceDamageIndex;
                pierceLifetimeIndex = pierceLifetimeIndex + 1 < weaponRef.PierceLifetimeFalloff.Count ? ++pierceLifetimeIndex : pierceLifetimeIndex;
            }

            if(weaponRef.Ricochets)
            {
                RicochetProjectile(other.gameObject);
            }

            if(weaponRef.HasAOEAttack)
            {
                SpawnAOE(other.ClosestPoint(transform.position));
            }

            if(!weaponRef.HasPierce && !weaponRef.Ricochets)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Finds if there are enemies in range and ricochets to the closest one.s
    /// </summary>
    protected void RicochetProjectile(GameObject hitEnemy)
    {
        if (ricochetCount < weaponRef.RicochetCount[comboIndex])
        {
            ++ricochetCount;
            enemiesInRange.Clear();
            enemiesNotHit.Clear();

            RaycastHit[] enemies = Physics.SphereCastAll(transform.position, weaponRef.RicochetRange[comboIndex], transform.forward, 0, weaponRef.EnemyLayer);

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

            GetComponent<Rigidbody>().linearVelocity = (enemy.transform.position - transform.position).normalized * weaponRef.ProjectileSpeed[comboIndex];
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
        //if (cIndex == -1)
        //{
        //    cIndex = Random.Range(0, weaponRef.WeaponAOEs.Count);
        //}

        GameObject spawnedAOE = Instantiate(weaponRef.WeaponAOEs[comboIndex], spawnPos, Quaternion.identity);
        spawnedAOE.GetComponent<BaseAOEBehaviour>().SetReference(weaponRef, comboIndex);
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
