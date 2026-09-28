/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/18/2026
 * Brief: Handles projectile collision and lifetime
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class BaseProjectileBehaviour : MonoBehaviour
{
    BaseWeaponScriptable weaponRef;
    int pierceDamageIndex;
    int pierceLifetimeIndex;

    int myDamage;
    float myLifetime;
    int baseDamage;
    float baseLifetime;
    bool hasPierce;
    bool hasLifesteal;

    /// <summary>
    /// Sets it's own variables to the scriptable object copy ref
    /// </summary>
    /// <param name="dataRef"></param> Ref to the SO with the up to date weapon stats
    /// <param name="combo"></param> Defaults to 0 if there is no combo
    public void SetData(BaseWeaponScriptable dataRef, int combo = 0)
    {
        weaponRef = dataRef;

        myDamage = dataRef.WeaponDamage[combo];
        myLifetime = dataRef.ProjectileLifetime[combo];
        hasPierce = dataRef.HasPierce;
        hasLifesteal = dataRef.HasLifesteal;

        baseDamage = myDamage;
        baseLifetime = myLifetime;

        pierceDamageIndex = 0; pierceLifetimeIndex = 0;

        StartCoroutine(ProjectileLifetime());
    }

    /// <summary>
    /// Destroys the projectile when its lifetime runs out.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ProjectileLifetime()
    {
        float elapsedTime = 0;

        while(myLifetime > elapsedTime)
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
        //Replace dummy check with enemy collision check
        if(other.GetComponent<DummyBehaviour>())
        {
            //TODO Replace with damage enemy
            Debug.Log("Did " + myDamage + " damage!");

            if(hasLifesteal)
            {
                int healValue = Mathf.CeilToInt(myDamage * ConvertToPercentage(weaponRef.LifestealAmount));

                //TODO Replace with heal player
                Debug.Log("Healed for " + healValue + "!");
            }

            //Don't want to destroy projectile if it pierces
            if(hasPierce)
            {
                Debug.Log(Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex])));

                myDamage = myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex]))
                    >= weaponRef.MinPierceDamage ? myDamage - Mathf.CeilToInt(baseDamage * ConvertToPercentage(weaponRef.PierceDamageFalloff[pierceDamageIndex]))
                    : weaponRef.MinPierceDamage;

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
            else
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
    /// Converts the inspector variable to it's percentage value if it isn't already a percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected float ConvertToPercentage(float value)
    {
        return value >= 1 ? value / 100 : value;
    }
}
