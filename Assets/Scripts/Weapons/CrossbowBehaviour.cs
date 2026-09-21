/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/18/2026
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

    protected override void Attack()
    {
        base.Attack();
    }
}
