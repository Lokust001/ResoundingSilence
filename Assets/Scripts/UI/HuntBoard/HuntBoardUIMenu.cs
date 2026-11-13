/*
* Author: Tyler
* Contributors:
* Last Modified: 11/10/2026
* Summary: Controls the hunt board UI
* To Do:   N/A
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HuntBoardUIMenu : MenuBase
{
    
    private SideObjective currentObj;
    private RewardGiver currentRewardGiver;

    [SerializeField]
    private TMP_Text ObjectiveText;

    [SerializeField]
    private GameObject acceptButtons;

    [SerializeField]
    private GameObject claimRewardButton;

    [SerializeField]
    private TMP_Text rewardText;

    [SerializeField]
    private GameObject acceptQuestButton;


    /// <summary>
    /// enables public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdateHuntBoardData += UpdateHuntBoardData;
        InputPublicEvents.KeyboardMouseEnabled += KBMEnabled;
        InputPublicEvents.ControllerEnabled += ControllerSupportEnabled;
    }

    /// <summary>
    /// disables public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.UpdateHuntBoardData -= UpdateHuntBoardData;
        InputPublicEvents.InteractPressed -= CloseMenu;
        InputPublicEvents.ControllerEnabled -= ControllerSupportEnabled; 
        InputPublicEvents.KeyboardMouseEnabled -= KBMEnabled;
    }

    /// <summary>
    /// updates the menu with the proper side objective data
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateHuntBoardData(SideObjective obj, RewardGiver rewarder)
    {
        acceptButtons.SetActive(false);
        claimRewardButton.SetActive(false);
        rewardText.gameObject.SetActive(false);
        currentObj = obj;
        currentRewardGiver = rewarder;
        ObjectiveText.text = obj.GetSideObjectiveTooltipText();

        if (SideObjectiveManager.Instance.currentHuntObjectives.Contains(obj)) 
        {
            ObjectiveText.text += $"\nProgress: {obj.huntEnemiesProgress} / {obj.huntedEnemiesGoal}";
            InputPublicEvents.InteractPressed += CloseMenu;
            return;
        }

        if (!SideObjectiveManager.Instance.completedObjectives.Contains(obj))
        {
            acceptButtons.SetActive(true);
            defaultSelectedGameobject = acceptQuestButton;
            if (InputManager.Instance.ControllerIsEnabled)
            {
                ControllerSupportEnabled();
            }
            else
            {
                InputPublicEvents.InteractPressed += CloseMenu;
            }

            return;
        }
        
        //at this point, we know the quest is complete
        ObjectiveText.text += $"\nComplete!";
        claimRewardButton.SetActive(true);

        defaultSelectedGameobject = claimRewardButton;
        if (InputManager.Instance.ControllerIsEnabled)
        {
            ControllerSupportEnabled();
        }
        else
        {
            InputPublicEvents.InteractPressed += CloseMenu;
        }

        rewardText.gameObject.SetActive(true);
        rewardText.text = $"Rewards: ";

        if (rewarder == null)
        {
            throw new System.Exception("no rewarder");
        }

        List<Reward> rewards = rewarder.RollRewards();

        foreach (Reward reward in rewards)
        {
            rewardText.text += $"{reward.rewardName} ";
        }

        rewardText.text.Trim();
    }

    /// <summary>
    /// the player accepts the side objective
    /// </summary>
    public void AcceptSideObjective()
    {
        SideObjectiveManager.Instance.AddNewSideObjective(currentObj);
        CloseMenu();
    }

    /// <summary>
    /// the player rejects the side objective
    /// </summary>
    public void CloseMenu()
    {
        currentObj = null;
        currentRewardGiver = null;
        defaultSelectedGameobject = null;
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    /// <summary>
    /// Claims the reward from the hunt board
    /// </summary>
    public void ClaimRewards()
    {
        currentRewardGiver.StartGivingRewards();
        UIPublicEvents.RewardsForObjectiveClaimed?.Invoke(currentObj);
        CloseMenu();
    }

    /// <summary>
    /// when controller is selected, select the right obj
    /// </summary>
    private void ControllerSupportEnabled()
    {
        InputPublicEvents.InteractPressed -= CloseMenu;
        EventSystem.current.SetSelectedGameObject(defaultSelectedGameobject);
    }

    /// <summary>
    /// Allows the player to close the menu with controller
    /// </summary>
    private void KBMEnabled()
    {
        InputPublicEvents.InteractPressed += CloseMenu;
    }
}
