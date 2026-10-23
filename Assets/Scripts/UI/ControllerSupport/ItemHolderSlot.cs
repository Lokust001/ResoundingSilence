/*
* Author: Tyler
* Contributors:
* Last Modified: 10/17/2026
* Summary: This is the base of all the slots that can hold an item in the custom controller support menus
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class ItemHolderSlot : ControllerSupportedClickable
{
    [SerializeField]
    private Image highlight;

    //[HideInInspector]
    public CarriedItemBehavior heldItem;

    /// <summary>
    /// initializes the slot
    /// </summary>
    /// <param name="item"></param>
    public virtual void InitSlot(CarriedItemBehavior item)
    {
        heldItem = item;
    }

    /// <summary>
    /// runs when a pin gets placed on this tile. TODO: add modifier to pin
    /// </summary>
    /// <param name="item"></param>
    public virtual void SetNewItemInTile(CarriedItemBehavior item)
    {
        item.Parent = this;
        item.transform.SetParent(transform);
        item.transform.position = transform.position;
        heldItem = item;
    }

    /// <summary>
    /// Triggers when the mouse is hovering over this or the controller is selecting this
    /// </summary>
    public override void HoveredOver()
    {
        base.HoveredOver();
        //if this is hovered over and has a pin in it, tell that pin to hover
        if (heldItem != null && heldItem.Parent == this)
        {
            heldItem.HoveredOver();
        }

        highlight.enabled = true;
    }

    /// <summary>
    /// triggers when the mouse leaves this or the controller unselects this
    /// </summary>
    public override void UnHoveredOver()
    {
        base.UnHoveredOver();
        //unhovers the pin too
        if (heldItem != null)
        {
            heldItem.UnHoveredOver();
        }

        highlight.enabled = false;
    }

    /// <summary>
    /// triggers when the object is clicked on, either by controller or by mouse
    /// </summary>
    public override void ClickedOn()
    {
        base.ClickedOn();
        if (heldItem != null && heldItem.Parent == this)
        {
            heldItem.ClickedOn();
        }
    }

    /// <summary>
    /// runs when a pin leaves the slot. TODO: remove modifier from pin.
    /// </summary>
    public virtual void UnequipItem()
    {

    }
}
