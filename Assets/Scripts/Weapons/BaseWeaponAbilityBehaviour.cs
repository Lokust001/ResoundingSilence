/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 11/10/2026
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

    [SerializeField] protected GameObject activePreview;
    [SerializeField] protected int currentAbilityIndex;

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

    /// <summary>
    /// stops everything when it get's switched off
    /// </summary>
    public void DisableWeapon()
    {
        aimingAbilityOne = false;
        aimingAbilityTwo = false;
        activeWeapon = false;
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
            if (ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimatorUsed != null && !weaponAnimator.Equals(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimatorUsed))
            {
                weaponAnimator = attackAnimations[comboIndex - 1].AnimatorUsed;
            }

            if(aimingAbilityOne)
            {
                abilityOneActive = true;
                if (ThisWeaponData.editableAbilities[abilityOneIndex].HasProjectile)
                {
                    spawnAbilityOneProjectile = true;
                }
            }
            else
            {
                abilityTwoActive = true;
                if (ThisWeaponData.editableAbilities[abilityTwoIndex].HasProjectile)
                {
                    spawnAbilityTwoProjectile = true;
                }
            }

            weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
        }
        else
        {
            isAttacking = true;
        }
    }

    /// <summary>
    /// Allows the preview to follow the spot in the world the player aims at, if the preview should do that.
    /// </summary>
    protected override void FixedUpdate()
    {
        if (activeWeapon)
        {
            base.FixedUpdate();

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
        }
    }

    /// <summary>
    /// Anim event that will spawn the projectile of an ability or weapon
    /// </summary>
    public override void SpawnProjectile(int cIndex)
    {
        bool hasBurst = ThisWeaponData.ComboAttacks[cIndex].BurstAttacks.Count > 1 ? true : false;
        BurstData rData;
        int damage;

        //Determines if the projectile being spawned belongs to an ability or the weapon attack
        if (spawnAbilityOneProjectile)
        {
            rData = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityDamage;
        }
        else if (spawnAbilityTwoProjectile)
        {
            rData = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityDamage;
        }
        else
        {
            rData = ThisWeaponData.ComboAttacks[cIndex].BurstAttacks[burstIndex];
            damage = ThisWeaponData.ComboAttacks[cIndex].WeaponDamage;
        }

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
            if (spawnAbilityOneProjectile)
            {
                spawnAbilityOneProjectile = false;
                CastingAbility();
            }
            if (spawnAbilityTwoProjectile)
            {
                spawnAbilityTwoProjectile = false;
                CastingAbility();
            }
        }
    }

    /// <summary>
    /// Anim event that will call a melee attack of an ability or weapon
    /// </summary>
    public override void MeleeSwing(int cIndex)
    {
        bool hasBurst = ThisWeaponData.ComboAttacks[cIndex].BurstAttacks.Count > 1 ? true : false;
        BurstData rData;
        int damage;

        //Determines if the projectile being spawned belongs to an ability or the weapon attack
        if (abilityOneActive)
        {
            rData = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityOneIndex].AbilityDamage;
        }
        else if (abilityTwoActive)
        {
            rData = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityProjectile;
            damage = ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityDamage;
        }
        else
        {
            rData = ThisWeaponData.ComboAttacks[cIndex].BurstAttacks[burstIndex];
            damage = ThisWeaponData.ComboAttacks[cIndex].WeaponDamage;
        }

        foreach (Collider c in meleeHitboxes)
        {
            if (c.enabled)
            {
                Collider[] targets = Physics.OverlapBox(c.bounds.center, c.bounds.extents, Quaternion.identity, ThisWeaponData.EnemyLayer);

                foreach (Collider t in targets)
                {
                    if (t.GetComponent<Enemy>())
                    {
                        t.GetComponent<Enemy>().EnemyTakeDamage(damage);

                        if (rData.HasLifesteal)
                        {
                            int healValue = Mathf.CeilToInt(damage * ConvertToPercentage(rData.WeaponLifeSteal.LifestealPercent));

                            //TODO Replace with heal player
                            FindAnyObjectByType<MidRunDataManager>().ChangePlayerHealth(healValue);
                        }
                    }
                    Debug.Log(t.name + " took " + damage + " damage!");
                }
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
            if (abilityOneActive)
            {
                abilityOneActive = false;
                CastingAbility();
            }
            if (abilityTwoActive)
            {
                abilityTwoActive = false;
                CastingAbility();
            }
        }
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
            currentAbilityIndex = abilityOneIndex;
            if (ThisWeaponData.editableAbilities[abilityOneIndex].HasPreview)
            {
                AimingAbility();
            }
            else
            {
                if (ThisWeaponData.editableAbilities[abilityOneIndex].AbilityAnim.AnimatorUsed != null && !weaponAnimator.Equals(ThisWeaponData.editableAbilities[abilityOneIndex].AbilityAnim.AnimatorUsed))
                {
                    weaponAnimator = attackAnimations[comboIndex - 1].AnimatorUsed;
                }

                abilityOneActive = true;
                if(ThisWeaponData.editableAbilities[abilityOneIndex].HasProjectile)
                {
                    spawnAbilityOneProjectile = true;
                }
                weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
                //CastingAbility();
            }
        }
        else if (ThisWeaponData.editableAbilities[abilityOneIndex].CancelOnRecast && abilityOneActive)
        {
            weaponAnimator.SetBool("AbilityOver", true);
        }
    }

    /// <summary>
    /// How the player aims ability two. Also sets whether or not the player is aiming the ability
    /// </summary>
    virtual protected void AimingAbilityTwo()
    {
        if (abilityTwoReady && activeWeapon)
        {
            aimingAbilityTwo = !aimingAbilityTwo;
            aimingAbilityOne = false;
            currentAbilityIndex = abilityTwoIndex;
            if (ThisWeaponData.editableAbilities[currentAbilityIndex].HasPreview)
            {
                AimingAbility();
            }
            else
            {
                if (ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityAnim.AnimatorUsed != null && !weaponAnimator.Equals(ThisWeaponData.editableAbilities[abilityTwoIndex].AbilityAnim.AnimatorUsed))
                {
                    weaponAnimator = attackAnimations[comboIndex - 1].AnimatorUsed;
                }

                abilityTwoActive = true;
                if (ThisWeaponData.editableAbilities[abilityTwoIndex].HasProjectile)
                {
                    spawnAbilityTwoProjectile = true;
                }

                weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
                //CastingAbility();
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

        if (ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.AnimatorUsed != null && !weaponAnimator.Equals(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.AnimatorUsed))
        {
            weaponAnimator = attackAnimations[comboIndex - 1].AnimatorUsed;
        }

        weaponAnimator.Play(ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.AnimClip.name, ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityReadyAnim.LayerIndex);
    }

    /// <summary>
    /// Anim event that shows the ability preview
    /// </summary>
    /// <param name="orbitsPlayer"></param>
    /// <param name="sizeIsRange"></param>
    public void DisplayAbilityPreview()
    {
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
        continuouslyUpdatePreview = false;
        attackReady = false;
        aimingAbilityOne = false;
        aimingAbilityTwo = false;

        if (ThisWeaponData.editableAbilities[currentAbilityIndex].HasProjectile)
        {
            activePreview.SetActive(false);
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
        GameObject tempAbility = Instantiate(abilityAOEs[currentAbilityIndex], activePreview.transform.position, activePreview.transform.rotation);
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
            ThisWeaponData.editableAbilities[currentAbilityIndex].AbilityCooldown,
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
}
