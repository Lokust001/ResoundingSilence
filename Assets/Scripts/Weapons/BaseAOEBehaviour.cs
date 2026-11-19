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
    protected AoeData weaponRef;
    protected int comboIndex;
    protected int aoeDamage;
    protected LayerMask enemyLayer;

    /// <summary>
    /// Gets a reference to the copy of the scriptable object the weapon is using
    /// </summary>
    /// <param name="dataRef"></param>
    public void SetReference(AoeData dataRef, int damage, LayerMask lm, int cIndex = 0)
    {
        weaponRef = dataRef;
        comboIndex = cIndex;
        transform.localScale = new Vector3(weaponRef.AoeSize, transform.localScale.y, weaponRef.AoeSize);
        aoeDamage = damage;
        enemyLayer = lm;

        StartCoroutine(ActivateAOE());
    }

    /// <summary>
    /// Waits x amount of time and then activates the AOE
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ActivateAOE()
    {
        yield return new WaitForSeconds(weaponRef.AoeDelay);

        int damage = weaponRef.AoeDuration > 0 ? Mathf.CeilToInt(aoeDamage / weaponRef.AoeDamageTicks) : aoeDamage;
        float delay = weaponRef.AoeDamageTicks > 0 ? weaponRef.AoeDuration / weaponRef.AoeDamageTicks : 0;
        float elapsed = 0;

        do
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius * weaponRef.AoeSize, enemyLayer);

            foreach(Collider t in targets)
            {
                t.GetComponent<Enemy>().EnemyTakeDamage(damage);
                Debug.Log(t.name + " took " + damage + " damage!");
            }

            yield return new WaitForSeconds(delay);
            elapsed += delay;
        } while (elapsed < weaponRef.AoeDuration);

        Destroy(this.gameObject);
    }
}
