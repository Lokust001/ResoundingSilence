/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 11/12/2026
* Summary: Controls the player's hud
* To Do:   N/A
*/

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        Testing,
        Minimap,
        HuntBoard
    }

    [SerializeField]
    private Settings settings;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private Image healthBarSlider;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private AnimationCurve healthBarAnimation;

    [SerializeField, ShowIf(nameof(settings), Settings.HealthBar)]
    private float AnimationTime = 1.0f;

    [SerializeField, ShowIf(nameof(settings), Settings.EXPBar)]
    private Image expBarSlider;

    [SerializeField, ShowIf(nameof(settings), Settings.Minimap)]
    private MiniMapCamera mapCamPrefab;

    private MiniMapCamera mapCam;
    #region TESTING

    [SerializeField, ShowIf(nameof(settings), Settings.Cooldown)]
    private float Ability1MaxCooldown;

    [SerializeField, ShowIf(nameof(settings), Settings.Cooldown)]
    private float Ability2MaxCooldown;

    private Coroutine healthAnimCoroutine;

    private bool mapIsCurrentlyMini;

    [SerializeField, ShowIf(nameof(settings), Settings.HuntBoard)]
    private TMP_Text prefabHuntText;

    [SerializeField, ShowIf(nameof(settings), Settings.HuntBoard)]
    private Color huntCompleteColor;

    [SerializeField, ShowIf(nameof(settings), Settings.HuntBoard)]
    private GameObject huntBoardContainer;
    private List<(SideObjective, TMP_Text)> huntTexts = new();

    /// <summary>
    /// debug command to force the ui to take one damage
    /// </summary>
    [Button("Testing Take one damage")]
    private void TakeOneDamage()
    {
        MidRunDataManager.Instance.ChangePlayerHealth(-10);
    }

    /// <summary>
    /// gains one exp for testing
    /// </summary>
    [Button("Testing gain one exp")]
    private void GainOneExp()
    {
        MidRunDataManager.Instance.GainXP(1);
    }

    /// <summary>
    /// turns on the public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdatePlayerHealthSlider += UpdatePlayerHealthBarValue;
        UIPublicEvents.UpdatePlayerEXPSlider += UpdatePlayerEXPBarValue;
        UIPublicEvents.UpdateHUDHuntObjs += UpdateHuntObjective;
        GenericPublicEvents.SetCurrentWeapon += NewWeaponEnabled;
        UIPublicEvents.RewardsForObjectiveClaimed += RemoveHuntObjective;
    }

    /// <summary>
    /// turns off the public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.UpdatePlayerHealthSlider -= UpdatePlayerHealthBarValue;
        UIPublicEvents.UpdatePlayerEXPSlider -= UpdatePlayerEXPBarValue; 
        UIPublicEvents.UpdateHUDHuntObjs -= UpdateHuntObjective;
        UIPublicEvents.RewardsForObjectiveClaimed -= RemoveHuntObjective;
        GenericPublicEvents.SetCurrentWeapon -= NewWeaponEnabled;
    }

    /// <summary>
    /// makes the map camera
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        mapCam = Instantiate(mapCamPrefab, MainCameraManager.Instance.transform);
        mapCam.InitMinimapCamera();
    }

    #region Testing

    /// <summary>
    /// Sets ability images
    /// </summary>
    /// <param name="weapon"></param>
    private void NewWeaponEnabled(BaseWeaponBehaviour weapon)
    {
        UIPublicEvents.ChangeCooldownImages?.Invoke(
            weapon.ThisWeaponData.editableAbilities[weapon.GetComponent<BaseWeaponAbilityBehaviour>().abilityOneIndex].AbilityImage,
            weapon.ThisWeaponData.editableAbilities[weapon.GetComponent<BaseWeaponAbilityBehaviour>().abilityOneIndex].BackgroundImage,
            CooldownUIController.CooldownToUpdate.Ability1);
        UIPublicEvents.ChangeCooldownImages?.Invoke(
            weapon.ThisWeaponData.editableAbilities[weapon.GetComponent<BaseWeaponAbilityBehaviour>().abilityTwoIndex].AbilityImage,
            weapon.ThisWeaponData.editableAbilities[weapon.GetComponent<BaseWeaponAbilityBehaviour>().abilityTwoIndex].BackgroundImage,
            CooldownUIController.CooldownToUpdate.Ability2);

        ImmediatelyRefreshAllCooldowns();
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
            StopAllCoroutines();
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

    /// <summary>
    /// creates a new text obj to track hunt board
    /// </summary>
    /// <param name="obj"></param>
    private void CreateNewHuntObjective(SideObjective obj)
    {
        if (obj == null)
        {
            return;
        }

        TMP_Text NewHuntText = Instantiate(prefabHuntText, huntBoardContainer.transform);
        huntTexts.Add(new (obj, NewHuntText));

        NewHuntText.text = $"{obj.GetSideObjectiveTooltipText()}: 0/{obj.huntedEnemiesGoal}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(huntBoardContainer.transform as RectTransform);
    }

    /// <summary>
    /// updates a text tracker for hunt board
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.Exception"></exception>
    private void UpdateHuntObjective(SideObjective obj)
    {
        if (obj == null)
        {
            return;
        }

        TMP_Text textToUpdate = huntTexts.Count > 0 ? huntTexts.FirstOrDefault(x => x.Item1 == obj).Item2 : null;

        if (textToUpdate == null)
        {
            CreateNewHuntObjective(obj);
            return;
        }

        string huntProgress = $"{obj.huntEnemiesProgress}/{obj.huntedEnemiesGoal}";

        if (obj.huntEnemiesProgress >= obj.huntedEnemiesGoal)
        {
            huntProgress = $"<color=#{ColorUtility.ToHtmlStringRGB(huntCompleteColor)}>Complete!</color>";
        }

        try
        {
            textToUpdate.text = $"{obj.GetSideObjectiveTooltipText()}:\n{huntProgress}";
        }
        catch
        {
            throw new System.Exception($"Could not find a text for hunt objective {obj.name}, and couldnt make a new one");
        }
    }

    /// <summary>
    /// removes the hunt text associated with the given objective
    /// </summary>
    /// <param name="obj"></param>
    private void RemoveHuntObjective(SideObjective obj)
    {
        (SideObjective, TMP_Text) temp = huntTexts.FirstOrDefault(x => x.Item1 == obj);

        if (temp.Item1 == null)
        {
            return;
        }

        huntTexts.Remove(temp);
        Destroy(temp.Item2);
    }
    #endregion
    #endregion
}
