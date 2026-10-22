/*
* Author: Tyler
* Contributors:
* Last Modified: 10/14/2026
* Summary: This will be the script that holds a tarot card
* To Do:   N/A
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TarotCardHolder : ItemHolderSlot
{
    [SerializeField]
    private Image lockedSlotImage;
    public TarotScriptable ownedCard;
    private bool slotIsUnlocked = false;

    private List<ControllerSupportedClickable> neighbors;

    private TarotCardController controller;

    /// <summary>
    /// sets up the right public events
    /// </summary>
    private void Awake()
    {
        UIPublicEvents.TarotMenuOpened += TarotMenuOpened;
        controller = FindAnyObjectByType<TarotCardController>();
    }

    /// <summary>
    /// dismantles the right public events
    /// </summary>
    private void OnDestroy()
    {
        UIPublicEvents.TarotMenuOpened -= TarotMenuOpened;
    }

    /// <summary>
    /// initializes this slot as an inventory slot
    /// </summary>
    /// <param name="cardToOwn"></param>
    public void SetInventorySlot(TarotScriptable cardToOwn)
    {
        ownedCard = cardToOwn;
        slotIsUnlocked = false;

        if (ownedCard.isUnlocked)
        {
            UnlockSlot();
        }
        else
        {
            if (heldItem != null)
            {
                heldItem.CanBePickedUp = false;
            }
            lockedSlotImage.gameObject.SetActive(true);
            lockedSlotImage.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// unlocks this inventory slot
    /// </summary>
    public void UnlockSlot()
    {
        if (heldItem != null)
        {
            heldItem.CanBePickedUp = true;
        }
        
        if (lockedSlotImage != null)
        {
            lockedSlotImage.gameObject.SetActive(false);
        }

        
    }

    /// <summary>
    /// if the player unlocked this slot's tarot card in between runs, this will unlock it
    /// </summary>
    private void TarotMenuOpened()
    {

        if ((ownedCard == null || ownedCard.isUnlocked) && !slotIsUnlocked)
        {
            UnlockSlot();
        }
    }

    /// <summary>
    /// returns a list of this slot's neighbors
    /// </summary>
    /// <returns></returns>
    public override List<ControllerSupportedClickable> getNeighbors()
    {
        if (neighbors == null || neighbors.Count <= 0)
        {
            neighbors = new();

            List<TarotCardHolder> tempList = new();

            if (ownedCard != null)
            {
                tempList = UtilityFunctions.GetAdjacentTiles<TarotCardHolder>(
                controller.tarotInventorySlots.IndexOf(this),
                controller.tarotInventorySlots,
                controller.inventoryHeight,
                controller.inventoryWidth);
            }
            else
            {
                tempList = UtilityFunctions.GetAdjacentTiles<TarotCardHolder>(
                controller.equippedSlots.IndexOf(this),
                controller.equippedSlots,
                controller.equippedHeight,
                controller.equippedWidth);
            }

            

            for(int i = 0; i < tempList.Count; i++) 
            {
                neighbors.Add(tempList[i]);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// unequips the card if its not an inventory card
    /// </summary>
    public override void UnequipItem()
    {
        base.UnequipItem();
        if (ownedCard == null)
        {
            heldItem = null;
        }
        
    }
}
