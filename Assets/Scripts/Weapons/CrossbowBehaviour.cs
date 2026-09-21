/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/21/2026
 * Brief: Handles the crossbow's basic attacks and abilities.
 * TODO:
 * ***************************************************************************/
using UnityEngine;

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

    [SerializeField] AbilityOne abilityOne;
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
