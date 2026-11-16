/******************************************************************************
 * Author: Brad Dixon
 * Contributors: Tyler Hayes
 * Last Modified: 11/10/2026
 * Brief: Calls the specific weapon attack
 * TODO:
 * ***************************************************************************/
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeaponBehaviour : MonoBehaviour
{
    protected enum AbilitySettings
    {
        AbilityOne,
        AbilityTwo
    }

    //Public so the upgrade manager can find the reference
    //[HideInInspector]
    [Expandable]
    public BaseWeaponScriptable ThisWeaponData;
    [SerializeField] protected int weaponDataID;
    [SerializeField] protected LayerMask groundLayerMask;

    [SerializeField] protected Animator weaponAnimator;
    [SerializeField] protected List<AnimationPath> attackAnimations = new List<AnimationPath>();
    [SerializeField] protected int comboIndex;

    [SerializeField] protected List<Collider> meleeHitboxes = new List<Collider>();

    public GameObject weaponModel;
    public bool activeWeapon;

    protected Vector3 mousePos = Vector3.forward;

    protected Vector3 aimDir = Vector3.forward;

    protected bool playerAiming;

    protected float currentAttackSpeed;

    protected bool isAttacking;
    protected bool attackReady;

    protected bool burstAttacking;
    protected int burstIndex;

    /// <summary>
    /// Gets a reference to the copy of the weapon's data
    /// </summary>
    virtual protected void Start()
    {
        ThisWeaponData = StaticDataManager.Instance.GetWeaponAtID(weaponDataID);
        //TESTING ONLY
        MidRunDataManager.Instance.EquipWeaponInSlot(0, ThisWeaponData);
        //tells the abilities to instantiate
        ThisWeaponData.GetAbilities();
        attackReady = true;
        comboIndex = 0;
        burstIndex = 0;

        currentAttackSpeed = ThisWeaponData.AttackSpeed;
        UpdateAttackSpeed();

        GenericPublicEvents.SetCurrentWeapon?.Invoke(this);
    }

    /// <summary>
    /// Enables input event listening
    /// </summary>
    virtual protected void OnEnable()
    {
        InputPublicEvents.ShootPressed += PlayerAttacking;
        InputPublicEvents.ShootReleased += PlayerStoppedAttacking;
        InputPublicEvents.KeyboardMouseEnabled += KBMEnabled;
        InputPublicEvents.ControllerEnabled += ControllerEnabled;
        attackReady = true;
        comboIndex = 0;
        burstIndex = 0;

        StartCoroutine(EnableInputs());
    }

    /// <summary>
    /// enables the right input based on the control type
    /// </summary>
    /// <returns></returns>
    private IEnumerator EnableInputs()
    {
        while (InputManager.Instance == null)
        {
            yield return null;
            Debug.Log($"Player Inputs are null");
        }

        if (InputManager.Instance.ControllerIsEnabled)
        {
            ControllerEnabled();
        }
        else
        {
            KBMEnabled();
        }
    }

    /// <summary>
    /// triggers when the keyboard and mouse get enabled
    /// </summary>
    private void KBMEnabled()
    {
        InputPublicEvents.MouseMoved += GetMousPos;
        InputPublicEvents.PlayerAimed -= PlayerAimed;
        InputPublicEvents.PlayerStoppedAiming -= PlayerStoppedAiming;
    }

    /// <summary>
    /// triggers when controller is enabled
    /// </summary>
    private void ControllerEnabled()
    {
        InputPublicEvents.MouseMoved -= GetMousPos;
        InputPublicEvents.PlayerAimed += PlayerAimed;
        InputPublicEvents.PlayerStoppedAiming += PlayerStoppedAiming;
    }


    /// <summary>
    /// Disables input event listening to prevent duplicates
    /// </summary>
    virtual protected void OnDisable()
    {
        InputPublicEvents.ShootPressed -= PlayerAttacking;
        InputPublicEvents.ShootReleased -= PlayerStoppedAttacking;
        InputPublicEvents.KeyboardMouseEnabled -= KBMEnabled;
        InputPublicEvents.ControllerEnabled -= ControllerEnabled;
        InputPublicEvents.PlayerStoppedAiming -= PlayerStoppedAiming;
        InputPublicEvents.MouseMoved -= GetMousPos;
        InputPublicEvents.PlayerAimed -= PlayerAimed;
    }

    #region Attacking
    /// <summary>
    /// Determines if whether or not the player casts an ability or attacks with the weapon
    /// </summary>
    virtual protected void PlayerAttacking()
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
        Plane p = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (p.Raycast(ray, out float hit))
        {
            mousePos = ray.GetPoint(hit) - transform.position;
            aimDir = mousePos.normalized;
        }
    }

    /// <summary>
    /// gets the direction of the player's aim
    /// </summary>
    /// <param name="dir"></param>
    protected void PlayerAimed(Vector2 dir)
    {
        aimDir = new(dir.x, aimDir.y, dir.y);
        playerAiming = true;

    }

    /// <summary>
    /// trigger when the player has stopped aiming
    /// </summary>
    protected void PlayerStoppedAiming()
    {
        playerAiming = false;
    }

    /// <summary>
    /// Attacks with the weapon if off cooldown
    /// </summary>
    virtual protected void FixedUpdate()
    {
        if (UIManager.Instance.CurrentlyInFullscreenMenu())
        {
            return;
        }

        Quaternion rot = Quaternion.LookRotation(aimDir);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;

        //Check if attack speed gets changed
        if (currentAttackSpeed != ThisWeaponData.AttackSpeed)
        {
            Debug.Log("Changing attack speed");
            currentAttackSpeed = ThisWeaponData.AttackSpeed;
            UpdateAttackSpeed();
        }

        if (isAttacking && attackReady)
        {
            Attack();
            attackReady = false;
        }
    }

    /// <summary>
    /// Updates the weapon's animation speed. Lets us make the attacks look faster or slower.
    /// </summary>
    protected void UpdateAttackSpeed()
    {
        weaponAnimator.speed = currentAttackSpeed;
    }

    /// <summary>
    /// Tells the weapon to attack. Throws error if not replaced with attack functionality.
    /// </summary>
    virtual protected void Attack()
    {
        if (activeWeapon)
        {
            if (!burstAttacking)
            {
                ++comboIndex;
            }

            if (attackAnimations[comboIndex - 1].AnimatorUsed != null && !weaponAnimator.Equals(attackAnimations[comboIndex - 1].AnimatorUsed))
            {
                weaponAnimator = attackAnimations[comboIndex - 1].AnimatorUsed;
            }

            weaponAnimator.Play(attackAnimations[comboIndex - 1].AnimClip.name, attackAnimations[comboIndex - 1].LayerIndex);
        }
    }

    /// <summary>
    /// Determines if a burst of attacks should be done all at once or if they should be done in order
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackBursts(bool weaponAttack = true)
    {
        if (activeWeapon)
        {
            yield return new WaitForSeconds(ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks[burstIndex].TimeTillNextBurst);
            ++burstIndex;

            if (burstIndex < ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks.Count)
            {
                Attack();
            }
            else
            {
                burstIndex = 0;
                burstAttacking = false;

                if (weaponAttack)
                {
                    StartCoroutine(AttackDelay());
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// Called by animation event to spawn projectile during attack animation
    /// </summary>
    virtual public void SpawnProjectile(int cIndex)
    {

    }

    /// <summary>
    /// Called by an animation event to see if a melee swing hit anything
    /// </summary>
    virtual public void MeleeSwing(int cIndex)
    {

    }
    #endregion

    /// <summary>
    /// Waits for the weapon's attack time before letting it attack again
    /// </summary>
    /// <returns></returns>
    virtual protected IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(ThisWeaponData.ComboAttacks[comboIndex - 1].AttackCooldown);
        if (comboIndex >= attackAnimations.Count)
        {
            comboIndex = 0;
        }
        attackReady = true;
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

    public void SpawnVFX(GameObject vfx)
    {
        Instantiate(vfx);
    }
}
