using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUIController : MonoBehaviour
{
    public enum CooldownToUpdate
    {
        None,
        Dash,
        Ability1,
        Ability2,
        Extra1,
        Extra2,
        Extra3
    }

    [SerializeField]
    private CooldownToUpdate thisCooldownType;

    private Slider cooldownSlider;

    private Coroutine cooldownCoroutine;

    private void Awake()
    {
        cooldownSlider = GetComponent<Slider>();
        UIPublicEvents.UpdateHUDCooldownUI += SetNewCooldown;
        UIPublicEvents.ImmediatelyRefreshCooldown += ImmediatelyRefreshCooldown;
    }

    private void OnDestroy()
    {
        UIPublicEvents.UpdateHUDCooldownUI -= SetNewCooldown;
        UIPublicEvents.ImmediatelyRefreshCooldown -= ImmediatelyRefreshCooldown;
    }

    

    private void SetNewCooldown(CooldownToUpdate cooldownType = CooldownToUpdate.None, float maxCooldownValue = 1f)
    {
        if (cooldownType != thisCooldownType || cooldownCoroutine != null)
        {
            return;
        }

        cooldownCoroutine = StartCoroutine(AbilityCooldown(cooldownType, maxCooldownValue));
    }

    private IEnumerator AbilityCooldown(CooldownUIController.CooldownToUpdate cooldownType, float maxCooldown)
    {
        float timer = 0.0f;

        while (timer < maxCooldown)
        {
            timer += Time.deltaTime;
            cooldownSlider.value = timer / maxCooldown;
            yield return null;
        }

        cooldownCoroutine = null;
    }

    private void ImmediatelyRefreshCooldown(CooldownToUpdate cooldownType = CooldownToUpdate.None)
    {
        if (cooldownType != thisCooldownType)
        {
            return;
        }

        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }

        //1 is off cooldown
        cooldownSlider.value = 1;
    }
}
