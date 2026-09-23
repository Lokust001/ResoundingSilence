/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: Scriptable object for the pins. May need to make children later on depending on how complex the codebase gets
* To Do:   N/A
*/

using NaughtyAttributes;
using UnityEngine;
[CreateAssetMenu(fileName = "NewPin", menuName = "Scriptables/New Pin")]
public class PinScriptable : BaseScriptableObject
{
    public enum PinType
    {
        none,
        damage,
        buffDebuff
    }

    public enum WeaponStatToChange
    {
        none,
        bulletDamage,
        attackSpeed,
        addDebuff
    }

    [HorizontalLine(4, EColor.Indigo)]
    [Header("Pin Gameplay Values")]
    [Tooltip("The type of pin this is")]
    public PinType Type;

    [Tooltip("This is numerical value for how much to modify")]
    public float ModifierNumber;

    [Tooltip("How you want to change the weapon")]
    public WeaponStatToChange StatToChange;

    [HorizontalLine(4, EColor.Blue)]
    [Header("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    [Tooltip("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    public Color pinColor;

}
