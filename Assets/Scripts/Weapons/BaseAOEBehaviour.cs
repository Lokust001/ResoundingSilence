/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/20/2026
 * Brief: Base functionality for things that will create an AOE
 * TODO: Replace damage debugs with damage function calls
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class BaseAOEBehaviour : MonoBehaviour
{
    protected BaseWeaponScriptable weaponRef;
    protected int comboIndex;

    /// <summary>
    /// Gets a reference to the copy of the scriptable object the weapon is using
    /// </summary>
    /// <param name="dataRef"></param>
    public void SetReference(BaseWeaponScriptable dataRef, int cIndex = 0)
    {
        weaponRef = dataRef;
        comboIndex = cIndex;
        transform.localScale = new Vector3(weaponRef.AOESize[comboIndex], transform.localScale.y, weaponRef.AOESize[comboIndex]);

        StartCoroutine(ActivateAOE());
    }

    /// <summary>
    /// Waits x amount of time and then activates the AOE
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ActivateAOE()
    {
        yield return new WaitForSeconds(weaponRef.AOEDelay[comboIndex]);

        int damage = weaponRef.AOEDuration[comboIndex] > 0 ? Mathf.CeilToInt(weaponRef.AOEDamage[comboIndex] / weaponRef.AOEDamageTicks[comboIndex]) : weaponRef.AOEDamage[comboIndex];
        float delay = weaponRef.AOEDamageTicks[comboIndex] > 0 ? weaponRef.AOEDuration[comboIndex] / weaponRef.AOEDamageTicks[comboIndex] : 0;
        float elapsed = 0;

        do
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * weaponRef.AOESize[comboIndex], weaponRef.EnemyLayer);

            foreach(Collider t in targets)
            {
                t.GetComponent<Enemy>().EnemyTakeDamage(damage);
                Debug.Log(t.name + " took " + damage + " damage!");
            }

            yield return new WaitForSeconds(delay);
            elapsed += delay;
        } while (elapsed < weaponRef.AOEDuration[comboIndex]);

        Destroy(this.gameObject);
    }
}
