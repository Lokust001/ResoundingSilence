/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/23/2026
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

    [Header("Unique Ability One Variables"), HorizontalLine(height: 4, EColor.Green)]
    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityOne)]
    [SerializeField] Abilities abilityOne;
    [Header("Unique Ability Two Variables"), HorizontalLine(height: 4, EColor.Green)]
    [ShowIf(nameof(abilitySettings), AbilitySettings.AbilityTwo)]
    [SerializeField] Abilities abilityTwo;

    bool showingAbilityOnePreview;
    bool showingAbilityTwoPreview;

    GameObject activePreview;

    #region Ability Variables

    #region Splinter Shot Variables

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How big the ability AOE is.")]
    [SerializeField] float splinterShotAOE;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How much time must elapse between ticks of damage.")]
    [SerializeField] float splinterShotDOTTickDelay;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How long the DOT effects lasts.")]
    [SerializeField] float splinterShotDOTDuration;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject splinterShotPreview;

    #endregion

    #region Bomb Blast Variables

    [ShowIf(nameof(ViewingBombBlast))]
    [Tooltip("How big the ability AOE is.")]
    [SerializeField] float bombBlastAOE;

    [ShowIf(nameof(ViewingBombBlast))]
    [Tooltip("How far the player gets launched.")]
    [SerializeField] float playerLaunchDistance;

    [ShowIf(nameof(ViewingBombBlast))]
    [Tooltip("How far the player gets launched.")]
    [SerializeField] float enemyLaunchDistance;

    [ShowIf(nameof(ViewingBombBlast))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject bombBlastPreview;

    #endregion

    #region Scatter Shot Variables

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("The angle at which the scatter shot will spread.")]
    [SerializeField] float scatterShotCone;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("How much scatter shot should life steal for.")]
    [SerializeField] float scatterShotLifeSteal;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject scatterShotPreview;

    #endregion

    #region Stake Shot Variables

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How much percent damage the stake shot does per tick.")]
    [SerializeField] float stakeShotDOTPercentage;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How much the stake shot DOT life steals for.")]
    [SerializeField] float stakeShotLifeStealAmount;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How much time must elapse between ticks of damage.")]
    [SerializeField] float stakeShotDOTTickDelay;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How long the DOT effects lasts.")]
    [SerializeField] float stakeShotDOTDuration;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject stakeShotPreview;

    #region Custom ShowIf Bools

    /// <summary>
    /// Custom bool to see if the inspector is actively viewing the Splinter Shot ability
    /// </summary>
    /// <returns></returns>
    private bool ViewingSplinterShot()
    {
        return ((abilitySettings == AbilitySettings.AbilityOne && abilityOne == Abilities.SplinterShot) ||
            (abilitySettings == AbilitySettings.AbilityTwo && abilityTwo == Abilities.SplinterShot));
    }

    /// <summary>
    /// Custom bool to see if the inspector is actively viewing the Bomb Blast ability
    /// </summary>
    /// <returns></returns>
    private bool ViewingBombBlast()
    {
        return ((abilitySettings == AbilitySettings.AbilityOne && abilityOne == Abilities.BombBlast) ||
            (abilitySettings == AbilitySettings.AbilityTwo && abilityTwo == Abilities.BombBlast));
    }

    /// <summary>
    /// Custom bool to see if the inspector is actively viewing the Scatter Shot ability
    /// </summary>
    /// <returns></returns>
    private bool ViewingScatterShot()
    {
        return ((abilitySettings == AbilitySettings.AbilityOne && abilityOne == Abilities.ScatterShot) ||
            (abilitySettings == AbilitySettings.AbilityTwo && abilityTwo == Abilities.ScatterShot));
    }

    /// <summary>
    /// Custom bool to see if the inspector is actively viewing the Stake Shot ability
    /// </summary>
    /// <returns></returns>
    private bool ViewingStakeShot()
    {
        return ((abilitySettings == AbilitySettings.AbilityOne && abilityOne == Abilities.StakeShot) ||
            (abilitySettings == AbilitySettings.AbilityTwo && abilityTwo == Abilities.StakeShot));
    }

    #endregion

    #endregion

    #endregion

    /// <summary>
    /// Sets the activePreview variable to a preview to avoid null ref.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        CastScatterShot();
        CastStakeShot();

        activePreview = splinterShotPreview;
    }

    /// <summary>
    /// The crossbow's basic attack
    /// </summary>
    protected override void Attack()
    {
        base.Attack();
    }

    /// <summary>
    /// Shows the preview for ability one's attack
    /// </summary>
    protected override void AimingAbilityOne()
    {
        if (abilityOneReady)
        {
            ShowAbilityPreview(true);
        }

        base.AimingAbilityOne();
    }

    /// <summary>
    /// Shows the preview for ability two's attack
    /// </summary>
    protected override void AimingAbilityTwo()
    {
        if (abilityTwoReady)
        {
            ShowAbilityPreview(false);
        }

        base.AimingAbilityTwo();
    }

    /// <summary>
    /// Displays the corresponding preview when an ability is selected
    /// </summary>
    /// <param name="pressedAbilityOne"></param> Let's us know which ability preview to show/hide
    private void ShowAbilityPreview(bool pressedAbilityOne)
    {
        switch (pressedAbilityOne == true ? abilityOne : abilityTwo)
        {
            case Abilities.SplinterShot:

                if(activePreview != splinterShotPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = splinterShotPreview;
                break;
            case Abilities.BombBlast:
                if (activePreview != bombBlastPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = bombBlastPreview;
                break;
            case Abilities.ScatterShot:
                if (activePreview != scatterShotPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = scatterShotPreview;
                break;
            case Abilities.StakeShot:
                if (activePreview != stakeShotPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = stakeShotPreview;
                break;
        }

        if(activePreview.activeInHierarchy)
        {
            activePreview.SetActive(false);
        }
        else
        {
            activePreview.SetActive(true);
        }
    }

    /// <summary>
    /// Determines which ability the player is trying to cast it and then casts it
    /// </summary>
    protected override void CastingAbility()
    {
        Collider[] enemies = new Collider[0];

        switch (aimingAbilityOne == true ? abilityOne : abilityTwo)
        {
            case Abilities.SplinterShot:
            case Abilities.BombBlast:
                enemies = Physics.OverlapCapsule(activePreview.transform.position + Vector3.up, activePreview.transform.position,
                    activePreview.GetComponent<CapsuleCollider>().radius);
                break;
            case Abilities.ScatterShot:
            case Abilities.StakeShot:
                enemies = Physics.OverlapBox(activePreview.GetComponent<BoxCollider>().bounds.center, 
                    activePreview.GetComponent<BoxCollider>().bounds.extents);
                break;
        }

        foreach (Collider e in enemies)
        {
            Debug.Log("Hit " + e.gameObject.name);
        }

        activePreview.SetActive(false);

        base.CastingAbility();
    }

    /// <summary>
    /// Functinality for Splinter Shot ability
    /// </summary>
    private void CastSplinterShot()
    {
        float abilityRange = abilityOne == Abilities.SplinterShot ? abilityOneRange : abilityTwoRange;

        splinterShotPreview.transform.position = Vector3.ClampMagnitude(mousePos, abilityRange);
    }

    /// <summary>
    /// Functinality for Bomb Blast ability
    /// </summary>
    private void CastBombBlast()
    {
        float abilityRange = abilityOne == Abilities.BombBlast ? abilityOneRange : abilityTwoRange;

        bombBlastPreview.transform.position = Vector3.ClampMagnitude(mousePos, abilityRange);
    }

    /// <summary>
    /// Functinality for Scatter Shot ability
    /// </summary>
    private void CastScatterShot()
    {
        float abilityRange = abilityOne == Abilities.ScatterShot ? abilityOneRange : abilityTwoRange;

        Vector3 pos = scatterShotPreview.transform.position;
        pos.z = transform.position.z + ((abilityRange / 2) * 1.25f);
        scatterShotPreview.transform.position = pos;

        scatterShotPreview.transform.localScale = new Vector3(scatterShotCone, 
            abilityRange, scatterShotPreview.transform.localScale.z);
    }

    /// <summary>
    /// Functinality for Stake Shot ability
    /// </summary>
    private void CastStakeShot()
    {
        float abilityRange = abilityOne == Abilities.StakeShot ? abilityOneRange : abilityTwoRange;

        Vector3 pos = stakeShotPreview.transform.position;
        pos.z = transform.position.z + abilityRange / 2;
        stakeShotPreview.transform.position = pos;

        stakeShotPreview.transform.localScale = new Vector3(stakeShotPreview.transform.localScale.x, 
            stakeShotPreview.transform.localScale.y, abilityRange);
    }

    /// <summary>
    /// Handles showing how the ability is being aimed
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 lookDir = mousePos - transform.position;
        Quaternion rot = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(lookDir), 20f);
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;

        CastSplinterShot();
        CastBombBlast();
    }
}
