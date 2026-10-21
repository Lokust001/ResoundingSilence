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
    public enum focus
    {
        None,
        Inventory,
        EquippedSlots
    }

    private focus currentFocus;

    public int inventoryHeight;
    public int inventoryWidth;

    public int equippedHeight;
    public int equippedWidth;

    public List<TarotCardHolder> tarotInventorySlots = new();

    public List<TarotCardHolder> equippedSlots = new();

    [SerializeField]
    private TMP_Text tooltipText;

    [SerializeField]
    private TarotItemBehavior tarotItemPrefab;

    private TarotCardHolder prevSelectedInventory;
    private TarotCardHolder prevSelectedEquipped;

    [SerializeField]
    private GeneralButtons readyButton;

    [SerializeField]
    private GeneralButtons backButton;

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
        UIPublicEvents.SetNewTooltipTextObject?.Invoke(tooltipText);
        base.MenuOpenedSucessfully();
        UIPublicEvents.TarotMenuOpened?.Invoke();
        
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
        SwapCurrentFocus(focus.Inventory);
        prevSelectedInventory = ControllerUIService.Instance.currentlySelected as TarotCardHolder;
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
            if (currentFocus == focus.EquippedSlots && (direction == 5 || direction == 6 || direction == 7))
            {
                SelectDefaultItemInFocus(focus.Inventory);
                return;
            }
            if (currentFocus == focus.EquippedSlots && (0 < direction && direction < 5))
            {
                ControllerUIService.Instance.SelectSpecficTile(readyButton);
                return;
            }

            if (currentFocus == focus.Inventory && (direction == 1 || direction == 2 || direction == 3))
            {
                SelectDefaultItemInFocus(focus.EquippedSlots);
                return;
            }

            if (currentFocus == focus.Inventory && direction == 0)
            {
                ControllerUIService.Instance.SelectSpecficTile(backButton);
                return;
            }

            return;
        }

        if (target is not TarotCardHolder)
        {
            ControllerUIService.Instance.SelectSpecficTile(target);
            return;
        }

        TarotCardHolder slot = target as TarotCardHolder;
        ControllerUIService.Instance.SelectSpecficTile(target);
        if (slot.ownedCard != null)
        {
            prevSelectedInventory = slot;
        }
        else
        {
            prevSelectedEquipped = slot;
        }

    }

    public void SelectDefaultItemInFocus(focus type = focus.None)
    {
        if (type == focus.None)
        {
            type = currentFocus;
        }

        switch (type)
        {
            case focus.None:
                throw new System.Exception("currentfocus is none");
            case focus.Inventory:

                if (prevSelectedInventory == null)
                {
                    prevSelectedInventory = tarotInventorySlots[0];
                }
                ControllerUIService.Instance.SelectSpecficTile(prevSelectedInventory);
                SwapCurrentFocus(focus.Inventory);

                break;
            case focus.EquippedSlots:

                if (prevSelectedEquipped == null)
                {
                    prevSelectedEquipped = equippedSlots[0];
                }
                ControllerUIService.Instance.SelectSpecficTile(prevSelectedEquipped);
                SwapCurrentFocus(focus.EquippedSlots);
                break;
            default:
                throw new System.Exception("currentfocus is out of bounds of the enum");
        }
    }

    protected override void PickUpItem(CarriedItemBehavior item)
    {
        if (item is TarotItemBehavior tarot)
        {
            if (!tarot.data.isUnlocked)
            {
                return;
            }
        }
        base.PickUpItem(item);
    }
}
