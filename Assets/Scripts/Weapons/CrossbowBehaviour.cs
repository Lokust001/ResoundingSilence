/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/21/2026
 * Brief: Handles the crossbow's basic attacks and abilities.
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;

public class CrossbowBehaviour : BaseAimedWeaponBehaviour
{
    enum AbilityOne
    {
        SplinterShot,
        BombBlast,
        ScatterShot,
        StakeShot
    }

    enum AbilityTwo
    {
        SplinterShot,
        BombBlast,
        ScatterShot,
        StakeShot
    }

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [SerializeField] AbilityOne abilityOne;
    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [SerializeField] AbilityTwo abilityTwo;

    /// <summary>
    /// The crossbow's basic attack
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
    }

    /// <summary>
    /// Determines which ability the player is trying to cast it and then casts it
    /// </summary>
    protected override void CastingAbility()
    {
        base.CastingAbility();
    }
}
