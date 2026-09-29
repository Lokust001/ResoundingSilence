/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/29/2026
 * Brief: Handles the crossbow's basic attacks and abilities.
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections;

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
    [Tooltip("How much total damage the splinter shot will do.")]
    [SerializeField] int splinterShotDamage;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How big the ability AOE is.")]
    [SerializeField] float splinterShotAOESize;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How many times the splinter shot will try to damage.")]
    [SerializeField] int splinterShotTotalDamageTicks;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("How long the ability persists.")]
    [SerializeField] float splinterShotDuration;

    [ShowIf(nameof(ViewingSplinterShot))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject splinterShotPreview;

    [SerializeField] bool splinterShotPlaced;

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

    bool bombBlastPlaced = false;

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

        ScatterShotPreview();
        StakeShotPreview();

        activePreview = splinterShotPreview;
        splinterShotPlaced = false;
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

    #region Ability Previews

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
    /// How the splinter shot preview looks
    /// </summary>
    private void SplinterShotPreview()
    {
        if (!splinterShotPlaced)
        {
            splinterShotPreview.transform.localScale = new Vector3(splinterShotAOESize,
                splinterShotPreview.transform.localScale.y, splinterShotAOESize);

            float abilityRange = abilityOne == Abilities.SplinterShot ? abilityOneRange : abilityTwoRange;

            splinterShotPreview.transform.position = Vector3.ClampMagnitude(mousePos, abilityRange);
        }
    }

    /// <summary>
    /// How the bomb blast preview looks
    /// </summary>
    private void BombBlastPreview()
    {
        if (!bombBlastPlaced)
        {
            float abilityRange = abilityOne == Abilities.BombBlast ? abilityOneRange : abilityTwoRange;

            bombBlastPreview.transform.position = Vector3.ClampMagnitude(mousePos, abilityRange);
        }
    }

    /// <summary>
    /// How the scatter shot preview looks
    /// </summary>
    private void ScatterShotPreview()
    {
        float abilityRange = abilityOne == Abilities.ScatterShot ? abilityOneRange : abilityTwoRange;

        Vector3 pos = scatterShotPreview.transform.position;
        pos.z = transform.position.z + ((abilityRange / 2) * 1.25f);
        scatterShotPreview.transform.position = pos;

        scatterShotPreview.transform.localScale = new Vector3(scatterShotCone, 
            abilityRange, scatterShotPreview.transform.localScale.z);
    }

    /// <summary>
    /// How the stake shot preview looks
    /// </summary>
    private void StakeShotPreview()
    {
        float abilityRange = abilityOne == Abilities.StakeShot ? abilityOneRange : abilityTwoRange;

        Vector3 pos = stakeShotPreview.transform.position;
        pos.z = transform.position.z + abilityRange / 2;
        stakeShotPreview.transform.position = pos;

        stakeShotPreview.transform.localScale = new Vector3(stakeShotPreview.transform.localScale.x, 
            stakeShotPreview.transform.localScale.y, abilityRange);
    }

    #endregion

    /// <summary>
    /// Handles showing how the ability is being aimed
    /// </summary>
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector3 lookDir = mousePos - transform.position;
        Quaternion rot = Quaternion.RotateTowards(weaponModel.transform.rotation,
            Quaternion.LookRotation(lookDir), 20f);
        rot.x = 0;
        rot.z = 0;
        weaponModel.transform.rotation = rot;

        SplinterShotPreview();
        BombBlastPreview();
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
                StartCoroutine(CastingSplinterShot());
                break;
            case Abilities.BombBlast:
                enemies = Physics.OverlapCapsule(activePreview.transform.position + Vector3.up, activePreview.transform.position,
                    activePreview.GetComponent<CapsuleCollider>().radius);
                break;
            case Abilities.ScatterShot:
                CastingScatterShot();
                break;
            case Abilities.StakeShot:
                enemies = Physics.OverlapBox(activePreview.GetComponent<BoxCollider>().bounds.center,
                    activePreview.GetComponent<BoxCollider>().bounds.extents);
                break;
        }

        //activePreview.SetActive(false);

        base.CastingAbility();
    }

    /// <summary>
    /// How the splinter shot ability interacts with the enemies
    /// </summary>
    private IEnumerator CastingSplinterShot()
    {
        splinterShotPlaced = true;
        int tickDamage = splinterShotDamage / splinterShotTotalDamageTicks;
        //Stores the remainder damage in case the damage doesn't divide evenly
        int remainderDamage = splinterShotDamage % splinterShotTotalDamageTicks;
        for(int i = 0; i < splinterShotTotalDamageTicks; ++i)
        {
            if(i + 1 == splinterShotTotalDamageTicks)
            {
                tickDamage += remainderDamage;
            }
            Collider[] enemiesToHit = Physics.OverlapCapsule(splinterShotPreview.transform.position + Vector3.up, splinterShotPreview.transform.position,
                    splinterShotPreview.GetComponent<CapsuleCollider>().radius);

            foreach(Collider enemyCollider in enemiesToHit)
            {
                if (enemyCollider.GetComponent<DummyBehaviour>())
                {
                    Debug.Log(enemyCollider.name + " took " + tickDamage + " damage!");
                }
            }

            yield return new WaitForSeconds(splinterShotDuration / splinterShotTotalDamageTicks);
        }

        splinterShotPreview.SetActive(false);
        splinterShotPlaced = false;
    }

    /// <summary>
    /// How the bomb blast ability interacts with player and enemies
    /// </summary>
    private void CastingBombBlast()
    {

    }

    /// <summary>
    /// How the scatter shot ability interacts with the enemies
    /// </summary>
    private void CastingScatterShot()
    {

    }

    /// <summary>
    /// How the stake shot ability interacts with the enemies
    /// </summary>
    private void CastingStakeShot()
    {

    }
}
