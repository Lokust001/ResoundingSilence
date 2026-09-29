/*
* Author: Tyler
* Contributors: Brad Dixon, Brenden
* Last Modified: 09/24/2026
* Summary: This is the base scriptable object for all weapon scriptable objects.
*          Handles the data for the weapons.
* To Do:   Add more variables as needed. Change status effects as needed.
*/

using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBaseWeapon", menuName = "Scriptables/Weapons/BaseWeapon")]
public class BaseWeaponScriptable : BaseScriptableObject
{
    #region GeneralData

    private enum WeaponType
    {
        Melee,
        Ranged
    }

    private enum EffectType
    {
        Decay,
        Slow,
        Weak,
        Shock,
        Burn
    }

    private enum ShownSettings
    {
        None,
        Lore,
        CombatData,
        StatusEffects
    }

    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Use to select what type of weapon this one will be.")]
    private WeaponType weaponType;

    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Changing between the options changes what variables are shown in the inspector.")]
    private ShownSettings shownSettings;

    [ShowIf(nameof(shownSettings), ShownSettings.StatusEffects)]
    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Use to select what type of status effects this weapon will inflict.")]
    private EffectType effectType;

    [SerializeField]
    [Tooltip("This is the grid that will be attached to this kind of weapon when it is made")]
    private UpgradeTileGrid upgradeGrid;

    #endregion

    #region Lore


    [ShowIf(nameof(shownSettings), ShownSettings.Lore)]
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("This is the name that will appear on all of the UI")]
    public string WeaponName;

    #endregion

    #region Weapon

    [HideInInspector]
    public List<int> WeaponDamage = new List<int>();

    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<int> BaseWeaponDamage = new List<int>();

    [HideInInspector]
    public List<float> AttackCooldown = new List<float>();

    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<float> BaseAttackCooldown = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile speeds in a combo.")]
    public List<float> ProjectileSpeed = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile lifetimes in a combo.")]
    public List<float> ProjectileLifetime = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How much shooting should slow the player by. " +
        "Is a list in case the weapon's combo attacks should slow down the player by a different amount.")]
    public List<float> MovementSlowdown = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("Whether or not the weapon's attacks have pierce.")]
    public bool HasPierce;

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("How much damage is lost after piercing a target. Is a list in case you want pierce damage fall off to not be linear.")]
    public List<float> PierceDamageFalloff = new List<float>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("How much lifetime is lost after piercing a target. Is a list in case you want pierce lifetime fall off to not be linear.")]
    public List<float> PierceLifetimeFalloff = new List<float>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("Set if you want to cap how many enemies the projectile can pierce through. Set to -1 if infinite. " +
        "Made as a list in case you want combo attacks to pierce a different amount of enemies.")]
    public List<int> PierceAmount = new List<int>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("The minimum amount of damage the weapon can be decreased to from piercing. If <= 0, will just destroy when damage is 0.")]
    public int MinPierceDamage;

    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("Whether or not a weapon has lifesteal.")]
    public bool HasLifesteal;

    [ShowIf(nameof(WeaponLifeSteal))]
    [Tooltip("How much lifesteal a weapon has.")]
    public float LifestealAmount;

    #endregion

    #region StatusEffects

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How much damage the decay does each time it ticks.")]
    public int DecayDamage;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("The max amount of decay stacks an enemy can have.")]
    public int MaxDecayStacks;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How much time must pass before the next tick of decay damage occurs.")]
    public float DecayDelay;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float DecayDuration;

    [ShowIf(nameof(ViewingSlowEffect))]
    [Tooltip("How much the enemy is slowed by.")]
    public float SlowStrength;

    [ShowIf(nameof(ViewingSlowEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float SlowDuration;

    [ShowIf(nameof(ViewingWeakEffect))]
    [Tooltip("How much the enemy's attack is reduced by.")]
    public float WeakStrength;

    [ShowIf(nameof(ViewingWeakEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float WeakDuration;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much damage the shock does each time it ticks.")]
    public int ShockDamage;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("The max amount of shock stacks an enemy can have.")]
    public int MaxShockStacks;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much time must pass before the next tick of shock damage occurs.")]
    public float ShockDelay;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float ShockDuration;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much time must pass before the effect chains damage to nearby enemies.")]
    public float ChainDelay;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How close enemies have to be to each other for the chain to connect.")]
    public float ChainRange;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("The max amount of enemies that can be hit by one chain.")]
    public int MaxChainCount;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much damage the shock does each time it ticks.")]
    public int BurnDamage;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("The max amount of shock stacks an enemy can have.")]
    public int MaxBurnStacks;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much time must pass before the next tick of shock damage occurs.")]
    public float BurnDelay;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float BurnDuration;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much time must pass before the effect tries to ignite the ground.")]
    public float IgniteDelay;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How big the AOE of the burning ground is.")]
    public float IgniteSize;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much the ignite chance increase by per stack.")]
    public int IgniteChancePerStack;

    #endregion

    /// <summary>
    /// Custom bool for multiple enum values
    /// </summary>
    /// <returns></returns>
    private bool RangedWeaponSettings()
    {
        return weaponType == WeaponType.Ranged && shownSettings == ShownSettings.CombatData;
    }

    /// <summary>
    /// Custom bool for naughty attribute show if
    /// </summary>
    /// <returns></returns>
    private bool WeaponLifeSteal()
    {
        return shownSettings == ShownSettings.CombatData && HasLifesteal;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingDecayEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Decay;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingSlowEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Slow;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingWeakEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Weak;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingShockEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Shock;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingBurnEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Burn;
    }

    /// <summary>
    /// When the weapon is initialised make sure weapon damage is set to numbers
    /// </summary>
    public void Awake()
    {
        WeaponDamage = BaseWeaponDamage;
        AttackCooldown = BaseAttackCooldown;
    }

    /// <summary>
    /// changes weapon damage based on the amount of damage buffs given to the weapon on the grid
    /// </summary>
    /// <param name="DamageBuff">Greater than one multiplies the base damage numbers</param>
    public void updateWeaponDamage(float DamageBuff)
    {
        for(int i = 0; i < WeaponDamage.Count; i++)
        {
            WeaponDamage[i] = Mathf.CeilToInt(BaseWeaponDamage[i] * DamageBuff);
        }
    }

    /// <summary>
    /// changes weapon attack speed based on the amount of dpeed buffs given to the weapon on the grid
    /// </summary>
    /// <param name="DamageBuff">Less than one, multiplies the base attack cooldown</param>
    public void updateWeaponSpeed(float SpeedBoost)
    {
        for(int i = 0; i < AttackCooldown.Count; i++)
        {
            AttackCooldown[i] = Mathf.FloorToInt(BaseAttackCooldown[i] * SpeedBoost);
        }
    }
}
