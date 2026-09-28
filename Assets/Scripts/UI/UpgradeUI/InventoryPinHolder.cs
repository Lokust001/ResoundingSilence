/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: this is the behavior for the inventory slots
* To Do:   N/A
*/

using UnityEngine;

public class InventoryPinHolder : PinHolderSlot
{
    private PinItemBehavior pin;
    public PinScriptable pinData { get; private set; }

    /// <summary>
    /// initializes the slot
    /// </summary>
    /// <param name="pin"></param>
    public void InitSlot(PinItemBehavior pin)
    {
        this.pin = pin;
        pinData = pin.pinData;
    }

    /// <summary>
    /// force grabs the pin that this owns
    /// </summary>
    public override void ClickedOn()
    {
        controller.GrabPlacedPin(pin);
    }
}
