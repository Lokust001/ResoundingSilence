/*
* Author: Tyler
* Contributors:
* Last Modified: 09/16/2026
* Summary: This is the base scriptable object for all weapon scriptable objects.
* To Do:   Add more variables as needed.
*/

using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseWeapon", menuName = "Scriptables/Weapons/BaseWeapon")]
public class BaseWeaponScriptable : BaseScriptableObject
{
    #region GeneralData

    //[System.Serializable]
    private enum ShownSettings
    {
        None,
        Lore,
        CombatData
    }

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
    [HorizontalLine(4, EColor.Red)]
    [Tooltip("This is how many times this weapon will attack per second.")]
    public float AttacksPerSecond;
    
    [ShowIf(nameof(shownSettings), ShownSettings.CombatData)]
    [Tooltip("This is how much damage the weapon will deal per swing/bullet")]
    public float DamagePerHit;

    #endregion
}
