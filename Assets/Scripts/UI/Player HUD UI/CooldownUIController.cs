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

    [SerializeField]
    private Image cooldownImage;

    [SerializeField]
    private Image backgroundImage;

    private Coroutine cooldownCoroutine;

    /// <summary>
    /// initializes the slider
    /// </summary>
    private void Awake()
    {
        UIPublicEvents.ChangeCooldownImages += ChangeCooldownImage;

    }

    /// <summary>
    /// turns on cooldown UI events
    /// </summary>
    private void OnEnable()
    {
        UIPublicEvents.UpdateHUDCooldownUI += SetNewCooldown;
        UIPublicEvents.ImmediatelyRefreshCooldown += ImmediatelyRefreshCooldown;
    }

    /// <summary>
    /// Turns off cooldown UI events
    /// </summary>
    private void OnDisable()
    {
        UIPublicEvents.ImmediatelyRefreshCooldown -= ImmediatelyRefreshCooldown;
        UIPublicEvents.ChangeCooldownImages -= ChangeCooldownImage;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        UIPublicEvents.UpdateHUDCooldownUI -= SetNewCooldown;

    }

    /// <summary>
    /// changes the image on the cooldown
    /// </summary>
    /// <param name="backgroundImage"></param>
    /// <param name="cooldownSliderImage"></param>
    /// <param name="cooldownType"></param>
    public void ChangeCooldownImage(Sprite backgroundImage, Sprite cooldownSliderImage, CooldownToUpdate cooldownType)
    {
        if (thisCooldownType != cooldownType)
        {
            return;
        }

        if (backgroundImage == null || cooldownSliderImage == null)
        {
            return;
        }

        this.backgroundImage.sprite = backgroundImage;
        cooldownImage.sprite = cooldownSliderImage;
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
        float timer = maxCooldown;

        //the cooldown itself
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            //updates the slider - has to be between 0 and 1 so we divide to get the percentage
            cooldownImage.fillAmount = timer / maxCooldown;
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
        cooldownImage.fillAmount = 1;
    }
}
