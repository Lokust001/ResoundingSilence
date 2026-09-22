/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/22/2026
 * Brief: Weapon architecture that all weapons inherit
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class BaseWeaponBehaviour : MonoBehaviour
{
    protected enum AbilitySettings
    {
        AbilityOne,
        AbilityTwo
    }

    //Public so the upgrade manager can find the reference
    [HideInInspector] public BaseWeaponScriptable ThisWeaponData;
    [SerializeField] protected int weaponDataID;

    [Header("Base Ability Variables"), HorizontalLine(height: 4, EColor.Red)]
    [SerializeField] protected AbilitySettings abilitySettings;

    protected Vector3 mousePos;

    protected bool isAttacking;
    protected bool attackReady;

    protected bool aimingAbilityOne;
    protected bool aimingAbilityTwo;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [Tooltip("Set true if you want ability one to be ready to use on spawn.")]
    [SerializeField] protected bool abilityOneReady;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [Tooltip("How long the ability is on cooldown for.")]
    [SerializeField] protected float abilityOneCooldown;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [Tooltip("How long the ability puts the player in end lag for.")]
    [SerializeField] protected float abilityOneEndLag;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [Tooltip("How much damage the ability does.")]
    [SerializeField] protected int abilityOneDamage;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [Tooltip("How far from the player the ability can be cast.")]
    [SerializeField] protected float abilityOneRange;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [Tooltip("Set true if you want ability two to be ready to use on spawn.")]
    [SerializeField] protected bool abilityTwoReady;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [Tooltip("How long the ability is on cooldown for.")]
    [SerializeField] protected float abilityTwoCooldown;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [Tooltip("How long the ability puts the player in end lag for.")]
    [SerializeField] protected float abilityTwoEndLag;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [Tooltip("How much damage the ability does.")]
    [SerializeField] protected int abilityTwoDamage;

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [Tooltip("How far from the player the ability can be cast.")]
    [SerializeField] protected float abilityTwoRange;

    /// <summary>
    /// Gets a reference to the copy of the weapon's data
    /// </summary>
    virtual protected void Start()
    {
        ThisWeaponData = StaticDataManager.Instance.GetWeaponAtID(weaponDataID);
        attackReady = true;

        if(!abilityOneReady)
        {
            StartCoroutine(AbilityDelay(abilityOneReady, abilityOneCooldown));
        }

        if(!abilityTwoReady)
        {
            StartCoroutine(AbilityDelay(abilityTwoReady, abilityTwoCooldown));
        }
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
        InputPublicEvents.AbilityTwoPressed += AimingAbilityTwo;
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
        InputPublicEvents.AbilityTwoPressed -= AimingAbilityTwo;
    }

    /// <summary>
    /// Updates whether or not the player is holding the attack button
    /// </summary>
    protected void PlayerAttacking()
    {
        if (aimingAbilityOne || aimingAbilityTwo)
        {
            CastingAbility();
        }
        else
        {
            isAttacking = true;
        }
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
    /// How the player aims ability one. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityOne()
    {
        if (abilityOneReady)
        {
            aimingAbilityOne = !aimingAbilityOne;
            aimingAbilityTwo = false;
        }
    }

    /// <summary>
    /// How the player aims ability two. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityTwo()
    {
        if (abilityTwoReady)
        {
            aimingAbilityTwo = !aimingAbilityTwo;
            aimingAbilityOne = false;
        }
    }

    /// <summary>
    /// The base inheritance for using weapon abilities
    /// </summary>
    virtual protected void CastingAbility()
    {
        attackReady = false;
        if(aimingAbilityOne)
        {
            aimingAbilityOne = false;
            StartCoroutine(AbilityEndLag(abilityOneEndLag));
            StartCoroutine(AbilityDelay(true, abilityOneCooldown));
        }
        else if(aimingAbilityTwo)
        {
            aimingAbilityTwo = false;
            StartCoroutine(AbilityEndLag(abilityTwoEndLag));
            StartCoroutine(AbilityDelay(false, abilityTwoCooldown));
        }

        //Add the ability functionality in the actual weapon script
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

    /// <summary>
    /// How long the player gets put in end lag after using an ability
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    virtual protected IEnumerator AbilityEndLag(float time)
    {
        yield return new WaitForSeconds(time);
        attackReady = true;
    }

    /// <summary>
    /// After casting an ability, the player has to wait for it's cooldown before they can cast it again.
    /// </summary>
    /// <param name="abilityReady"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    virtual protected IEnumerator AbilityDelay(bool isAbilityOne, float time)
    {
        if(isAbilityOne)
        {
            abilityOneReady = false;
        }
        else
        {
            abilityTwoReady = false;
        }

        yield return new WaitForSeconds(time);

        if (isAbilityOne)
        {
            abilityOneReady = true;
        }
        else
        {
            abilityTwoReady = true;
        }
    }
}
