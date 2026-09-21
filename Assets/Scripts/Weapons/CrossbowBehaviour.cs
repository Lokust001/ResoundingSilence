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
    enum Abilities
    {
        SplinterShot,
        BombBlast,
        ScatterShot,
        StakeShot
    }

    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [SerializeField] Abilities abilityOne;
    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [SerializeField] Abilities abilityTwo;

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
        switch(aimingAbilityOne == true ? abilityOne : abilityTwo)
        {
            case Abilities.SplinterShot:
                CastSplinterShot();
                break;
            case Abilities.BombBlast:
                CastBombBlast();
                break;
            case Abilities.ScatterShot:
                CastScatterShot();
                break;
            case Abilities.StakeShot:
                CastStakeShot();
                break;
        }

        base.CastingAbility();
    }

    /// <summary>
    /// Functinality for Splinter Shot ability
    /// </summary>
    private void CastSplinterShot()
    {
        Debug.Log("Casting Splinter Shot");
    }

    /// <summary>
    /// Functinality for Bomb Blast ability
    /// </summary>
    private void CastBombBlast()
    {
        Debug.Log("Casting Bomb Blast");
    }

    /// <summary>
    /// Functinality for Scatter Shot ability
    /// </summary>
    private void CastScatterShot()
    {
        Debug.Log("Casting Scatter Shot");
    }

    /// <summary>
    /// Functinality for Stake Shot ability
    /// </summary>
    private void CastStakeShot()
    {
        Debug.Log("Casting Stake Shot");
    }
}
