using UnityEngine;

public class InventoryPinHolder : PinHolderSlot
{
    private PinItemBehavior pin;

    public void InitSlot(PinItemBehavior pin)
    {
        this.pin = pin;
    }

    public override void ClickedOn()
    {
        controller.GrabPlacedPin(pin);
    }
}
