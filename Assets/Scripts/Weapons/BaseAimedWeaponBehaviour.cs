/******************************************************************************
 * Author: Brad Dixon
 * Contributors: 
 * Last Modified: 9/23/2026
 * Brief: Handles the base functions that all aimed weapons will use
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;

public class BaseAimedWeaponBehaviour : BaseWeaponBehaviour
{
    [HorizontalLine(height: 4, EColor.Violet)]
    [SerializeField] protected GameObject weaponProjectile;

    /// <summary>
    /// Spawns the projectile and fires it towards the mouse
    /// </summary>
    override protected void Attack()
    {
        Vector3 bulletDir = mousePos - transform.position;
        bulletDir.y = 0;

        GameObject spawnedProjectile = Instantiate(weaponProjectile, transform.position, Quaternion.LookRotation(bulletDir.normalized, Vector3.up));
        spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(ThisWeaponData);
        spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * ThisWeaponData.ProjectileSpeed[0];
    }
}
