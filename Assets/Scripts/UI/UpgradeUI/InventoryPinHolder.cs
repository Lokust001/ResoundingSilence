/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: this is the behavior for the inventory slots
* To Do:   N/A
*/

using System.Collections.Generic;

public class InventoryPinHolder : ItemHolderSlot
{
    public PinScriptable pinData { get; private set; }
    protected UpgradeMenuController controller;

    protected virtual void Awake()
    {
        controller = GetComponentInParent<UpgradeMenuController>();
    }

    /// <summary>
    /// initializes the slot
    /// </summary>
    /// <param name="pin"></param>
    public override void InitSlot(CarriedItemBehavior item)
    {
        base.InitSlot(item);

        if (item is PinItemBehavior pin)
        {
            pinData = pin.pinData;
        }
        
    }

    /// <summary>
    /// force grabs the pin that this owns
    /// </summary>
    public override void ClickedOn()
    {
        if (heldItem.Parent != this)
        {
            controller.GrabPlacedItem(heldItem);
            return;
        }

        base.ClickedOn();
    }

    /// <summary>
    /// Gets all of the nighbors in the pin holder, then returns them
    /// TODO: Also swap over to the grid if this is the rightmost item
    /// </summary>
    /// <returns></returns>
    public override List<ControllerSupportedClickable> getNeighbors()
    {
        List<ControllerSupportedClickable> returnlist = new();

        List<InventoryPinHolder> temp = UtilityFunctions.GetAdjacentTiles<InventoryPinHolder>(
            controller.inventorySlots.IndexOf(this),
            controller.inventorySlots,
            controller.inventorySlots.Count,
            2);

        for (int i = 0; i < temp.Count; i++)
        {
            returnlist.Add(temp[i]);
        }

        return returnlist;


    }
}
