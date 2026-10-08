/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: this is the behavior for the slots that hold pins
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class PinHolderSlot : ControllerSupportedClickable
{
    [SerializeField]
    protected Image Highlight;
    protected UpgradeMenuController controller;

    public PinItemBehavior pin;

    /// <summary>
    /// sets any refs
    /// </summary>
    private void Awake()
    {
        controller = GetComponentInParent<UpgradeMenuController>();
    }

    /// <summary>
    /// runs when a pin gets placed on this tile. TODO: add modifier to pin
    /// </summary>
    /// <param name="pin"></param>
    public virtual void SetNewPinInTile(PinItemBehavior pin)
    {
        pin.Parent = this;
        pin.transform.SetParent(transform);
        pin.transform.position = transform.position;
        this.pin = pin;
    }

    /// <summary>
    /// runs when a pin leaves the slot. TODO: remove modifier from pin.
    /// </summary>
    public virtual void UnequipPin()
    {

    }

    /// <summary>
    /// Highlights when this is hovered over.
    /// </summary>
    public override void HoveredOver()
    {
        //if this is hovered over and has a pin in it, tell that pin to hover
        if (pin != null && pin.Parent == this)
        {
            pin.HoveredOver();
        }

        Highlight.enabled = true;

        
    }

    /// <summary>
    /// unhighlights when focus is lost from this
    /// </summary>
    public override void UnHoveredOver()
    {
        //unhovers the pin too
        if (pin != null)
        {
            pin.UnHoveredOver();
        }

        Highlight.enabled = false;
    }
}
