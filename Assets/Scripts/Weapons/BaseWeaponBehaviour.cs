/******************************************************************************
 * Author: Brad Dixon
 * Contributors: Tyler Hayes
 * Last Modified: 11/5/2026
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
    [SerializeField] protected GameObject weaponModel;
    [SerializeField] protected LayerMask groundLayerMask;

    [SerializeField] protected Animator weaponAnimator;
    [SerializeField] protected List<AnimationClip> attackAnimations = new List<AnimationClip>();
    [SerializeField] protected int comboIndex;

    [SerializeField] List<Collider> meleeHitboxes = new List<Collider>();

    [Header("Base Ability Variables"), HorizontalLine(height: 4, EColor.Red)]
    //[SerializeField] protected AbilitySettings abilitySettings;

    //[SerializeField] protected List<AbilityData> WeaponAbilities = new List<AbilityData>();
    [SerializeField] protected List<GameObject> abilityAOEs = new List<GameObject>();

    [SerializeField] protected GameObject activePreview;
    [SerializeField] protected int currentAbilityIndex;

    bool continuouslyUpdatePreview;

    protected Vector3 mousePos = Vector3.forward;

    protected Vector3 aimDir = Vector3.forward;

    protected bool playerAiming;

    protected float currentAttackSpeed;

    protected bool isAttacking;
    protected bool attackReady;

    protected bool burstAttacking;
    protected int burstIndex;

    protected bool aimingAbilityOne;
    protected bool aimingAbilityTwo;

    protected bool abilityOneActive;
    protected bool abilityTwoActive;

    [SerializeField] public int abilityOneIndex;
    [SerializeField] public int abilityTwoIndex;

    [SerializeField] protected bool abilityOneReady;
    [SerializeField] protected bool abilityTwoReady;


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

        currentAttackSpeed = ThisWeaponData.AttackSpeed;
        UpdateAttackSpeed();

        int i = 0;
        foreach (GameObject g in abilityAOEs)
        {
            if (g != null)
            {
                activePreview = g;

                ThisWeaponData.editableAbilities[i].BaseAbilitySize = g.transform.localScale;
            }

            ++i;
        }

        if (!abilityOneReady)
        {
            StartCoroutine(AbilityDelay(abilityOneReady, ThisWeaponData.editableAbilities[abilityOneIndex].AbilityCooldown));
        }

        if (!abilityTwoReady)
        {
            StartCoroutine(AbilityDelay(abilityTwoReady, ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityCooldown));
        }

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
        InputPublicEvents.AbilityOnePressed += AimingAbilityOne;
        InputPublicEvents.AbilityTwoPressed += AimingAbilityTwo;

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
        InputPublicEvents.AbilityOnePressed -= AimingAbilityOne;
        InputPublicEvents.AbilityTwoPressed -= AimingAbilityTwo;
        InputPublicEvents.MouseMoved -= GetMousPos;
        InputPublicEvents.PlayerAimed -= PlayerAimed;
    }

    #region Attacking
    /// <summary>
    /// Determines if whether or not the player casts an ability or attacks with the weapon
    /// </summary>
    protected void PlayerAttacking()
    {
        if (aimingAbilityOne || aimingAbilityTwo)
        {
            //weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
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
        weaponModel.transform.rotation = rot;

        if (continuouslyUpdatePreview)
        {
            float staticY = activePreview.transform.position.y;

            Vector3 pos = activePreview.transform.position;

            if (InputManager.Instance.ControllerIsEnabled)
            {
                if (playerAiming)
                {
                    Vector3 currentAimVel = aimDir * StaticDataManager.Instance.playerAimSpeed;

                    pos = transform.position + Vector3.ClampMagnitude(
                        (activePreview.transform.position - transform.position) + currentAimVel,
                        ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityRange);
                }

            }
            else
            {
                pos = transform.position + Vector3.ClampMagnitude(mousePos, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityRange);
            }

            pos.y = staticY;
            activePreview.transform.position = pos;
        }

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
            //StartCoroutine(AttackDelay());
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
        ++comboIndex;
        weaponAnimator.Play("Base Layer." + attackAnimations[comboIndex - 1].name);
    }

    /// <summary>
    /// Determines if a burst of attacks should be done all at once or if they should be done in order
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackBursts()
    {
        for (int i = 1; i < ThisWeaponData.AttackBursts[comboIndex - 1]; ++i)
        {
            yield return new WaitForSeconds(ThisWeaponData.TimeBetweenBursts[comboIndex - 1]);
            SpawnProjectile();
        }

        burstAttacking = false;
    }

    /// <summary>
    /// Called by animation event to spawn projectile during attack animation
    /// </summary>
    public void SpawnProjectile()
    {
        BurstData rData;
        int damage;
        if(abilityOneActive)
        {
            rData = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityDamage;
        }
        else if(abilityTwoActive)
        {
            rData = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityDamage;
        }
        else
        {
            rData = ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks[burstIndex];
            damage = ThisWeaponData.ComboAttacks[comboIndex - 1].WeaponDamage;
        }

        Vector3 bulletDir = aimDir;
        bulletDir.y = 0;

        GameObject spawnedProjectile = Instantiate(ThisWeaponData.Projectiles[comboIndex - 1], transform.position, Quaternion.LookRotation(bulletDir.normalized, Vector3.up));
        spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(rData, damage, ThisWeaponData.EnemyLayer);
        spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * ThisWeaponData.ProjectileSpeed[comboIndex - 1];

        //If the weapon has multiple bursts fire them
        if (ThisWeaponData.AttackBursts[comboIndex - 1] > 1 && !burstAttacking)
        {
            burstAttacking = true;
            StartCoroutine(AttackBursts());
        }

        StartCoroutine(AttackDelay());
    }

    /// <summary>
    /// Called by an animation event to see if a melee swing hit anything
    /// </summary>
    public void MeleeSwing()
    {
        foreach (Collider c in meleeHitboxes)
        {
            if (c.enabled)
            {
                Collider[] targets = Physics.OverlapBox(c.bounds.center, c.bounds.extents, Quaternion.identity, ThisWeaponData.EnemyLayer);

                foreach (Collider t in targets)
                {
                    if (t.GetComponent<Enemy>())
                    {
                        t.GetComponent<Enemy>().EnemyTakeDamage(ThisWeaponData.WeaponDamage[comboIndex - 1]);

                        if (ThisWeaponData.HasLifesteal)
                        {
                            int healValue = Mathf.CeilToInt(ThisWeaponData.WeaponDamage[comboIndex - 1] * ConvertToPercentage(ThisWeaponData.LifestealAmount[comboIndex - 1]));

                            //TODO Replace with heal player
                            FindAnyObjectByType<MidRunDataManager>().ChangePlayerHealth(healValue);
                        }
                    }
                    Debug.Log(t.name + " took " + ThisWeaponData.WeaponDamage[comboIndex - 1] + " damage!");
                }
            }
        }

        StartCoroutine(AttackDelay());
    }
    #endregion

    #region Abilities
    /// <summary>
    /// Waits for the weapon's attack time before letting it attack again
    /// </summary>
    /// <returns></returns>
    virtual protected IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(ThisWeaponData.AttackCooldown[0]);
        if (comboIndex >= attackAnimations.Count)
        {
            comboIndex = 0;
        }
        attackReady = true;
    }

    /// <summary>
    /// How the player aims ability one. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityOne()
    {
        if (abilityOneReady)
        {
            aimingAbilityOne = !aimingAbilityOne;
            weaponAnimator.SetBool("AbilityCast", !aimingAbilityOne);
            aimingAbilityTwo = false;
            currentAbilityIndex = abilityOneIndex;
            if (ThisWeaponData.editableAbilities[abilityOneIndex].HasPreview)
            {
                AimingAbility();
            }
            else
            {
                //weaponAnimator.Play(WeaponAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, WeaponAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
                CastingAbility();
            }
        }
        else if(ThisWeaponData.editableAbilities[abilityOneIndex].CancelOnRecast && abilityOneActive)
        {
            weaponAnimator.SetBool("AbilityOver", true);
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
            weaponAnimator.SetBool("AbilityCast", !aimingAbilityTwo);
            aimingAbilityOne = false;
            currentAbilityIndex = abilityTwoIndex;
            if (ThisWeaponData.editableAbilities[currentAbilityIndex].HasPreview)
            {
                AimingAbility();
            }
            else
            {
                //weaponAnimator.Play(WeaponAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, WeaponAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
                CastingAbility();
            }
        }
        else if (ThisWeaponData.editableAbilities[abilityTwoIndex].CancelOnRecast && abilityTwoActive)
        {
            weaponAnimator.SetBool("AbilityOver", true);
        }
    }

    /// <summary>
    /// Displays the ability preview and plays the readying animation
    /// </summary>
    /// <param name="abilityOne"></param>
    virtual protected void AimingAbility()
    {
        activePreview.SetActive(false);

        activePreview = abilityAOEs[currentAbilityIndex];
        weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
    }

    /// <summary>
    /// Anim event that shows the ability preview
    /// </summary>
    /// <param name="orbitsPlayer"></param>
    /// <param name="sizeIsRange"></param>
    public void DisplayAbilityPreview()
    {
        Debug.Log("REACHED!!!");
        continuouslyUpdatePreview = !ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.OrbitsPlayer;

        float sizeValue = ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.SizeIsRange ?
            ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityRange : ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAOESize;

        Vector3 abilitySize = ThisWeaponData.editableAbilities[currentAbilityIndex].BaseAbilitySize;
        Vector3 pos = Vector3.zero;

        if (ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.ScaleXOnly)
        {
            abilitySize.x *= sizeValue;
        }
        else if (ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.ScaleZOnly)
        {
            abilitySize.z *= sizeValue;
        }
        else
        {
            abilitySize *= sizeValue;

            if (ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.ShapeType != PreviewShape.Shape.Cone)
            {
                abilitySize.y = ThisWeaponData.editableAbilities[currentAbilityIndex].BaseAbilitySize.y;
            }
        }

        switch (ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.ShapeType)
        {
            case PreviewShape.Shape.Square:
            case PreviewShape.Shape.Sphere:
                pos.z = abilitySize.z / 2;
                break;
            case PreviewShape.Shape.Cone:
                pos.z = abilitySize.y / 2;
                break;
        }

        activePreview.transform.localScale = abilitySize;

        if (ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.OrbitsPlayer)
        {
            activePreview.transform.localPosition = pos;
        }

        activePreview.SetActive(true);
    }

    /// <summary>
    /// The base inheritance for using weapon abilities
    /// </summary>
    virtual public void CastingAbility()
    {
        weaponAnimator.SetBool("AbilityCast", true);
        continuouslyUpdatePreview = false;
        attackReady = false;
        aimingAbilityOne = false;
        aimingAbilityTwo = false;



        StartCoroutine(AbilityDamage(currentAbilityIndex));

        StartCoroutine(AbilityEndLag(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityEndLag));
        StartCoroutine(AbilityDelay(currentAbilityIndex == abilityOneIndex, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown));
    }

    /// <summary>
    /// Contains functionality for how the ability behaves
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AbilityDamage(int abilityIndex)
    {
        GameObject tempAbility = Instantiate(abilityAOEs[currentAbilityIndex], activePreview.transform.position, activePreview.transform.rotation);
        if (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.OrbitsPlayer)
        {
            tempAbility.transform.parent = weaponModel.transform;
            tempAbility.transform.localScale = activePreview.transform.localScale;
        }
        activePreview.SetActive(false);

        yield return new WaitForSeconds(ThisWeaponData.editableAbilities[abilityIndex].AbilityDelay);

        int damage = ThisWeaponData.editableAbilities[abilityIndex].AbilityDuration > 0 ?
            Mathf.CeilToInt(ThisWeaponData.editableAbilities[abilityIndex].AbilityDamage / ThisWeaponData.editableAbilities[abilityIndex].AbilityDamageTicks)
            : ThisWeaponData.editableAbilities[abilityIndex].AbilityDamage;
        float delay = ThisWeaponData.editableAbilities[abilityIndex].AbilityDamageTicks > 0 ?
            ThisWeaponData.editableAbilities[abilityIndex].AbilityDuration / ThisWeaponData.editableAbilities[abilityIndex].AbilityDamageTicks : 0;
        float elapsed = 0;

        Collider[] targets;

        float r = 0;
        Vector3 center = Vector3.zero;
        Vector3 extents = Vector3.zero;

        switch (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.ShapeType)
        {
            case PreviewShape.Shape.Square:
            case PreviewShape.Shape.Cone:
                center = tempAbility.GetComponent<BoxCollider>().transform.TransformPoint(tempAbility.GetComponent<BoxCollider>().center);
                extents = Vector3.Scale(tempAbility.GetComponent<BoxCollider>().size, tempAbility.GetComponent<BoxCollider>().transform.lossyScale) * .5f;

                targets = Physics.OverlapBox(center, extents, tempAbility.GetComponent<BoxCollider>().transform.rotation,
                ThisWeaponData.editableAbilities[abilityIndex].AbilityLayerMask);
                break;
            case PreviewShape.Shape.Sphere:
                r = tempAbility.GetComponent<SphereCollider>() ? tempAbility.GetComponent<SphereCollider>().radius :
                    tempAbility.GetComponent<CapsuleCollider>().radius;
                targets = Physics.OverlapSphere(tempAbility.transform.position,
                    r * ThisWeaponData.editableAbilities[abilityIndex].AbilityAOESize,
                    ThisWeaponData.editableAbilities[abilityIndex].AbilityLayerMask);
                break;
            default:
                targets = new Collider[0];
                break;
        }

        if (!ThisWeaponData.editableAbilities[abilityIndex].PersistingAbility)
        {
            tempAbility.SetActive(false);
        }

        List<Collider> tars = new List<Collider>();

        do
        {
            tars.Clear();

            foreach (Collider t in targets)
            {
                if (t != null)
                {
                    Vector3 originPosition = ThisWeaponData.editableAbilities[abilityIndex].PreviewData.KnockBackFromPreviewCenter ? tempAbility.transform.position : this.transform.position;
                    Vector3 forceDir = new Vector3(t.transform.position.x - originPosition.x, 0,
                    t.transform.position.z - originPosition.z).normalized;

                    if (t.GetComponent<Enemy>())
                    {
                        //replace with the enemy taking damage
                        t.GetComponent<Rigidbody>().AddForce(ThisWeaponData.editableAbilities[abilityIndex].AbilityEnemyKnockback * forceDir, ForceMode.Impulse);
                        t.GetComponent<Enemy>().EnemyTakeDamage(damage);

                        if (ThisWeaponData.editableAbilities[abilityIndex].AbilityLifeSteal > 0)
                        {
                            int healValue = Mathf.CeilToInt(damage * ConvertToPercentage(ThisWeaponData.editableAbilities[abilityIndex].AbilityLifeSteal));

                            //TODO Replace with heal player
                            FindAnyObjectByType<MidRunDataManager>().ChangePlayerHealth(healValue);
                        }
                        Debug.Log(t.name + " took " + damage + " damage!");
                    }
                    else
                    {
                        t.GetComponentInParent<Rigidbody>().AddForce(ThisWeaponData.editableAbilities[abilityIndex].AbilityPlayerKnockback * forceDir, ForceMode.Impulse);
                    }
                    tars.Add(t);
                }
            }

            yield return new WaitForSeconds(delay);
            elapsed += delay;

            if (ThisWeaponData.editableAbilities[abilityIndex].PersistingAbility)
            {
                switch (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.ShapeType)
                {
                    case PreviewShape.Shape.Square:
                    case PreviewShape.Shape.Cone:
                        targets = Physics.OverlapBox(center, extents, tempAbility.GetComponent<BoxCollider>().transform.rotation,
                        ThisWeaponData.editableAbilities[abilityIndex].AbilityLayerMask);
                        break;
                    case PreviewShape.Shape.Sphere:
                        targets = Physics.OverlapSphere(tempAbility.transform.position,
                            r * ThisWeaponData.editableAbilities[abilityIndex].AbilityAOESize,
                            ThisWeaponData.editableAbilities[abilityIndex].AbilityLayerMask);
                        break;
                    default:
                        targets = new Collider[0];
                        break;
                }
            }
            else
            {
                targets = tars.ToArray();
            }

        } while (elapsed < ThisWeaponData.editableAbilities[abilityIndex].AbilityDuration);

        Destroy(tempAbility);
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
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(
            currentAbilityIndex == abilityOneIndex ?
            CooldownUIController.CooldownToUpdate.Ability1 : CooldownUIController.CooldownToUpdate.Ability2,
            ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown);

        if (isAbilityOne)
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
    #endregion

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
