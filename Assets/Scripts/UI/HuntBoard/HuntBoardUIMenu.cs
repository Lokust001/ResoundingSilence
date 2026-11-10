using TMPro;
using UnityEngine;

public class HuntBoardUIMenu : MenuBase
{
    
    private SideObjective currentObj;

    [SerializeField]
    private TMP_Text ObjectiveText;

    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdateHuntBoardData += UpdateHuntBoardData;
    }

    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.UpdateHuntBoardData -= UpdateHuntBoardData;
    }

    private void UpdateHuntBoardData(SideObjective obj)
    {
        currentObj = obj;
        ObjectiveText.text = obj.GetSideObjectiveTooltipText();
    }

    public void AcceptSideObjective()
    {
        SideObjectiveManager.Instance.AddNewSideObjective(currentObj);
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    public void RejectQuest()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }
}
