/*
* Author: Tyler
* Contributors:
* Last Modified: 10/1/2026
* Summary: Scriptable object for the abilities. Will havbe specific abilities derive from this as needed, but these 
*          vars are on every ability scriptable
* To Do:   N/A
*/

using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptables/AbilityBase")]
public class AbilityBaseScriptable : BaseScriptableObject
{
    [Header("If you dont want to use any of these variables, set them to -1")]
    [HorizontalLine(4, EColor.Green), AllowNesting]
    public int damage;

    public float PrimaryStat;

    public float SecondaryStat;

    public float Cooldown;

    public float Duration;

    public float AOE;
}
