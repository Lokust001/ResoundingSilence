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

public class TarotCardController : MenuControllerSupportBase
{
    [SerializeField]
    private List<TarotCardHolder> tarotInventorySlots = new();

    [SerializeField]
    private List<TarotCardHolder> equippedSlots = new();

    [SerializeField]
    private TarotItemBehavior tarotItemPrefab;

    public override void InitMenu()
    {
        base.InitMenu();
        PopulateInventory();
    }

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

    protected override void MenuOpenedSucessfully()
    {
        base.MenuOpenedSucessfully();
        UIPublicEvents.TarotMenuOpened?.Invoke();
    }

    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        UIPublicEvents.TarotMenuClosed?.Invoke();
    }

    public void StartGamePressed()
    {
        GenericPublicEvents.StartGamePressed?.Invoke();
        UIPublicEvents.UpdateUIManagerStack?.Invoke(UiMenuType.PlayerHUD);
    }
}
