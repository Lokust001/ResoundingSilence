/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/27/2026
 * Brief: Calls the specific weapon attack
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

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

    [SerializeField] protected List<AbilityData> WeaponAbilities = new List<AbilityData>();

    protected GameObject activePreview;
    [SerializeField] protected int currentAbilityIndex;

    bool continuouslyUpdatePreview;

    protected Vector3 mousePos = Vector3.forward;

    protected float currentAttackSpeed;

    protected bool isAttacking;
    protected bool attackReady;

    bool burstAttacking;

    protected bool aimingAbilityOne;
    protected bool aimingAbilityTwo;

    [SerializeField] protected int abilityOneIndex;
    [SerializeField] protected int abilityTwoIndex;

    [SerializeField] protected bool abilityOneReady;
    [SerializeField] protected bool abilityTwoReady;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    //[Tooltip("Set true if you want ability one to be ready to use on spawn.")]
    //[SerializeField] protected bool abilityOneReady;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    //[Tooltip("How long the ability is on cooldown for.")]
    //[SerializeField] protected float abilityOneCooldown;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    //[Tooltip("How long the ability puts the player in end lag for.")]
    //[SerializeField] protected float abilityOneEndLag;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    //[Tooltip("How far from the player the ability can be cast.")]
    //[SerializeField] protected float abilityOneRange;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    //[Tooltip("Set true if you want ability two to be ready to use on spawn.")]
    //[SerializeField] protected bool abilityTwoReady;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    //[Tooltip("How long the ability is on cooldown for.")]
    //[SerializeField] protected float abilityTwoCooldown;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    //[Tooltip("How long the ability puts the player in end lag for.")]
    //[SerializeField] protected float abilityTwoEndLag;

    //[ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    //[Tooltip("How far from the player the ability can be cast.")]
    //[SerializeField] protected float abilityTwoRange;

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

        foreach(AbilityData ability in WeaponAbilities)
        {
            if(ability.HasPreview)
            {
                activePreview = ability.PreviewData.AbilityPreview;
            }
            ability.BaseAbilitySize = new Vector3(ability.PreviewData.AbilityPreview.transform.localScale.x, 
                ability.PreviewData.AbilityPreview.transform.localScale.y, ability.PreviewData.AbilityPreview.transform.localScale.z);
            SetAbilityStats(ability, ability.PrimaryStats);
            SetAbilityStats(ability, ability.SecondaryStats);
        }

        if (!abilityOneReady)
        {
            StartCoroutine(AbilityDelay(abilityOneReady, WeaponAbilities[abilityOneIndex].AbilityCooldown));
        }

        if(!abilityTwoReady)
        {
            StartCoroutine(AbilityDelay(abilityTwoReady, WeaponAbilities[abilityTwoIndex].AbilityCooldown));
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
    /// Sets the stats for each of the abilities
    /// </summary>
    virtual protected void SetAbilityStats(AbilityData ability, List<StatValues> stats)
    {
        foreach(StatValues value in stats)
        {
            switch(value.StatType)
            {
                case AbilityData.Stats.Damage:
                    ability.AbilityDamage = Mathf.CeilToInt(value.StatValue);
                    break;
                case AbilityData.Stats.DamageTicks:
                    ability.AbilityDamageTicks = Mathf.CeilToInt(value.StatValue);
                    break;
                case AbilityData.Stats.Duration:
                    ability.AbilityDuration = value.StatValue;
                    break;
                //case AbilityData.Stats.AggroRange:
                //    ability.AbilityAggroRange = value.StatValue;
                //    break;
                //case AbilityData.Stats.PlayerSpeed:s
                //    ability.AbilityPlayerSpeed = value.StatValue;
                //    break;
                //case AbilityData.Stats.WeaponSpeed:
                //    ability.AbilityWeaponSpeed = value.StatValue;
                //    break;
                case AbilityData.Stats.LifeSteal:
                    ability.AbilityLifeSteal = value.StatValue;
                    break;
                case AbilityData.Stats.AOESize:
                    ability.AbilityAOESize = value.StatValue;
                    break;
                case AbilityData.Stats.Delay:
                    ability.AbilityDelay = value.StatValue;
                    break;
                case AbilityData.Stats.EnemyKnockback:
                    ability.AbilityEnemyKnockback = value.StatValue;
                    break;
                case AbilityData.Stats.PlayerKnockback:
                    ability.AbilityPlayerKnockback = value.StatValue;
                    break;
                default:
                    Debug.Log(value.StatType);
                    break;
            }
        }
    }

    #region Attacking
    /// <summary>
    /// Determines if whether or not the player casts an ability or attacks with the weapon
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
        Plane p = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if(p.Raycast(ray, out float hit))
        {
            mousePos = ray.GetPoint(hit) - transform.position;
        }
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

        Vector3 lookDir = mousePos;
        Quaternion rot = Quaternion.RotateTowards(weaponModel.transform.rotation,
            Quaternion.LookRotation(lookDir), 20f);
        rot.x = 0;
        rot.z = 0;
        weaponModel.transform.rotation = rot;

        if(continuouslyUpdatePreview)
        {
            float staticY = activePreview.transform.position.y;
            Vector3 pos = transform.position + Vector3.ClampMagnitude(mousePos, WeaponAbilities[currentAbilityIndex].AbilityRange);
            pos.y = staticY;
            activePreview.transform.position = pos;
        }

        //Check if attack speed gets changed
        if(currentAttackSpeed != ThisWeaponData.AttackSpeed)
        {
            Debug.Log("Changing attack speed");
            currentAttackSpeed = ThisWeaponData.AttackSpeed;
            UpdateAttackSpeed();
        }

        if (isAttacking && attackReady)
        {
            Attack();
            attackReady = false;
            StartCoroutine(AttackDelay());
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
        for(int i = 1; i < ThisWeaponData.AttackBursts[comboIndex - 1]; ++i)
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
        Debug.Log(comboIndex - 1);
        Vector3 bulletDir = mousePos;
        bulletDir.y = 0;

        GameObject spawnedProjectile = Instantiate(ThisWeaponData.Projectiles[comboIndex - 1], transform.position, Quaternion.LookRotation(bulletDir.normalized, Vector3.up));
        spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(ThisWeaponData);
        spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * ThisWeaponData.ProjectileSpeed[comboIndex - 1];

        //If the weapon has multiple bursts fire them
        if(ThisWeaponData.AttackBursts[comboIndex - 1] > 1 && !burstAttacking)
        {
            burstAttacking = true;
            StartCoroutine(AttackBursts());
        }
    }

    /// <summary>
    /// Called by an animation event to see if a melee swing hit anything
    /// </summary>
    public void MeleeSwing()
    {
        foreach(Collider c in meleeHitboxes)
        {
            if(c.enabled)
            {
                Collider[] targets = Physics.OverlapBox(c.bounds.center, c.bounds.extents, Quaternion.identity, ThisWeaponData.EnemyLayer);

                foreach (Collider t in targets)
                {
                    if(t.GetComponent<Enemy>())
                    {
                        t.GetComponent<Enemy>().EnemyTakeDamage(ThisWeaponData.WeaponDamage[comboIndex - 1]);
                    }
                    Debug.Log(t.name + " took " + ThisWeaponData.WeaponDamage[comboIndex - 1] + " damage!");
                }
            }
        }
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
        if(comboIndex >= attackAnimations.Count)
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
            AimingAbility();
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
            AimingAbility();
        }
    }

    /// <summary>
    /// Displays the ability preview and plays the readying animation
    /// </summary>
    /// <param name="abilityOne"></param>
    virtual protected void AimingAbility()
    {
        activePreview.SetActive(false);

        if(WeaponAbilities[currentAbilityIndex].HasPreview)
        {
            activePreview = WeaponAbilities[currentAbilityIndex].PreviewData.AbilityPreview;
            weaponAnimator.Play(WeaponAbilities[currentAbilityIndex].AbilityReadyAnim.AnimClip.name, 0);
        }
        else
        {
            CastingAbility();
        }

        //activePreview.SetActive(abilityOne ? aimingAbilityOne : aimingAbilityTwo);
    }

    /// <summary>
    /// Anim event that shows the ability preview
    /// </summary>
    /// <param name="orbitsPlayer"></param>
    /// <param name="sizeIsRange"></param>
    public void DisplayAbilityPreview()
    {
        continuouslyUpdatePreview = !WeaponAbilities[currentAbilityIndex].PreviewData.OrbitsPlayer;

        float sizeValue = WeaponAbilities[currentAbilityIndex].PreviewData.SizeIsRange ? WeaponAbilities[currentAbilityIndex].AbilityRange : WeaponAbilities[currentAbilityIndex].AbilityAOESize;

        Vector3 abilitySize = WeaponAbilities[currentAbilityIndex].BaseAbilitySize;
        Vector3 pos = Vector3.zero;

        if (WeaponAbilities[currentAbilityIndex].PreviewData.ScaleXOnly)
        {
            abilitySize.x *= sizeValue;
        }
        else if(WeaponAbilities[currentAbilityIndex].PreviewData.ScaleZOnly)
        {
            abilitySize.z *= sizeValue;
        }
        else
        {
            abilitySize *= sizeValue;

            if (WeaponAbilities[currentAbilityIndex].PreviewData.ShapeType != PreviewShape.Shape.Cone)
            {
                abilitySize.y = WeaponAbilities[currentAbilityIndex].BaseAbilitySize.y;
            }
            //pos.z = abilitySize.z; //(abilitySize.z / 2);
        }

        switch(WeaponAbilities[currentAbilityIndex].PreviewData.ShapeType)
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

        if (WeaponAbilities[currentAbilityIndex].PreviewData.OrbitsPlayer)
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

        StartCoroutine(AbilityDamage());

        StartCoroutine(AbilityEndLag(WeaponAbilities[currentAbilityIndex].AbilityEndLag));
        StartCoroutine(AbilityDelay(true, WeaponAbilities[currentAbilityIndex].AbilityCooldown));

        //if (aimingAbilityOne)
        //{
        //    aimingAbilityOne = false;
        //    StartCoroutine(AbilityEndLag(WeaponAbilities[abilityOneIndex].AbilityEndLag));
        //    StartCoroutine(AbilityDelay(true, WeaponAbilities[abilityOneIndex].AbilityCooldown));
        //}
        //else if (aimingAbilityTwo)
        //{
        //    aimingAbilityTwo = false;
        //    StartCoroutine(AbilityEndLag(WeaponAbilities[abilityTwoIndex].AbilityEndLag));
        //    StartCoroutine(AbilityDelay(false, WeaponAbilities[abilityTwoIndex].AbilityCooldown));
        //}

        //Add the ability functionality in the actual weapon script
    }

    /// <summary>
    /// Contains functionality for how the ability behaves
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AbilityDamage()
    {
        GameObject tempAbility = null;
        if (WeaponAbilities[currentAbilityIndex].PersistingAbility)
        {
            tempAbility = Instantiate(activePreview, activePreview.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(WeaponAbilities[currentAbilityIndex].AbilityDelay);

        int damage = WeaponAbilities[currentAbilityIndex].AbilityDuration > 0 ? 
            Mathf.CeilToInt(WeaponAbilities[currentAbilityIndex].AbilityDamage / WeaponAbilities[currentAbilityIndex].AbilityDamageTicks)
            : WeaponAbilities[currentAbilityIndex].AbilityDamage;
        float delay = WeaponAbilities[currentAbilityIndex].AbilityDamageTicks > 0 ?
            WeaponAbilities[currentAbilityIndex].AbilityDuration / WeaponAbilities[currentAbilityIndex].AbilityDamageTicks : 0;
        float elapsed = 0;

        Collider[] targets;

        float r = 0;
        Vector3 center = Vector3.zero;
        Vector3 extents = Vector3.zero;

        switch (WeaponAbilities[currentAbilityIndex].PreviewData.ShapeType)
        {
            case PreviewShape.Shape.Square:
            case PreviewShape.Shape.Cone:
                center = activePreview.GetComponent<BoxCollider>().bounds.center;
                extents = activePreview.GetComponent<BoxCollider>().bounds.extents;

                targets = Physics.OverlapBox(center, extents, Quaternion.identity,
                WeaponAbilities[currentAbilityIndex].AbilityLayerMask);
                break;
            case PreviewShape.Shape.Sphere:
                r = activePreview.GetComponent<SphereCollider>() ? activePreview.GetComponent<SphereCollider>().radius :
                    activePreview.GetComponent<CapsuleCollider>().radius;
                targets = Physics.OverlapSphere(activePreview.transform.position,
                    r * WeaponAbilities[currentAbilityIndex].AbilityAOESize,
                    WeaponAbilities[currentAbilityIndex].AbilityLayerMask);
                break;
            default:
                targets = new Collider[0];
                break;
        }

        activePreview.SetActive(false);

        do
        {
            if (WeaponAbilities[currentAbilityIndex].PersistingAbility)
            {
                switch (WeaponAbilities[currentAbilityIndex].PreviewData.ShapeType)
                {
                    case PreviewShape.Shape.Square:
                    case PreviewShape.Shape.Cone:
                        targets = Physics.OverlapBox(center, extents, Quaternion.identity,
                        WeaponAbilities[currentAbilityIndex].AbilityLayerMask);
                        break;
                    case PreviewShape.Shape.Sphere:
                        targets = Physics.OverlapSphere(activePreview.transform.position,
                            r * WeaponAbilities[currentAbilityIndex].AbilityAOESize,
                            WeaponAbilities[currentAbilityIndex].AbilityLayerMask);
                        break;
                    default:
                        targets = new Collider[0];
                        break;
                }
            }

            foreach (Collider t in targets)
            {
                Vector3 forceDir = new Vector3(t.transform.position.x - activePreview.transform.position.x, 0,
                t.transform.position.z - activePreview.transform.position.z).normalized;

                if (t.GetComponent<Enemy>())
                {
                    //replace with the enemy taking damage
                    t.GetComponent<Rigidbody>().AddForce(WeaponAbilities[currentAbilityIndex].AbilityEnemyKnockback * forceDir, ForceMode.Impulse);
                    t.GetComponent<Enemy>().EnemyTakeDamage(damage);
                    Debug.Log(t.name + " took " + damage + " damage!");
                }
                else
                {
                    t.GetComponentInParent<Rigidbody>().AddForce(WeaponAbilities[currentAbilityIndex].AbilityPlayerKnockback * forceDir, ForceMode.Impulse);
                }
            }

            yield return new WaitForSeconds(delay);
            elapsed += delay;
        } while (elapsed < WeaponAbilities[currentAbilityIndex].AbilityDuration);

        if (WeaponAbilities[currentAbilityIndex].PersistingAbility)
        {
            Destroy(tempAbility);
        }
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
