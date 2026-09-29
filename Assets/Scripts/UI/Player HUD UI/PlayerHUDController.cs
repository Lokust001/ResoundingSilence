using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using NaughtyAttributes;

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

    [Button("Testing Take one damage")]
    private void TakeOneDamage()
    {
        currentPlayerHealth--;
        UpdatePlayerHealthBarValue(currentPlayerHealth);
    }

    private void Awake()
    {
        InputPublicEvents.AbilityOnePressed += Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed += Ability2Wrapper;
        InputPublicEvents.InteractPressed += DashWrapper;
        InputPublicEvents.ShootPressed += ImmediatelyRefreshAllCooldowns;

        currentPlayerHealth = maxHealthValue;
    }

    protected override void OnDestroy()
    {
        InputPublicEvents.AbilityOnePressed -= Ability1Wrapper;
        InputPublicEvents.AbilityTwoPressed -= Ability2Wrapper;
        InputPublicEvents.InteractPressed -= DashWrapper;
        InputPublicEvents.ShootPressed -= ImmediatelyRefreshAllCooldowns;
        base.OnDestroy();
        
    }

    private void DashWrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Dash, DashMaxCooldown);
    }

    private void Ability1Wrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability1, Ability1MaxCooldown);
    }

    private void Ability2Wrapper()
    {
        UIPublicEvents.UpdateHUDCooldownUI?.Invoke(CooldownUIController.CooldownToUpdate.Ability2, Ability2MaxCooldown);
    }

    private void ImmediatelyRefreshAllCooldowns()
    {
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Dash);
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability2);
        UIPublicEvents.ImmediatelyRefreshCooldown?.Invoke(CooldownUIController.CooldownToUpdate.Ability1);
    }

    private void UpdatePlayerHealthBarValue(float currentHealth, float MaxHealth = -1)
    {
        if (MaxHealth >= 0)
        {
            maxHealthValue = MaxHealth;
        }

        healthBarSlider.value = currentHealth / maxHealthValue;
    }
    #endregion
}
