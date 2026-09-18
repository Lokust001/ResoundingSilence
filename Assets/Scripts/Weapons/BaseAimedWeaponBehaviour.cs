/******************************************************************************
 * Author: Brad Dixon
 * Contributors: 
 * Last Modified: 9/18/2026
 * Brief: Handles the base functions that all aimed weapons will use
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class BaseAimedWeaponBehaviour : MonoBehaviour
{
    //Public so the upgrade manager can find the reference
    [HideInInspector] public BaseWeaponScriptable ThisWeaponData;
    [SerializeField] protected int weaponDataID;
    [SerializeField] protected GameObject weaponProjectile;

    Vector3 mousePos;

    protected bool isShooting;
    protected bool attackReady;

    /// <summary>
    /// Gets a reference to the copy of the weapon's data
    /// </summary>
    virtual protected void Start()
    {
        ThisWeaponData = StaticDataManager.Instance.GetWeaponAtID(weaponDataID);
        attackReady = true;
    }

    /// <summary>
    /// Enables input event listening
    /// </summary>
    protected void OnEnable()
    {
        InputPublicEvents.ShootPressed += PlayerShooting;
        InputPublicEvents.ShootReleased += PlayerStoppedShooting;
        InputPublicEvents.MouseMoved += GetMousPos;
    }

    /// <summary>
    /// Disables input event listening to prevent duplicates
    /// </summary>
    protected void OnDisable()
    {
        InputPublicEvents.ShootPressed -= PlayerShooting;
        InputPublicEvents.ShootReleased -= PlayerStoppedShooting;
        InputPublicEvents.MouseMoved -= GetMousPos;
    }

    /// <summary>
    /// Updates whether or not the player is holding the attack button
    /// </summary>
    protected void PlayerShooting()
    {
        isShooting = true;
    }

    /// <summary>
    /// Updates whether or not the player is holding the attack button
    /// </summary>
    protected void PlayerStoppedShooting()
    {
        isShooting = false;
    }

    /// <summary>
    /// Gets the mouse's position on the screen
    /// </summary>
    protected void GetMousPos(Vector2 pos)
    {
        mousePos = pos;
    }

    /// <summary>
    /// Shoots the weapon if off cooldown
    /// </summary>
    protected void FixedUpdate()
    {
        if(isShooting && attackReady)
        {
            ShootWeapon();
            attackReady = false;
            StartCoroutine(ShootDelay());
        }
    }

    /// <summary>
    /// Spawns the projectile and fires it towards the mouse
    /// </summary>
    protected void ShootWeapon()
    {
        Plane p = new Plane(Vector3.forward, 0);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        float distanceAlongRay = 0; //Defauls to 0

        if(p.Raycast(ray, out distanceAlongRay))
        {
            Vector3 bulletDir = new Vector3(ray.GetPoint(distanceAlongRay).x, 0, ray.GetPoint(distanceAlongRay).y) - transform.position;

            GameObject spawnedProjectile = Instantiate(weaponProjectile, transform.position, Quaternion.identity);
            spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(ThisWeaponData);
            spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * ThisWeaponData.ProjectileSpeed[0];
        }
    }

    /// <summary>
    /// Waits for the weapon's attack time before letting it attack again
    /// </summary>
    /// <returns></returns>
    protected IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(ThisWeaponData.AttackCooldown[0]);
        attackReady = true;
    }
}
