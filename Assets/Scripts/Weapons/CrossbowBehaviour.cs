/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/23/2026
 * Brief: Handles the crossbow's basic attacks and abilities.
 * TODO: In the inspector, update the layer masks to include enemy layers
 * ***************************************************************************/
using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;

public class CrossbowBehaviour : BaseWeaponBehaviour
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

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("How much total damage the splinter shot will do.")]
    //[SerializeField] int splinterShotDamage;

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("How big the ability AOE is.")]
    //[SerializeField] float splinterShotAOESize;

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("How many times the splinter shot will try to damage.")]
    //[SerializeField] int splinterShotTotalDamageTicks;

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("How long the ability persists.")]
    //[SerializeField] float splinterShotDuration;

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("The preview so the player can see how their ability will look.")]
    //[SerializeField] GameObject splinterShotPreview;

    //[ShowIf(nameof(ViewingSplinterShot))]
    //[Tooltip("Which types of layers should be checked to damage with the splinter shot.")]
    //[SerializeField] LayerMask splinterShotLayerMask;

    #endregion

    #region Bomb Blast Variables

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("How much damage bomb blast does to enemies.")]
    //[SerializeField] int bombBlastDamage;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("How big the ability AOE is.")]
    //[SerializeField] float bombBlastAOESize;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("How long it takes for the bomb blast to detonate.")]
    //[SerializeField] float bombBlastDetTime;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("How far the player gets launched.")]
    //[SerializeField] float playerLaunchDistance;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("How far the player gets launched.")]
    //[SerializeField] float enemyLaunchDistance;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("The preview so the player can see how their ability will look.")]
    //[SerializeField] GameObject bombBlastPreview;

    //[ShowIf(nameof(ViewingBombBlast))]
    //[Tooltip("Which types of layers should be checked to launch with the bomb blast.")]
    //[SerializeField] LayerMask bombBlastLayerMask;

    #endregion

    #region Scatter Shot Variables

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("How much damage scatter shot does.")]
    [SerializeField] int scatterShotDamage;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("The angle at which the scatter shot will spread.")]
    [SerializeField] float scatterShotCone;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("How much scatter shot should life steal for.")]
    [SerializeField] float scatterShotLifeSteal;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("The preview so the player can see how their ability will look.")]
    [SerializeField] GameObject scatterShotPreview;

    [ShowIf(nameof(ViewingScatterShot))]
    [Tooltip("Which types of layers should be checked to hit with scatter shot.")]
    [SerializeField] LayerMask scatterShotLayerMask;

    #endregion

    #region Stake Shot Variables

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How much total damage the stake shot will do.")]
    [SerializeField] int stakeShotDamage;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How much the stake shot DOT life steals for.")]
    [SerializeField] float stakeShotLifeStealAmount;

    [ShowIf(nameof(ViewingStakeShot))]
    [Tooltip("How many ticks of damage the stake shot does.")]
    [SerializeField] int stakeShotTickCount;

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

        activePreview = WeaponAbilities[0].AbilityPreview;
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

                if(activePreview != WeaponAbilities[0].AbilityPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = WeaponAbilities[0].AbilityPreview;
                break;
            case Abilities.BombBlast:
                if (activePreview != WeaponAbilities[1].AbilityPreview)
                {
                    activePreview.SetActive(false);
                }

                activePreview = WeaponAbilities[1].AbilityPreview;
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
        WeaponAbilities[0].AbilityPreview.transform.localScale = new Vector3(WeaponAbilities[0].AbilityAOESize,
            WeaponAbilities[0].AbilityPreview.transform.localScale.y, WeaponAbilities[0].AbilityAOESize);

        float abilityRange = abilityOne == Abilities.SplinterShot ? WeaponAbilities[abilityOneIndex].AbilityRange : WeaponAbilities[abilityTwoIndex].AbilityRange;

        float staticY = WeaponAbilities[0].AbilityPreview.transform.position.y;
        Vector3 pos = transform.position + Vector3.ClampMagnitude(mousePos, abilityRange);
        pos.y = staticY;
        WeaponAbilities[0].AbilityPreview.transform.position = pos;
    }

    /// <summary>
    /// How the bomb blast preview looks
    /// </summary>
    private void BombBlastPreview()
    {
        WeaponAbilities[1].AbilityPreview.transform.localScale = new Vector3(WeaponAbilities[1].AbilityAOESize,
            WeaponAbilities[1].AbilityPreview.transform.localScale.y, WeaponAbilities[1].AbilityAOESize);

        float abilityRange = abilityOne == Abilities.BombBlast ? WeaponAbilities[abilityOneIndex].AbilityRange : WeaponAbilities[abilityTwoIndex].AbilityRange;

        float staticY = WeaponAbilities[1].AbilityPreview.transform.position.y;
        Vector3 pos = transform.position + Vector3.ClampMagnitude(mousePos, abilityRange);
        pos.y = staticY;
        WeaponAbilities[1].AbilityPreview.transform.position = pos;
    }

    /// <summary>
    /// How the scatter shot preview looks
    /// </summary>
    private void ScatterShotPreview()
    {
        float abilityRange = abilityOne == Abilities.ScatterShot ? WeaponAbilities[abilityOneIndex].AbilityRange : WeaponAbilities[abilityTwoIndex].AbilityRange;

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
        float abilityRange = abilityOne == Abilities.StakeShot ? WeaponAbilities[abilityOneIndex].AbilityRange : WeaponAbilities[abilityTwoIndex].AbilityRange;

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

        SplinterShotPreview();
        BombBlastPreview();
    }

    /// <summary>
    /// Determines which ability the player is trying to cast it and then casts it
    /// </summary>
    protected override void CastingAbility()
    {
        switch (aimingAbilityOne == true ? abilityOne : abilityTwo)
        {
            case Abilities.SplinterShot:
                StartCoroutine(CastingSplinterShot());
                break;
            case Abilities.BombBlast:
                StartCoroutine(CastingBombBlast());
                break;
            case Abilities.ScatterShot:
                CastingScatterShot();
                break;
            case Abilities.StakeShot:
                StartCoroutine(CastingStakeShot());
                break;
        }

        activePreview.SetActive(false);

        base.CastingAbility();
    }

    /// <summary>
    /// How the splinter shot ability interacts with the enemies
    /// </summary>
    private IEnumerator CastingSplinterShot()
    {
        GameObject tempAbility = Instantiate(WeaponAbilities[0].AbilityPreview, WeaponAbilities[0].AbilityPreview.transform.position, Quaternion.identity);

        int tickDamage = WeaponAbilities[0].AbilityDamage / WeaponAbilities[0].AbilityDamageTicks;
        //Stores the remainder damage in case the damage doesn't divide evenly
        int remainderDamage = WeaponAbilities[0].AbilityDamage % WeaponAbilities[0].AbilityDamageTicks;
        for(int i = 0; i < WeaponAbilities[0].AbilityDamageTicks; ++i)
        {
            tickDamage = i + 1 < stakeShotTickCount ? tickDamage : tickDamage + remainderDamage;

            Collider[] enemiesToHit = Physics.OverlapCapsule(tempAbility.transform.position + Vector3.up, tempAbility.transform.position,
                    tempAbility.GetComponent<CapsuleCollider>().radius * tempAbility.transform.localScale.x, WeaponAbilities[0].AbilityLayerMask);

            foreach(Collider enemyCollider in enemiesToHit)
            {
                if (enemyCollider.GetComponent<Enemy>())
                {
                    //replace with the enemy taking damage
                    enemyCollider.GetComponent<Enemy>().EnemyTakeDamage(tickDamage);
                }
            }

            yield return new WaitForSeconds(WeaponAbilities[0].AbilityDuration / WeaponAbilities[0].AbilityDamageTicks);
        }

        Destroy(tempAbility);
    }

    /// <summary>
    /// How the bomb blast ability interacts with player and enemies
    /// </summary>
    private IEnumerator CastingBombBlast()
    {
        GameObject tempAbility = Instantiate(WeaponAbilities[1].AbilityPreview, WeaponAbilities[1].AbilityPreview.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(WeaponAbilities[1].AbilityDelay);

        Collider[] entitiesToHit = Physics.OverlapCapsule(tempAbility.transform.position + Vector3.up, tempAbility.transform.position,
                    tempAbility.GetComponent<CapsuleCollider>().radius * WeaponAbilities[1].AbilityAOESize, WeaponAbilities[1].AbilityLayerMask);

        Debug.Log(entitiesToHit.Length);

        foreach (Collider entityCollider in entitiesToHit)
        {
            Vector3 forceDir = new Vector3(entityCollider.transform.position.x - tempAbility.transform.position.x, 0,
                entityCollider.transform.position.z - tempAbility.transform.position.z).normalized;
            Debug.Log(WeaponAbilities[1].AbilityEnemyKnockback * forceDir);
            if (entityCollider.GetComponent<Enemy>())
            {
                //replace with the enemy taking damage
                entityCollider.GetComponent<Enemy>().EnemyTakeDamage(WeaponAbilities[1].AbilityDamage);
                entityCollider.GetComponent<Rigidbody>().AddForce(WeaponAbilities[1].AbilityEnemyKnockback * forceDir, ForceMode.Impulse);
            }
            else //if(entityCollider.GetComponentInParent<PlayerController>())
            {
                entityCollider.GetComponentInParent<Rigidbody>().AddForce(WeaponAbilities[1].AbilityPlayerKnockback * forceDir, ForceMode.Impulse);
            }
        }

        Destroy(tempAbility);
    }

    /// <summary>
    /// How the scatter shot ability interacts with the enemies
    /// </summary>
    private void CastingScatterShot()
    {
        Collider[] enemiesHit = Physics.OverlapBox(activePreview.GetComponent<BoxCollider>().bounds.center,
                    activePreview.GetComponent<BoxCollider>().bounds.extents, Quaternion.identity, scatterShotLayerMask);

        foreach(Collider enemyCollider in enemiesHit)
        {
            if(enemyCollider.GetComponent<Enemy>())
            {
                //replace with the enemy taking damage
                enemyCollider.GetComponent<Enemy>().EnemyTakeDamage(scatterShotDamage);

                int healValue = Mathf.CeilToInt(scatterShotDamage * ConvertToPercentage(scatterShotLifeSteal));

                //TODO Replace with heal player
                //FindAnyObjectByType<DummyPlayerBehaviour>().Heal(healValue);
            }
        }
    }

    /// <summary>
    /// How the stake shot ability interacts with the enemies
    /// </summary>
    private IEnumerator CastingStakeShot()
    {
        Collider[] enemiesHit = Physics.OverlapBox(activePreview.GetComponent<BoxCollider>().bounds.center,
                    activePreview.GetComponent<BoxCollider>().bounds.extents, Quaternion.identity, scatterShotLayerMask);

        int tickDamage = stakeShotDamage / stakeShotTickCount;
        //Stores the remainder damage in case the damage doesn't divide evenly
        int remainderDamage = stakeShotDamage % stakeShotTickCount;

        List<Collider> stillAlive = new List<Collider>(enemiesHit);

        for (int i = 0; i < stakeShotTickCount; ++i)
        {
            tickDamage = i + 1 < stakeShotTickCount ? tickDamage : tickDamage + remainderDamage;

            enemiesHit = stillAlive.ToArray();
            stillAlive.Clear();

            foreach (Collider enemyCollider in enemiesHit)
            {
                if (enemyCollider.GetComponent<Enemy>())
                {
                    //replace with the enemy taking damage
                    enemyCollider.GetComponent<Enemy>().EnemyTakeDamage(tickDamage);

                    int healValue = Mathf.CeilToInt(tickDamage * ConvertToPercentage(scatterShotLifeSteal));

                    //TODO Replace with heal player
                    //FindAnyObjectByType<DummyPlayerBehaviour>().Heal(healValue);
                }

                yield return new WaitForFixedUpdate();

                if(enemyCollider != null)
                {
                    stillAlive.Add(enemyCollider);
                }
            }

            yield return new WaitForSeconds(stakeShotDOTDuration / stakeShotTickCount);
        }
    }
}
