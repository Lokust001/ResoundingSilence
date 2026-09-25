/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: this is the behavior for the slots that hold pins
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.EventSystems;

public class PinHolderSlot : Clickable
{
    protected UpgradeMenuController controller;

    /// <summary>
    /// sets any refs
    /// </summary>
    private void Awake()
    {
        controller = GetComponentInParent<UpgradeMenuController>();
    }

    /// <summary>
    /// Triggers when this slot is clicked on
    /// </summary>
    public override void ClickedOn()
    {
        base.ClickedOn();
        controller.PlaceCarriedPinInTile(this);
    }

    /// <summary>
    /// runs when a pin gets placed on this tile. TODO: add modifier to pin
    /// </summary>
    /// <param name="pin"></param>
    public virtual void SetNewPinInTile(PinItemBehavior pin)
    {
        pin.Parent = this;
        pin.transform.SetParent(transform);
    }

    /// <summary>
    /// runs when a pin leaves the slot. TODO: remove modifier from pin.
    /// </summary>
    public virtual void UnequipPin()
    {

    }
}
