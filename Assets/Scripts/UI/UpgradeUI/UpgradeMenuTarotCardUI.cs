using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuTarotCardUI : ControllerSupportedClickable
{
    private UpgradeMenuController menu;

    private List<ControllerSupportedClickable> adjacentTiles = new();

    [SerializeField]
    private Image highlight;

    public void InitCardUI()
    {
        menu = FindAnyObjectByType<UpgradeMenuController>();
        
    }

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

    public override void HoveredOver()
    {
        base.HoveredOver();
        highlight.enabled = true;
    }

    public override void UnHoveredOver()
    {
        base.UnHoveredOver();
        highlight.enabled = false;
    }
}
