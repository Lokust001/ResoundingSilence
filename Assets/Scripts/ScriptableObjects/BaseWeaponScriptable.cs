/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 09/18/2026
* Summary: This is the base scriptable object for all weapon scriptable objects.
*          Handles the data for the weapons.
* To Do:   Add more variables as needed.
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

    private enum ShownSettings
    {
        None,
        Lore,
        CombatData
    }

    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Use to select what type of weapon this one will be.")]
    private WeaponType weaponType;

    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Changing between the options changes what variables are shown in the inspector.")]
    private ShownSettings shownSettings;

    #endregion

    #region Lore

    
    [ShowIf(nameof(shownSettings), ShownSettings.Lore)]
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("This is the name that will appear on all of the UI")]
    public string WeaponName;

    #endregion

    #region Weapon

    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<int> WeaponDamage = new List<int>();

    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    public List<float> AttackCooldown = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile speeds in a combo.")]
    public List<float> ProjectileSpeed = new List<float>();

    [ShowIf(nameof(RangedWeaponSettings))]
    [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile lifetimes in a combo.")]
    public List<float> ProjectileLifetime = new List<float>();

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
}
