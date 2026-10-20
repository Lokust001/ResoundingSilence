/*
* Author: Tyler
* Contributors:
* Last Modified: 10/17/2026
* Summary: This is the script that oversees the tarot card menus
* To Do:   N/A
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class TarotCardController : MenuControllerSupportBase
{
    private enum focus
    {
        None,
        Inventory,
        EquippedSlots
    }

    private focus currentFocus;

    public int inventoryHeight;
    public int inventoryWidth;

    public List<TarotCardHolder> tarotInventorySlots = new();

    public List<TarotCardHolder> equippedSlots = new();

    [SerializeField]
    private TMP_Text tooltipText;

    [SerializeField]
    private TarotItemBehavior tarotItemPrefab;

    /// <summary>
    /// initializes the menu
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        PopulateInventory();
    }

    /// <summary>
    /// Populates the inventory with the tarot cards
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void PopulateInventory()
    {
        foreach (TarotScriptable tarot in StaticDataManager.Instance.tarotCards)
        {
            //if the inventory already has a copy of the tarot card, skip to the next loop
            if (tarotInventorySlots.Find(x => x.ownedCard == tarot))
            {
                continue;
            }

            TarotCardHolder nextAvailableHolder = tarotInventorySlots.FirstOrDefault(x => x.ownedCard == null);

            if (nextAvailableHolder == null)
            {
                throw new System.Exception("Tried to place a tarot card but had no empty slots");
            }

            TarotItemBehavior tarotItem = Instantiate(tarotItemPrefab, nextAvailableHolder.transform);
            nextAvailableHolder.InitSlot(tarotItem);
            tarotItem.InitItem(nextAvailableHolder);
            tarotItem.SetTarotData(tarot);
            nextAvailableHolder.SetInventorySlot(tarot);
        }
    }

    /// <summary>
    /// lets other scripts know that the menu is opening
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        base.MenuOpenedSucessfully();
        UIPublicEvents.TarotMenuOpened?.Invoke();
        UIPublicEvents.SetNewTooltipTextObject?.Invoke(tooltipText);
    }

    /// <summary>
    /// lets other scripts know that the menus been closed
    /// </summary>
    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        UIPublicEvents.TarotMenuClosed?.Invoke();
    }

    /// <summary>
    /// starts the game when the player is done with the tarot menu
    /// </summary>
    public void FinishedInTarotMenu()
    {
        GenericPublicEvents.StartGamePressed?.Invoke();
        UIPublicEvents.UpdateUIManagerStack?.Invoke(UiMenuType.PlayerHUD);
    }

    /// <summary>
    /// selects the default tile
    /// </summary>
    public override void SelectDefault()
    {
        ControllerUIService.Instance.SelectSpecficTile(tarotInventorySlots[0]);
    }

    /// <summary>
    /// turns on the public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();

        InputPublicEvents.SwapFocusToGrid += SwapFocusInputWrapper;
    }

    /// <summary>
    /// turns off the public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();

        InputPublicEvents.SwapFocusToGrid -= SwapFocusInputWrapper;
    }

    /// <summary>
    /// wrapper to swap the controller's focus
    /// </summary>
    private void SwapFocusInputWrapper()
    {
        SwapCurrentFocus();
    }

    /// <summary>
    /// swaps the controller's current focus
    /// </summary>
    /// <param name="focusToSwapTo"></param>
    private void SwapCurrentFocus(focus focusToSwapTo = focus.None)
    {
        switch (focusToSwapTo)
        {
            case focus.None:
                if (currentFocus == focus.Inventory)
                {
                    currentFocus = focus.EquippedSlots;
                }
                else
                {
                    currentFocus = focus.Inventory;
                }
                break;
            case focus.Inventory:
                currentFocus = focus.Inventory;

                break;
            case focus.EquippedSlots:
                currentFocus = focus.EquippedSlots;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// moves the controller's focused item. 
    /// TODO: fill in edge cases
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public override void MoveSelectedItem(ControllerSupportedClickable target, int direction)
    {
        if (target == null)
        {
            return;
        }

        ControllerUIService.Instance.SelectSpecficTile(target);
    }
}
