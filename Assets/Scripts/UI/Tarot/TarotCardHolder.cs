/*
* Author: Tyler
* Contributors:
* Last Modified: 10/14/2026
* Summary: This will be the script that holds a tarot card
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class TarotCardHolder : ItemHolderSlot
{
    [SerializeField]
    private Image lockedSlotImage;
    public TarotScriptable ownedCard;
    private bool slotIsUnlocked = false;

    private void Awake()
    {
        UIPublicEvents.TarotMenuOpened += TarotMenuOpened;
    }

    private void OnDestroy()
    {
        UIPublicEvents.TarotMenuOpened -= TarotMenuOpened;
    }

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

    private void TarotMenuOpened()
    {

        if ((ownedCard == null || ownedCard.isUnlocked) && !slotIsUnlocked)
        {
            UnlockSlot();
        }
    }

    
}
