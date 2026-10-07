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
        Power,
        Vitality,
        Speed,
        Status
    }

    public enum WeaponStatToChange
    {
        none,
        BulletDamage,
        AttackSpeed,
        addDebuff,
        Lifesteal
    }

    [HorizontalLine(4, EColor.Red)]
    [Tooltip("The name of the pin")]
    public string PinName;

    [HorizontalLine(4, EColor.Indigo)]
    [Header("Pin Gameplay Values")]
    [Tooltip("The type of pin this is")]
    public PinType Type;

    [Tooltip("This is numerical value for how much to modify in %")]
    public float ModifierNumber;

    [Tooltip("How you want to change the weapon")]
    public WeaponStatToChange StatToChange;

    [Tooltip("How much (in %) this gains from having adjacent tiles be the same type")]
    public int AdjacentTileScaling;

    [Tooltip("If true the scaling number will be added per adjacent tile that matches the type, if false it will multiply the base modifier by 1 + the percent in adjacent scaling")]
    public bool AdditiveScaling = true;

    [HorizontalLine(4, EColor.Blue)]
    [Header("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    [Tooltip("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    public Color pinColor;
}
