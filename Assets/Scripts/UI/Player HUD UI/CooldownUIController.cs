/*
* Author: Tyler
* Contributors:
* Last Modified: 09/29/2026
* Summary: controls the cooldown of the slider this is attached to.
* To Do:   N/A
*/

using System.Collections;
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

    /// <summary>
    /// initializes the slider
    /// </summary>
    private void Awake()
    {
        cooldownSlider = GetComponent<Slider>();
        UIPublicEvents.UpdateHUDCooldownUI += SetNewCooldown;
        UIPublicEvents.ImmediatelyRefreshCooldown += ImmediatelyRefreshCooldown;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        UIPublicEvents.UpdateHUDCooldownUI -= SetNewCooldown;
        UIPublicEvents.ImmediatelyRefreshCooldown -= ImmediatelyRefreshCooldown;
    }

    /// <summary>
    /// Turns on a new cooldown
    /// </summary>
    /// <param name="cooldownType">the type of cooldown to turn on - if it's not this object's type, it doesnt turn it on</param>
    /// <param name="maxCooldownValue">how much to cool down</param>
    private void SetNewCooldown(CooldownToUpdate cooldownType = CooldownToUpdate.None, float maxCooldownValue = 1f)
    {
        if (cooldownType != thisCooldownType || cooldownCoroutine != null)
        {
            return;
        }

        cooldownCoroutine = StartCoroutine(AbilityCooldown(maxCooldownValue));
    }

    /// <summary>
    /// the actual cooldown tracker - constantly updates the slider
    /// </summary>
    /// <param name="maxCooldown">how much the cooldown should be</param>
    /// <returns></returns>
    private IEnumerator AbilityCooldown(float maxCooldown)
    {
        float timer = 0.0f;

        //the cooldown itself
        while (timer < maxCooldown)
        {
            timer += Time.deltaTime;

            //updates the slider - has to be between 0 and 1 so we divide to get the percentage
            cooldownSlider.value = timer / maxCooldown;
            yield return null;
        }

        cooldownCoroutine = null;
    }

    /// <summary>
    /// If the type is this objects type, it instantly refreshes the cooldown
    /// </summary>
    /// <param name="cooldownType"></param>
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

        //1 is off cooldown - fully filled
        cooldownSlider.value = 1;
    }
}
