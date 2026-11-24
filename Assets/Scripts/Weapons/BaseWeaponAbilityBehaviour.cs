/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/20/2026
 * Brief: This script now controlls the ability functionality for weapons.
 * This way, abilities can have changes made to them while someone else changes weapon attacks.
 * TODO: Add ability functionality as needed.
 * ***************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;

public class BaseWeaponAbilityBehaviour : BaseWeaponBehaviour
{
    [Header("Base Ability Variables"), HorizontalLine(height: 4, EColor.Red)]
    [SerializeField] protected List<GameObject> abilityAOEs = new List<GameObject>();

    protected GameObject activePreview;
    protected int currentAbilityIndex;

    protected bool continuouslyUpdatePreview;

    protected bool aimingAbilityOne;
    protected bool aimingAbilityTwo;

    protected bool abilityOneActive;
    protected bool abilityTwoActive;

    [SerializeField] public int abilityOneIndex;
    [SerializeField] public int abilityTwoIndex;

    [SerializeField] protected bool abilityOneReady;
    [SerializeField] protected bool abilityTwoReady;

    protected bool spawnAbilityOneProjectile;
    protected bool spawnAbilityTwoProjectile;

    protected float AbilityOneCD;
    protected float AbilityTwoCD;

    /// <summary>
    /// stops everything when it get's switched off
    /// </summary>
    public void DisableWeapon()
    {
        aimingAbilityOne = false;
        aimingAbilityTwo = false;
        activeWeapon = false;
        weaponModel.SetActive(false);
        if(activePreview != null )
            activePreview.SetActive(false);
        weaponAnimator.SetBool("AbilityCast", true);
    }

