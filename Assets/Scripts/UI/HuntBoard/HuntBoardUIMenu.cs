/*
* Author: Tyler
* Contributors:
* Last Modified: 11/10/2026
* Summary: Controls the hunt board UI
* To Do:   N/A
*/

using TMPro;
using UnityEngine;

public class HuntBoardUIMenu : MenuBase
{
    
    private SideObjective currentObj;

    [SerializeField]
    private TMP_Text ObjectiveText;

    /// <summary>
    /// enables public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdateHuntBoardData += UpdateHuntBoardData;
    }

    /// <summary>
    /// disables public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.UpdateHuntBoardData -= UpdateHuntBoardData;
    }

    /// <summary>
    /// updates the menu with the proper side objective data
    /// </summary>
    /// <param name="obj"></param>
    private void UpdateHuntBoardData(SideObjective obj)
    {
        currentObj = obj;
        ObjectiveText.text = obj.GetSideObjectiveTooltipText();
    }

    /// <summary>
    /// the player accepts the side objective
    /// </summary>
    public void AcceptSideObjective()
    {
        SideObjectiveManager.Instance.AddNewSideObjective(currentObj);
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    /// <summary>
    /// the player rejects the side objective
    /// </summary>
    public void RejectQuest()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }
}
