/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/18/2026
 * Brief: Weapon architecture that all weapons inherit
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class BaseWeaponBehaviour : MonoBehaviour
{
    //Public so the upgrade manager can find the reference
    [HideInInspector] public BaseWeaponScriptable ThisWeaponData;
    [SerializeField] protected int weaponDataID;

    protected Vector3 mousePos;

    protected bool isAttacking;
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
    virtual protected void OnEnable()
    {
        InputPublicEvents.ShootPressed += PlayerAttacking;
        InputPublicEvents.ShootReleased += PlayerStoppedAttacking;
        InputPublicEvents.MouseMoved += GetMousPos;
        InputPublicEvents.AbilityOnePressed += AimingAbilityOne;
        InputPublicEvents.AbilityOneReleased += CastingAbilityOne;
        InputPublicEvents.AbilityTwoPressed += AimingAbilityTwo;
        InputPublicEvents.AbilityTwoReleased += CastingAbilityTwo;
    }

    /// <summary>
    /// Disables input event listening to prevent duplicates
    /// </summary>
    virtual protected void OnDisable()
    {
        InputPublicEvents.ShootPressed -= PlayerAttacking;
        InputPublicEvents.ShootReleased -= PlayerStoppedAttacking;
        InputPublicEvents.MouseMoved -= GetMousPos;
        InputPublicEvents.AbilityOnePressed -= AimingAbilityOne;
        InputPublicEvents.AbilityOneReleased -= CastingAbilityOne;
        InputPublicEvents.AbilityTwoPressed -= AimingAbilityTwo;
        InputPublicEvents.AbilityTwoReleased -= CastingAbilityTwo;
    }

    /// <summary>
    /// Updates whether or not the player is holding the attack button
    /// </summary>
    protected void PlayerAttacking()
    {
        isAttacking = true;
    }

    /// <summary>
    /// Updates whether or not the player is holding the attack button
    /// </summary>
    protected void PlayerStoppedAttacking()
    {
        isAttacking = false;
    }

    /// <summary>
    /// Gets the mouse's position on the screen
    /// </summary>
    protected void GetMousPos(Vector2 pos)
    {
        mousePos = pos;
    }

    /// <summary>
    /// How the player aims ability one
    /// </summary>
    virtual protected void AimingAbilityOne()
    {
        throw new System.Exception("Functionality not coded!");
    }

    /// <summary>
    /// How the player aims ability two
    /// </summary>
    virtual protected void AimingAbilityTwo()
    {
        throw new System.Exception("Functionality not coded!");
    }

    /// <summary>
    /// What ability one does
    /// </summary>
    virtual protected void CastingAbilityOne()
    {
        throw new System.Exception("Functionality not coded!");
    }

    /// <summary>
    /// What ability two does
    /// </summary>
    virtual protected void CastingAbilityTwo()
    {
        throw new System.Exception("Functionality not coded!");
    }

    /// <summary>
    /// Attacks with the weapon if off cooldown
    /// </summary>
    virtual protected void FixedUpdate()
    {
        if (isAttacking && attackReady)
        {
            Attack();
            attackReady = false;
            StartCoroutine(AttackDelay());
        }
    }

    /// <summary>
    /// Tells the weapon to attack. Throws error if not replaced with attack functionality.
    /// </summary>
    virtual protected void Attack()
    {
        throw new System.Exception("Forgot to add attack functionality.");
    }

    /// <summary>
    /// Waits for the weapon's attack time before letting it attack again
    /// </summary>
    /// <returns></returns>
    virtual protected IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(ThisWeaponData.AttackCooldown[0]);
        attackReady = true;
    }
}
