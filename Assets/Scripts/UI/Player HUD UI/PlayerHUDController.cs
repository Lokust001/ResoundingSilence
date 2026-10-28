/*
* Author: Tyler
* Contributors:
* Last Modified: 09/29/2026
* Summary: Controls the player's hud
* To Do:   N/A
*/

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MenuBase
{
    private enum Settings
    {
        None,
        HealthBar,
        EXPBar,
        Cooldown,
        Testing
    }

    [SerializeField]
    private Settings settings;

    [SerializeField, ShowIf(nameof(settings), Settings.Cooldown)]
    private List<CooldownUIController> cooldownUIElements;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private Image healthBarSlider;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private AnimationCurve healthBarAnimation;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private float AnimationTime = 1.0f;

    [SerializeField, ShowIf(nameof(settings), Settings.EXPBar)]
    private Image expBarSlider;

    #region TESTING

    [SerializeField]
    private float Ability1MaxCooldown;

    [SerializeField]
    private float Ability2MaxCooldown;

    [SerializeField]
    private float DashMaxCooldown;

    private Coroutine healthAnimCoroutine;

    /// <summary>
    /// debug command to force the ui to take one damage
    /// </summary>
    [Button("Testing Take one damage")]
    private void TakeOneDamage()
    {
        MidRunDataManager.Instance.ChangePlayerHealth(-10);
    }

    [Button("Testing gain one exp")]
    private void GainOneExp()
    {
        MidRunDataManager.Instance.GainXP(1);
    }

    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        InputPublicEvents.AbilityOnePressed += Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed += Ability2Wrapper;
        InputPublicEvents.InteractPressed += DashWrapper;
        InputPublicEvents.ShootPressed += ImmediatelyRefreshAllCooldowns;
        UIPublicEvents.UpdatePlayerHealthSlider += UpdatePlayerHealthBarValue;
        UIPublicEvents.UpdatePlayerEXPSlider += UpdatePlayerEXPBarValue;
    }

    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        InputPublicEvents.AbilityOnePressed -= Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed -= Ability2Wrapper;
        InputPublicEvents.InteractPressed -= DashWrapper;
        InputPublicEvents.ShootPressed -= ImmediatelyRefreshAllCooldowns;
        UIPublicEvents.UpdatePlayerHealthSlider -= UpdatePlayerHealthBarValue;
        UIPublicEvents.UpdatePlayerEXPSlider -= UpdatePlayerEXPBarValue;
    }

    #region Testing

    /// <summary>
    /// testing - turns on the dash cooldown
    /// </summary>
    private void DashWrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Dash, DashMaxCooldown);
    }

    /// <summary>
    /// testing - turns on the ability 1 cooldown
    /// </summary>
    private void Ability1Wrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability1, Ability1MaxCooldown);
    }

    /// <summary>
    /// testing - turns on the ability 2 cooldown
    /// </summary>
    private void Ability2Wrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability2, Ability2MaxCooldown);
    }

    /// <summary>
    /// testing - refreshes all the player's cooldowns
    /// </summary>
    private void ImmediatelyRefreshAllCooldowns()
    {
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Dash);
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability2);
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability1);
    }

    /// <summary>
    /// handles the updates to the player's ui health bar
    /// </summary>
    /// <param name="currentHealth">the current health value</param>
    /// <param name="MaxHealth">the max health value - only enter it if the player's max health changes</param>
    private void UpdatePlayerHealthBarValue()
    { 
        if(healthAnimCoroutine != null)
        {
            StopCoroutine(healthAnimCoroutine);
            healthAnimCoroutine = null;
        }
        healthAnimCoroutine = StartCoroutine(PlayerHealthAnimation());
    }

    /// <summary>
    /// Runs the player's health bar animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerHealthAnimation()
    {
        float timer = 0.0f;
        float currentHealthPercentage = healthBarSlider.fillAmount;
        float targetHealthPercentage = MidRunDataManager.Instance.CurrentPlayerHealth / MidRunDataManager.Instance.MaxPlayerHealth;
        

        while (timer < AnimationTime)
        {
            yield return null;
            
            healthBarSlider.fillAmount = Mathf.Lerp(
                MidRunDataManager.Instance.CurrentPlayerHealth / MidRunDataManager.Instance.MaxPlayerHealth, 
                currentHealthPercentage, 
                healthBarAnimation.Evaluate(timer / AnimationTime));

            if (healthBarSlider.fillAmount <= 0)
            {
                break;
            }
            
            timer += Time.deltaTime;
        }

        if (MidRunDataManager.Instance.CurrentPlayerHealth <= 0.0f)
        {
            MidRunDataManager.Instance.KillPlayer();
        }
    }

    /// <summary>
    /// Updates the players exp bar
    /// </summary>
    /// <param name="currentExp"></param>
    /// <param name="maxExp"></param>
    private void UpdatePlayerEXPBarValue()
    {
        //add animation
        expBarSlider.fillAmount = MidRunDataManager.Instance.CurrentPlayerEXP / MidRunDataManager.Instance.ExpNeededToLevelUp;
    }
    #endregion
    #endregion
}
