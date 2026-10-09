/*
* Author: Tyler
* Contributors: Brad Dixon, Brenden
* Last Modified: 10/09/2026
* Summary: This is the base scriptable object for all weapon scriptable objects.
*          Handles the data for the weapons. Acts as a tool to make any kind of weapon.
* To Do:   Add more variables as needed. Change status effects as needed.
*/

using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        BaseWeaponData,
        StatusEffects,
        Abilities,
        UpgradeGrid
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

    [ShowIf(nameof(shownSettings), ShownSettings.UpgradeGrid)]
    [Tooltip("This is the grid that will be attached to this kind of weapon when it is made")]
    public UpgradeTileGrid upgradeGrid;

    #endregion

    #region Lore


    [ShowIf(nameof(shownSettings), ShownSettings.Lore)]
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("This is the name that will appear on all of the UI")]
    public string WeaponName;

    #endregion

    #region Weapon

    #region General Variables
    [HideInInspector]
    public List<int> WeaponDamage = new List<int>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<int> BaseWeaponDamage = new List<int>();

    [HideInInspector]
    public List<float> AttackCooldown = new List<float>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<float> BaseAttackCooldown = new List<float>();

    [HideInInspector]
    public List<float> MovementSpeedChange = new List<float>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("How much attacking should change the player's move speed by. Use negative values to make player slower. " +
        "Is a list in case the weapon's combo attacks should change the player's speed by a different amount.")]
    public List<float> BaseMovementSpeedChange = new List<float>();

    [HideInInspector]
    public List<Vector2> PlayerDisplacement = new List<Vector2>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("The direction, and by how much, the player should be moved by when attacking. " +
        "Is a list in case the weapon's combo attacks should change the player's speed by a different amount.")]
    public List<Vector2> BasePlayerDisplacement = new List<Vector2>();

    [HideInInspector]
    public List<int> AttackBursts = new List<int>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("When making an attack, how many times the attack should occur. Is a list in case the weapon's combo attacks should cause different amount of bursts.")]
    public List<int> BaseAttackBursts = new List<int>();

    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [Tooltip("Whether or not a weapon has lifesteal.")]
    public bool HasLifesteal;

    [ShowIf(nameof(WeaponLifeSteal))]
    [Tooltip("How much lifesteal a weapon has. Made as a list in case you want combo attacks to have multiple life steal values.")]
    public List<float> BaseLifestealAmount = new List<float>();

    [HideInInspector]
    public List<float> LifestealAmount = new List<float>();

    #endregion

    #region Ranged Variables
    [HideInInspector]
    public List<float> ProjectileSpeed = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile speeds in a combo.")]
    public List<float> BaseProjectileSpeed = new List<float>();

    [HideInInspector]
    public List<float> ProjectileLifetime = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile lifetimes in a combo.")]
    public List<float> BaseProjectileLifetime = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("Whether or not the weapon's attacks have pierce.")]
    public bool HasPierce;

    [HideInInspector]
    public List<float> PierceDamageFalloff = new List<float>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("How much damage is lost after piercing a target. Is a list in case you want pierce damage fall off to not be linear.")]
    public List<float> BasePierceDamageFalloff = new List<float>();

    [HideInInspector]
    public List<float> PierceLifetimeFalloff = new List<float>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("How much lifetime is lost after piercing a target. Is a list in case you want pierce lifetime fall off to not be linear.")]
    public List<float> BasePierceLifetimeFalloff = new List<float>();

    [HideInInspector]
    public List<int> PierceAmount = new List<int>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("Set if you want to cap how many enemies the projectile can pierce through. Set to -1 if infinite. " +
        "Made as a list in case you want combo attacks to pierce a different amount of enemies.")]
    public List<int> BasePierceAmount = new List<int>();

    [HideInInspector]
    public List<int> MinPierceDamage = new List<int>();

    [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    [Tooltip("The minimum amount of damage the weapon can be decreased to from piercing. If <= 0, will just destroy when damage is 0. " +
        "Made as a list in case you want combo attacks to have varying minimum pierce damage.")]
    public List<int> BaseMinPierceDamage = new List<int>();
    #endregion

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

    #region Abilities

    [ShowIf(nameof(shownSettings), ShownSettings.Abilities)]
    [Expandable]
    [SerializeField]
    private List<AbilityBaseScriptable> abilities = new();
    private List<AbilityBaseScriptable> editableAbilities = new();

    
    /// <summary>
    /// Initializes the editableabilities list if it hasnt already been ititialized. Returns the list of editable abilities
    /// </summary>
    /// <returns></returns>
    public List<AbilityBaseScriptable> GetAbilities()
    {
        if (editableAbilities.Count <= 0)
        {
            foreach (AbilityBaseScriptable ability in abilities)
            {
                editableAbilities.Add(ability.CreateNonRefCopy<AbilityBaseScriptable>());
            }
        }

        return editableAbilities;
    }
    #endregion

    /// <summary>
    /// Custom bool for multiple enum values
    /// </summary>
    /// <returns></returns>
    private bool RangedWeaponSettings()
    {
        return weaponType == WeaponType.Ranged && shownSettings == ShownSettings.BaseWeaponData;
    }

    /// <summary>
    /// Custom bool for naughty attribute show if
    /// </summary>
    /// <returns></returns>
    private bool WeaponLifeSteal()
    {
        return shownSettings == ShownSettings.BaseWeaponData && HasLifesteal;
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
    /// Sets weapon variables to its base values
    /// </summary>
    public void Awake()
    {
        WeaponDamage = BaseWeaponDamage.ToList();
        AttackCooldown = BaseAttackCooldown.ToList();
        MovementSpeedChange = BaseMovementSpeedChange.ToList();
        PlayerDisplacement = BasePlayerDisplacement.ToList();
        LifestealAmount = BaseLifestealAmount.ToList();
        ProjectileSpeed = BaseProjectileSpeed.ToList();
        ProjectileLifetime = BaseProjectileLifetime.ToList();
        PierceDamageFalloff = BasePierceDamageFalloff.ToList();
        PierceLifetimeFalloff = BasePierceLifetimeFalloff.ToList();
        PierceAmount = BasePierceAmount.ToList();
        upgradeGrid.attachedWeapon = this;
    }

    /// <summary>
    /// changes weapon damage based on the amount of damage buffs given to the weapon on the grid
    /// </summary>
    /// <param name="DamageBuff">Greater than one multiplies the base damage numbers</param>
    public void UpdateWeaponDamage(float DamageBuff)
    {
        for(int i = 0; i < WeaponDamage.Count; i++)
        {
            WeaponDamage[i] = Mathf.CeilToInt(BaseWeaponDamage[i] * DamageBuff);
        }
    }

    /// <summary>
    /// changes weapon attack speed based on the amount of dpeed buffs given to the weapon on the grid
    /// </summary>
    /// <param name="SpeedBoost">Less than one, multiplies the base attack cooldown</param>
    public void UpdateWeaponSpeed(float SpeedBoost)
    {
        for(int i = 0; i < AttackCooldown.Count; i++)
        {
            AttackCooldown[i] = BaseAttackCooldown[i] * SpeedBoost;
        }
    }

    /// <summary>
    /// changes how much lifesteal the weapon has based on the parameter. Parameter should be a percentage
    /// </summary>
    /// <param name="lifestealPercent"></param>
    public void UpdateLifestealPercent(float lifestealPercent)
    {
        for (int i = 0; i < AttackCooldown.Count; i++)
        {
            LifestealAmount[i] = BaseLifestealAmount[i] * lifestealPercent;
        }
    }
}
