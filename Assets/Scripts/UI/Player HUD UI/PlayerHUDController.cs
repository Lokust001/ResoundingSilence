/*
* Author: Tyler
* Contributors:
* Last Modified: 09/29/2026
* Summary: Controls the player's hud
* To Do:   N/A
*/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDController : MenuBase
{
    [SerializeField]
    private List<CooldownUIController> cooldownUIElements;

    [SerializeField]
    private Slider healthBarSlider;

    [SerializeField]
    private float maxHealthValue = 5;

    [SerializeField]
    private float currentPlayerHealth;

    #region TESTING

    [SerializeField]
    private float Ability1MaxCooldown;

    [SerializeField]
    private float Ability2MaxCooldown;

    [SerializeField]
    private float DashMaxCooldown;

    /// <summary>
    /// debug command to force the ui to take one damage
    /// </summary>
    [Button("Testing Take one damage")]
    private void TakeOneDamage()
    {
        currentPlayerHealth--;
        UpdatePlayerHealthBarValue(currentPlayerHealth);
    }

    /// <summary>
    /// subscribes to the needed public events
    /// </summary>
    private void Awake()
    {
        InputPublicEvents.AbilityOnePressed += Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed += Ability2Wrapper;
        InputPublicEvents.InteractPressed += DashWrapper;
        InputPublicEvents.ShootPressed += ImmediatelyRefreshAllCooldowns;

        currentPlayerHealth = maxHealthValue;
    }

    /// <summary>
    /// unsubscribes from the public events
    /// </summary>
    protected override void OnDestroy()
    {
        InputPublicEvents.AbilityOnePressed -= Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed -= Ability2Wrapper;
        InputPublicEvents.InteractPressed -= DashWrapper;
        InputPublicEvents.ShootPressed -= ImmediatelyRefreshAllCooldowns;
        base.OnDestroy();

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
    private void UpdatePlayerHealthBarValue(float currentHealth, float MaxHealth = -1)
    {
        if (MaxHealth >= 0)
        {
            maxHealthValue = MaxHealth;
        }

        healthBarSlider.value = currentHealth / maxHealthValue;
    }
    #endregion
    #endregion
}