    /// <summary>
    /// turns everything on when the weapon is switched to
    /// </summary>
    public void EnableWeapon()
    {
        activeWeapon = true;
        weaponModel.SetActive(true);
        weaponAnimator.SetBool("AbilityCast", false);
        if (!abilityOneReady)
            UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability1, AbilityOneCD, ThisWeaponData.editableAbilities[abilityOneIndex].AbilityCooldown);
        else
            UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability1);
        if (!abilityTwoReady)
            UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability2, AbilityTwoCD, ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityCooldown);
        else
            UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability2);
    }

    /// <summary>
    /// Does the base weapon startup code and then sets up the abilities
    /// </summary>
    protected override void Start()
    {
        base.Start();

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

        //If we want an ability to start unreadied, it will start its cooldown
        if (!abilityOneReady)
        {
            StartCoroutine(AbilityDelay(abilityOneReady, ThisWeaponData.editableAbilities[abilityOneIndex].AbilityCooldown));
        }

        if (!abilityTwoReady)
        {
            StartCoroutine(AbilityDelay(abilityTwoReady, ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityCooldown));
        }
    }

    /// <summary>
    /// Enables the ability inputs
    /// </summary>
    protected override void OnEnable()
    {
        InputPublicEvents.AbilityOnePressed += AimingAbilityOne;
        InputPublicEvents.AbilityTwoPressed += AimingAbilityTwo;
        base.OnEnable();
    }

    /// <summary>
    /// Disables the ability inputs to prevent duplication errors
    /// </summary>
    protected override void OnDisable()
    {
        InputPublicEvents.AbilityOnePressed -= AimingAbilityOne;
        InputPublicEvents.AbilityTwoPressed -= AimingAbilityTwo;
        base.OnDisable();
    }

    /// <summary>
    /// Determines if the player is attacking or casting an ability
    /// </summary>
    protected override void PlayerAttacking()
    {
        if (aimingAbilityOne || aimingAbilityTwo)
        {
            //I intially put this at the start, but if you click the attack and then click and hold the attack button before it finishes, you won't attack until you hit the attack button again
            if (!okToAnimationCancel)
            {
                return;
            }

            SetAnimator(true);

            PlayAbilityCastAnimation();
        }
        else
        {
            isAttacking = true;
        }
    }

    /// <summary>
    /// If the animation uses a different animator than the current one, use that animator instead
    /// </summary>
    override protected void SetAnimator(bool isAbility = false)
    {
        if(isAbility)
        {
            if (ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimatorUsed != null && !weaponAnimator.Equals(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimatorUsed))
            {
                weaponAnimator = ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimatorUsed;
            }
        }
        else
        {
            base.SetAnimator();
        }
    }

    /// <summary>
    /// Determines which ability is being cast and plays their animation
    /// </summary>
    protected void PlayAbilityCastAnimation()
    {
        if (aimingAbilityOne)
        {
            abilityOneActive = true;
            if (ThisWeaponData.editableAbilities[abilityOneIndex].AbilityBurst.HasRangeAttack)
            {
                spawnAbilityOneProjectile = true;
            }
        }
        else
        {
            abilityTwoActive = true;
            if (ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityBurst.HasRangeAttack)
            {
                spawnAbilityTwoProjectile = true;
            }
        }

        weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
        okToAnimationCancel = false;
    }

    /// <summary>
    /// Allows the preview to follow the spot in the world the player aims at, if the preview should do that.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (!activeWeapon)
        {
            return;
        }

        base.FixedUpdate();

        if (continuouslyUpdatePreview)
        {
            PreviewFollowAim();
        }
    }

    /// <summary>
    /// Has an ability preview move to where a player is aiming within a set distance
    /// </summary>
    protected void PreviewFollowAim()
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

    /// <summary>
    /// Anim event that will spawn the projectile of an ability or weapon
    /// </summary>
    public override void SpawnProjectile()
    {
        bool hasBurst;
        if(spawnAbilityOneProjectile || spawnAbilityTwoProjectile)
        {
            hasBurst = ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityBurst.BurstRangedAttack.AttackProjectiles.Count > 1 ? true : false;
        }
        else
        {
            hasBurst = ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks.Count > 1 ? true : false;
        }
        BurstData rData = SetBurstData(spawnAbilityOneProjectile, spawnAbilityTwoProjectile);
        int damage = SetAttackDamage(spawnAbilityOneProjectile, spawnAbilityTwoProjectile);

        Vector3 bulletDir = aimDir;
        bulletDir.y = 0;

        int pIndex;

        if (rData.BurstRangedAttack.RandomizeProjectile)
        {
            pIndex = Random.Range(0, rData.BurstRangedAttack.AttackProjectiles.Count);
        }
        else
        {
            pIndex = rData.BurstRangedAttack.ProjectileIndex;

            //Allows the list to wrap around
            rData.BurstRangedAttack.ProjectileIndex = pIndex + 1 < rData.BurstRangedAttack.AttackProjectiles.Count ? ++rData.BurstRangedAttack.ProjectileIndex : 0;
        }

        GameObject spawnedProjectile = Instantiate(rData.BurstRangedAttack.AttackProjectiles[pIndex].ProjectilePrefab, transform.position, Quaternion.LookRotation(bulletDir.normalized, Vector3.up));
        spawnedProjectile.GetComponent<BaseProjectileBehaviour>().SetData(rData, damage, ThisWeaponData.EnemyLayer);
        spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = bulletDir.normalized * rData.BurstRangedAttack.AttackProjectiles[pIndex].ProjectileSpeed;

        //If the weapon has multiple bursts fire them
        if (hasBurst)
        {
            burstAttacking = true;
            StartCoroutine(AttackBursts((!abilityOneActive && !abilityTwoActive)));
        }
        else
        {
            if (!spawnAbilityOneProjectile && !spawnAbilityTwoProjectile)
            {
                StartCoroutine(AttackDelay());
            }
            else if (spawnAbilityOneProjectile)
            {
                spawnAbilityOneProjectile = false;
                CastingAbility();
            }
            else
            {
                spawnAbilityTwoProjectile = false;
                CastingAbility();
            }
        }
    }

    /// <summary>
    /// Returns the burst data reference for either one of the abilities or the weapon attack
    /// </summary>
    /// <param name="abilityOne"></param>
    /// <param name="abilityTwo"></param>
    /// <returns></returns>
    protected BurstData SetBurstData(bool abilityOne, bool abilityTwo)
    {
        if(abilityOne)
        {
            return ThisWeaponData.editableAbilities[abilityOneIndex].AbilityBurst;
        }

        if(abilityTwo)
        {
            return ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityBurst;
        }

        return ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks[burstIndex];
    }

    /// <summary>
    /// Returns the amount of damage that either an ability or attack will do
    /// </summary>
    /// <param name="abilityOne"></param>
    /// <param name="abilityTwo"></param>
    /// <returns></returns>
    protected int SetAttackDamage(bool abilityOne, bool abilityTwo)
    {
        if (abilityOne)
        {
            return ThisWeaponData.editableAbilities[abilityOneIndex].AbilityDamage;
        }

        if (abilityTwo)
        {
            return ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityDamage;
        }

        return ThisWeaponData.ComboAttacks[comboIndex - 1].WeaponDamage;
    }

    /// <summary>
    /// Anim event that will call a melee attack of an ability or weapon
    /// </summary>
    public override void MeleeSwing()
    {
        bool hasBurst = ThisWeaponData.ComboAttacks[comboIndex - 1].BurstAttacks.Count > 1 ? true : false;
        BurstData rData = SetBurstData(abilityOneActive, abilityTwoActive);
        int damage = SetAttackDamage(abilityOneActive, abilityTwoActive);

        foreach (Collider c in meleeHitboxes)
        {
            if (c.enabled)
            {
                DealMeleeDamage(Physics.OverlapBox(c.bounds.center, c.bounds.extents, Quaternion.identity, ThisWeaponData.EnemyLayer), rData, damage);
            }
        }

        //If the weapon has multiple bursts fire them
        if (hasBurst)
        {
            burstAttacking = true;
            StartCoroutine(AttackBursts((!abilityOneActive && !abilityTwoActive)));
        }
        else
        {
            if (!abilityOneActive && !abilityTwoActive)
            {
                StartCoroutine(AttackDelay());
            }
            else if (abilityOneActive)
            {
                abilityOneActive = false;
                CastingAbility();
            }
            else
            {
                abilityTwoActive = false;
                CastingAbility();
            }
        }
    }

    /// <summary>
    /// Damages every valid target in the melee attack's hitboxes
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="rData"></param>
    /// <param name="damage"></param>
    protected void DealMeleeDamage(Collider[] targets, BurstData rData, int damage)
    {
        foreach (Collider t in targets)
        {
            if (t.GetComponent<Enemy>())
            {
                t.GetComponent<Enemy>().EnemyTakeDamage(damage);

                MeleeLifesteal(rData, damage);
            }
        }
    }

    /// <summary>
    /// If the melee attack has lifesteal, heal the player
    /// </summary>
    /// <param name="rData"></param>
    /// <param name="damage"></param>
    protected void MeleeLifesteal(BurstData rData, int damage)
    {
        if(!rData.HasLifesteal)
        {
            return;
        }

        int healValue = Mathf.CeilToInt(damage * ConvertToPercentage(rData.WeaponLifeSteal.LifestealPercent));

        FindAnyObjectByType<MidRunDataManager>().ChangePlayerHealth(healValue);
    }

    /// <summary>
    /// How the player aims ability one. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityOne()
    {
        //If in a state where we can't cast abilities, exit function
        if((inEndLag || !okToAnimationCancel) || (!abilityOneActive && !abilityOneReady))
        {
            return;
        }

        //If canceling an active ability, cancel ability then exit function
        if(ThisWeaponData.editableAbilities[abilityOneIndex].CancelOnRecast && abilityOneActive)
        {
            weaponAnimator.SetBool("AbilityOver", true);
            StartCoroutine(AbilityEndLag(ThisWeaponData.editableAbilities[abilityOneIndex].AbilityEndLag));
            StartCoroutine(AbilityDelay(true, ThisWeaponData.editableAbilities[abilityOneIndex].AbilityCooldown));
            return;
        }

        aimingAbilityOne = !aimingAbilityOne;
        aimingAbilityTwo = false;
        currentAbilityIndex = abilityOneIndex;
        if (ThisWeaponData.editableAbilities[abilityOneIndex].HasPreview)
        {
            if (aimingAbilityOne)
            {
                AimingAbility();
            }
            else
            {
                activePreview.SetActive(false);
            }
        }
        else
        {
            SetAnimator(true);

            abilityOneActive = true;
            if(ThisWeaponData.editableAbilities[abilityOneIndex].AbilityBurst.HasRangeAttack)
            {
                spawnAbilityOneProjectile = true;
            }
            weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
            okToAnimationCancel = false;
        }
    }

    /// <summary>
    /// How the player aims ability two. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityTwo()
    {
        //If in a state where we can't cast abilities, exit function
        if ((inEndLag || !okToAnimationCancel) || (!abilityTwoActive && !abilityTwoReady))
        {
            return;
        }

        //If canceling an active ability, cancel ability then exit function
        if (ThisWeaponData.editableAbilities[abilityTwoIndex].CancelOnRecast && abilityTwoActive)
        {
            weaponAnimator.SetBool("AbilityOver", true);
            StartCoroutine(AbilityEndLag(ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityEndLag));
            StartCoroutine(AbilityDelay(false, ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityCooldown));
            return;
        }

        aimingAbilityTwo = !aimingAbilityTwo;
        aimingAbilityOne = false;
        currentAbilityIndex = abilityTwoIndex;
        if (ThisWeaponData.editableAbilities[currentAbilityIndex].HasPreview)
        {
            if (aimingAbilityTwo)
            {
                AimingAbility();
            }
            else
            {
                activePreview.SetActive(false);
            }
        }
        else
        {
            SetAnimator(true);

            abilityTwoActive = true;
            if (ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityBurst.HasRangeAttack)
            {
                spawnAbilityTwoProjectile = true;
            }

            weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
            okToAnimationCancel = false;
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

        activePreview.SetActive(true);

        SetAnimator(true);

        weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
    }

    /// <summary>
    /// Anim event that shows the ability preview
    /// </summary>
    /// <param name="orbitsPlayer"></param>
    /// <param name="sizeIsRange"></param>
    public void DisplayAbilityPreview()
    {
        int abilityIndex = currentAbilityIndex; //This function gets called by an anim even, so that's why I'm not passing through a param to set abilityIndex

        continuouslyUpdatePreview = !ThisWeaponData.editableAbilities[abilityIndex].PreviewData.OrbitsPlayer;

        Vector3 abilitySize = SetAbilitySize(abilityIndex);
        Vector3 pos = Vector3.zero;

        switch (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.ShapeType)
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

        if (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.OrbitsPlayer)
        {
            activePreview.transform.localPosition = pos;
        }
    }

    /// <summary>
    /// Returns the scale of how big the ability should be
    /// </summary>
    /// <param name="abilityIndex"></param>
    /// <returns></returns>
    protected Vector3 SetAbilitySize(int abilityIndex)
    {
        AbilityBaseScriptable abilityData = ThisWeaponData.editableAbilities[abilityIndex];

        float sizeValue = abilityData.PreviewData.SizeIsRange ? abilityData.AbilityRange : abilityData.AbilityAOESize;
        Vector3 abilitySize = abilityData.BaseAbilitySize;

        if(!abilityData.PreviewData.ScaleXOnly && !abilityData.PreviewData.ScaleZOnly)
        {
            abilitySize *= sizeValue;
            abilitySize.y = abilityData.PreviewData.ShapeType != PreviewShape.Shape.Cone ? abilityData.BaseAbilitySize.y : abilitySize.y;
            return abilitySize;
        }

        abilitySize.x = abilityData.PreviewData.ScaleXOnly ? abilitySize.x * sizeValue : abilitySize.x;
        abilitySize.z = abilityData.PreviewData.ScaleZOnly ? abilitySize.z * sizeValue : abilitySize.z;

        return abilitySize;
    }

    /// <summary>
    /// The base inheritance for using weapon abilities
    /// </summary>
    virtual public void CastingAbility()
    {
        okToAnimationCancel = true;
        continuouslyUpdatePreview = false;
        inEndLag = true;
        aimingAbilityOne = false;
        aimingAbilityTwo = false;

        if(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityVFX != null 
            && ThisWeaponData.editableAbilities[currentAbilityIndex].PreviewData.OrbitsPlayer)
        {
            Instantiate(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityVFX, transform.position, transform.rotation);
        }

        if (ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityBurst.HasRangeAttack)
        {
            activePreview.SetActive(false);
            if(currentAbilityIndex == abilityOneIndex)
            {
                abilityOneActive = false;
            }
            else
            {
                abilityTwoActive = false;
            }
        }
        else
        {
            StartCoroutine(AbilityDamage(currentAbilityIndex));
        }

        StartCoroutine(AbilityEndLag(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityEndLag));
        StartCoroutine(AbilityDelay(currentAbilityIndex == abilityOneIndex, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown));
    }

    /// <summary>
    /// Contains functionality for how the ability behaves
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AbilityDamage(int abilityIndex)
    {
        if(!ThisWeaponData.editableAbilities[abilityIndex].CancelOnRecast)
        {
            if (currentAbilityIndex == abilityOneIndex)
            {
                abilityOneActive = false;
            }
            else
            {
                abilityTwoActive = false;
            }
        }
        GameObject tempAbility = Instantiate(abilityAOEs[currentAbilityIndex], activePreview.transform.position, activePreview.transform.rotation);

        GameObject tempAbilityVfx = new GameObject();

        Vector3 ls = tempAbility.transform.localScale;
        if (ThisWeaponData.editableAbilities[abilityIndex].PreviewData.OrbitsPlayer)
        {
            tempAbility.transform.parent = transform;
            tempAbility.transform.localScale = ls;
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

        if (ThisWeaponData.editableAbilities[abilityIndex].AbilityVFX != null)
        {
            if (!ThisWeaponData.editableAbilities[abilityIndex].PersistingAbility)
            {
                tempAbility.SetActive(false);
                Instantiate(ThisWeaponData.editableAbilities[abilityIndex].AbilityVFX, transform.position, transform.rotation);
            }
            else
            {
                tempAbilityVfx = Instantiate(ThisWeaponData.editableAbilities[abilityIndex].AbilityVFX, tempAbility.transform.position, tempAbility.transform.rotation);
                tempAbilityVfx.transform.parent = tempAbility.transform;

                Vector3 vfxScale = new Vector3(tempAbility.transform.localScale.x, tempAbility.transform.localScale.z, 1);
                var vfxRadius = tempAbilityVfx.GetComponentInChildren<ParticleSystem>().shape;
                vfxRadius.scale = vfxScale;
                vfxRadius.radius = r;
            }
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

                    if (t.TryGetComponent<Enemy>(out Enemy enemyHit))
                    {
                        //replace with the enemy taking damage
                        StartCoroutine(t.GetComponent<EnemyWalk>().KnockbackEnemy(forceDir, ThisWeaponData.editableAbilities[abilityIndex].AbilityEnemyKnockback));
                        //t.GetComponent<Rigidbody>().AddForce(ThisWeaponData.editableAbilities[abilityIndex].AbilityEnemyKnockback * forceDir, ForceMode.Impulse);
                        enemyHit.EnemyTakeDamage(damage);
                        enemyHit.InflictDecay(ThisWeaponData.editableAbilities[abilityIndex].AbilityBurst.AttackDecayEffect);

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

            if (ThisWeaponData.editableAbilities[abilityIndex].AbilityVFX != null && ThisWeaponData.editableAbilities[abilityIndex].PersistingAbility)
            {
                ParticleSystem[] pFXs = tempAbilityVfx.GetComponentsInChildren<ParticleSystem>();

                foreach (ParticleSystem i in pFXs)
                {
                    if (i.main.duration > (ThisWeaponData.editableAbilities[abilityIndex].AbilityDuration - elapsed))
                    {
                        var vfxLoop = i.main;
                        vfxLoop.loop = false;
                    }
                }
            }

        } while (elapsed < ThisWeaponData.editableAbilities[abilityIndex].AbilityDuration);

        if (ThisWeaponData.editableAbilities[abilityIndex].CancelOnRecast)
        {
            if (currentAbilityIndex == abilityOneIndex)
            {
                abilityOneActive = false;
            }
            else
            {
                abilityTwoActive = false;
            }
        }

        Destroy(tempAbility);
    }

    /// <summary>
    /// How long the player gets put in end lag after using an ability
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    virtual protected IEnumerator AbilityEndLag(float time)
    {
        attackReady = false;
        yield return new WaitForSeconds(time);
        inEndLag = false;
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
            ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown,
            ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown);

        if (isAbilityOne)
        {
            abilityOneReady = false;
            AbilityOneCD = time;
        }
        else
        {
            abilityTwoReady = false;
            AbilityTwoCD = time;
        }

        if (isAbilityOne)
        {
            while(AbilityOneCD > 0)
            {
                yield return new WaitForSeconds(0.1f);
                AbilityOneCD -= 0.1f;
            }
        }
        else
        {
            while (AbilityTwoCD > 0)
            {
                yield return new WaitForSeconds(0.1f);
                AbilityTwoCD -= 0.1f;
            }
        }
        

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
