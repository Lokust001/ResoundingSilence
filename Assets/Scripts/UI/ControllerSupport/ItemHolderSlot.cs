using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemHolderSlot : ControllerSupportedClickable
{
    [SerializeField]
    private Image highlight;

    [HideInInspector]
    public CarriedItemBehavior heldItem;

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
