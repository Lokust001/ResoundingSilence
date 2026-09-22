/******************************************************************************
 * Author: Brad Dixon
 * Contributors: 
 * Last Modified: 9/22/2026
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
        Plane p = new Plane(Vector3.forward, 0);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        float distanceAlongRay = 0; //Defauls to 0

        if(p.Raycast(ray, out distanceAlongRay))
        {
            Vector3 bulletDir = new Vector3(ray.GetPoint(distanceAlongRay).x, 0, ray.GetPoint(distanceAlongRay).y) - transform.position;

            GameObject spawnedProjectile = Instantiate(weaponProjectile, transform.position, Quaternion.LookRotation(bulletDir.normalized, Vector3.up));
            spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(ThisWeaponData);
            spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * ThisWeaponData.ProjectileSpeed[0];
        }
    }
}
