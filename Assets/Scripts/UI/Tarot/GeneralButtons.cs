using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralButtons : ControllerSupportedClickable
{
    private TarotCardController controller;

    [SerializeField]
    private bool IsContinueButton;
    [SerializeField]
    private Image highlight;

    private void Awake()
    {
        controller = FindAnyObjectByType<TarotCardController>();
    }

    public override List<ControllerSupportedClickable> getNeighbors()
    {
        List<ControllerSupportedClickable> returnList = new() { null, null, null, null, null, null, null, null };

        if (IsContinueButton)
        {
            returnList[6] = controller.equippedSlots[controller.equippedSlots.Count - 1];
            returnList[7] = controller.equippedSlots[controller.equippedSlots.Count - 1];
            returnList[0] = controller.equippedSlots[controller.equippedSlots.Count - 1];
        }
        else
        {
            returnList[4] = controller.tarotInventorySlots[0];
        }
        return returnList;
    }

    public override void ClickedOn()
    {
        if (IsContinueButton)
        {
            controller.FinishedInTarotMenu();
        }
        else
        {
            GenericPublicEvents.ReturnToMainMenu?.Invoke();
        }
    }

    /// <summary>
    /// requests a tooltip and highlights when hovered over.
    /// </summary>
    public override void HoveredOver()
    {
        base.HoveredOver();
        highlight.enabled = true;
    }

    /// <summary>
    /// unhighlights when focus leaves this.
    /// </summary>
    public override void UnHoveredOver()
    {
        base.UnHoveredOver();
        highlight.enabled = false;
    }
}
