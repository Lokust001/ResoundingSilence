using System;
using UnityEditor.MPE;
using UnityEngine;

public class WeaponStatBuffs : MonoBehaviour
{
    public BaseWeaponScriptable weaponScriptable;

    #region Player Stat Buffs
    #endregion

    #region Pin Stat buffs
    #region PlayerStats
    PlayerController playerController = null;
    float DashBuff;
    float DamageResistance;
    float MoveSpeedBuff;
    float HealingPotency;
    float knockbackAmount;
    float regenAmount;
    #endregion

    #region WeaponStats
    float DamageBuff;
    float AttackSpeedBoost;
    float lifestealPercent;
    float AbilityDamage;
    float AbilityCooldownReduction;
    float BurstDamage;
    float ProjectileSpeedBuff;
    #endregion
    #endregion

    private void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        GenericPublicEvents.SetCurrentWeapon += setStatBuffs;
    }

    private void OnDestroy()
    {
        GenericPublicEvents.SetCurrentWeapon -= setStatBuffs;
    }


    private void setStatBuffs(BaseWeaponBehaviour behaviour)
    {
        if (behaviour.ThisWeaponData == weaponScriptable)
        {
            sendPlayerModifiers();
            sendWeaponMods();
        }
    }

    #region Player Functions
    /// <summary>
    /// takes and stores the player modifiers
    /// </summary>
    /// <param name="dash"></param>
    /// <param name="DR"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="healing"></param>
    /// <param name="kb"></param>
    /// <param name="regen"></param>
    public void getPlayerModifiers(float dash, float DR, float moveSpeed, float healing, float kb, float regen)
    {
        DashBuff = dash;
        DamageResistance = DR;
        MoveSpeedBuff = moveSpeed;
        HealingPotency = healing;
        knockbackAmount = kb;
        regenAmount = regen;
    }

    /// <summary>
    /// sends the player modifiers to the player
    /// </summary>
    public void sendPlayerModifiers()
    {
        playerController.DashDistance(DashBuff);
        playerController.DamageResistanceChange(DamageResistance);
        playerController.MoveSpeedIncrease(MoveSpeedBuff);
        playerController.healingPotencyChange(HealingPotency);
        playerController.KnockbackResistance(knockbackAmount);
        playerController.RegenBoost(regenAmount);
    }
    #endregion

    #region Weapon Functions

    public void getWeaponMods(float damageBuff, float attackSpeedBoost,float lifestealAmount, float abilityDamage, float abilityCooldown, float burst ,float projectileSpeedBuff)
    {
        DamageBuff = damageBuff;
        AttackSpeedBoost = attackSpeedBoost;
        lifestealPercent = lifestealAmount;
        AbilityDamage = abilityDamage;
        AbilityCooldownReduction = abilityCooldown;
        BurstDamage = burst;
        ProjectileSpeedBuff = projectileSpeedBuff;
    }
    
    public void sendWeaponMods()
    {
        UpdateWeaponDamage();
        UpdateWeaponSpeed();
        UpdateLifestealPercent();
        UpdateAbilityDamage();
        UpdateAbilityCooldown();
        AddBurst();
        ProjectileSpeedBoost();
    }

    /// <summary>
    /// changes weapon damage based on the amount of damage buffs given to the weapon on the grid
    /// </summary>
    public void UpdateWeaponDamage()
    {
        foreach (AttackData i in weaponScriptable.ComboAttacks)
        {
            i.WeaponDamage = Mathf.CeilToInt(i.BaseWeaponDamage * DamageBuff);
        }
    }

    /// <summary>
    /// changes weapon attack speed based on the amount of dpeed buffs given to the weapon on the grid
    /// </summary>
    public void UpdateWeaponSpeed()
    {
        weaponScriptable.AttackSpeed = weaponScriptable.AttackSpeed / AttackSpeedBoost;

        foreach (AttackData i in weaponScriptable.ComboAttacks)
        {
            i.AttackCooldown = i.BaseAttackCooldown * AttackSpeedBoost;
        }
    }

    /// <summary>
    /// changes how much lifesteal the weapon has based on the parameter. Parameter should be a percentage
    /// </summary>
    public void UpdateLifestealPercent()
    {
        foreach (AttackData i in weaponScriptable.ComboAttacks)
        {
            foreach (BurstData j in i.BurstAttacks)
            {
                //If adding lifesteal, let the attack know if has lifesteal now
                if (!j.HasLifesteal)
                {
                    j.HasLifesteal = true;
                    j.WeaponLifeSteal.LifestealPercent = lifestealPercent;
                    continue;
                }

                j.WeaponLifeSteal.LifestealPercent = j.WeaponLifeSteal.BaseLifestealPercent * lifestealPercent;
            }
        }
    }

    /// <summary>
    /// changes the amount of damage the abilities 
    /// </summary>
    public void UpdateAbilityDamage()
    {
        //editableAbilities[0].damage = editableAbilities[0].baseDamage * AbilityDamage;
        //editableAbilities[1].damage = editableAbilities[1].baseDamage * AbilityDamage;
    }

    /// <summary>
    /// Changes the cooldown of the abilities
    /// </summary>
    /// <param name="DamageBuff1"></param>
    /// <param name="DamageBuff2"></param>
    public void UpdateAbilityCooldown()
    {
        //editableAbilities[0].Cooldown = editableAbilities[0].baseCooldown * AbilityCooldownReduction;
        //editableAbilities[1].Cooldown = editableAbilities[1].baseCooldown * AbilityCooldownReduction;
    }

    /// <summary>
    /// adds a burst to the attack and adjusts burst damage
    /// </summary>
    public void AddBurst(int burstAttackIndex = 0)
    {
        foreach (AttackData i in weaponScriptable.ComboAttacks)
        {
            i.BurstAttacks = i.BaseBurstAttacks;
            i.BurstAttacks.Add(i.BurstAttacks[burstAttackIndex]);

            foreach (BurstData j in i.BurstAttacks)
            {
                j.BurstDamagePercent += (int)((BurstDamage * 100) - 100);
            }
        }
    }

    /// <summary>
    /// adds speed to the projectiles
    /// </summary>
    public void ProjectileSpeedBoost()
    {
        foreach (AttackData i in weaponScriptable.ComboAttacks)
        {
            foreach (BurstData j in i.BurstAttacks)
            {
                foreach (ProjectileData k in j.BurstRangedAttack.AttackProjectiles)
                {
                    k.ProjectileSpeed = k.BaseProjectileSpeed * ProjectileSpeedBuff;
                }
            }
        }
    }
    #endregion
}
