/*
* Author: Tyler
* Contributors:
* Last Modified: 10/17/2026
* Summary: This is will be the script that controls the tarot card items that can be picked up
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class TarotItemBehavior : CarriedItemBehavior
{
    public TarotScriptable data;
    public void SetTarotData(TarotScriptable scriptable)
    {
        data = scriptable;
        GetComponent<Image>().color = data.tempColor;
    }

    /// <summary>
    /// requests a tooltip when this becomes hovered over
    /// </summary>
    public override void HoveredOver()
    {
        base.HoveredOver();
        TooltipUIService.Instance.RequestTooltip(data);
    }

    public override void ClickedOn()
    {
        if (data.isUnlocked)
        {
            base.ClickedOn();
        }
    }
}
