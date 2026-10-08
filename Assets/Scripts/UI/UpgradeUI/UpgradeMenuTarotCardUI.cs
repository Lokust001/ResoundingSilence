/*
* Author: Tyler
* Contributors:
* Last Modified: 10/8/2026
* Summary: Controls an individual tarot card in the upgrade menu.
* To Do:   N/A
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuTarotCardUI : ControllerSupportedClickable
{
    private UpgradeMenuController menu;

    private List<ControllerSupportedClickable> adjacentTiles = new();

    [SerializeField]
    private Image highlight;

    /// <summary>
    /// initializes the card.
    /// </summary>
    public void InitCardUI()
    {
        if (menu == null)
        {
            menu = FindAnyObjectByType<UpgradeMenuController>();
        }
    }

    /// <summary>
    /// returns the neighbors of this tarot card as controllersupportedclickables
    /// </summary>
    /// <returns></returns>
    public override List<ControllerSupportedClickable> getNeighbors()
    {
        if (adjacentTiles.Count <= 0)
        {
            List<UpgradeMenuTarotCardUI> adjacentTarotTiles = UtilityFunctions.GetAdjacentTiles(
                menu.tarotSlots.IndexOf(this),
                menu.tarotSlots,
                1,
                menu.tarotSlots.Count);

            for (int i = 0; i < adjacentTarotTiles.Count; i++)
            {
                adjacentTiles.Add(adjacentTarotTiles[i]);
            }
        }
        return adjacentTiles;
    }

    /// <summary>
    /// highlights when hovered over
    /// TODO: add in tooltip once we get tarot cards working
    /// </summary>
    public override void HoveredOver()
    {
        base.HoveredOver();
        highlight.enabled = true;
    }

    /// <summary>
    /// unhighlights when hovered over
    /// TODO: remove tooltip
    /// </summary>
    public override void UnHoveredOver()
    {
        base.UnHoveredOver();
        highlight.enabled = false;
    }
}
