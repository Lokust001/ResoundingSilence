/*
* Author: Tyler
* Contributors:
* Last Modified: 10/14/2026
* Summary: This is the base of all the ui menus that need specialized controller support
* To Do:   N/A
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuControllerSupportBase : MenuBase

{
    [HideInInspector]
    public CarriedItemBehavior carriedItem;

    private ItemHolderSlot itemsPrevTile;

    [SerializeField]
    private Transform carriedItemParent;

    /// <summary>
    /// virtual. moves a selected item in a direction
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public virtual void MoveSelectedItem(ControllerSupportedClickable target, int direction)
    {

    }

    /// <summary>
    /// Selects the default item in the menu
    /// </summary>
    public virtual void SelectDefault()
    {

    }

    /// <summary>
    /// Triggers when the menu is properly closed
    /// </summary>
    protected override void MenuClosedSucessfully()
    {
        ReturnItemToItsTile();
        base.MenuClosedSucessfully();
    }

    /// <summary>
    /// sets up the public events for this object
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.ItemPickedUp += SetCarriedItem;
        InputPublicEvents.PinReleased += DropHeldItem;
        InputPublicEvents.DropPin += ReturnItemToItsTile;
        InputPublicEvents.ControllerEnabled += ReturnCarriedItemToInventory;
        InputPublicEvents.KeyboardMouseEnabled += ReturnCarriedItemToInventory;
    }

    /// <summary>
    /// disables all of the public events for this script
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.ItemPickedUp -= SetCarriedItem;
        InputPublicEvents.PinReleased -= DropHeldItem;
        InputPublicEvents.DropPin -= ReturnItemToItsTile;
        InputPublicEvents.ControllerEnabled -= ReturnCarriedItemToInventory;
        InputPublicEvents.KeyboardMouseEnabled -= ReturnCarriedItemToInventory;
    }

    #region inputs

    /// <summary>
    /// called from trying to pick up an item - not called from an empty tile
    /// </summary>
    /// <param name="item"></param>
    private void SetCarriedItem(CarriedItemBehavior item)
    {
        //places the currently held pin in the tile of the pin you want to pick up
        if (carriedItem != null)
        {
            item.StopMoving();

            //if the new item is in the inventory
            if (item.Parent == item.Owner)
            {
                //set the current item to its owner
                ReturnCarriedItemToInventory();
            }
            else
            {
                //set the current item to the tile of the new one
                PlaceItemInTile(item, item.Parent);
            }
        }

        PickUpItem(item);
    }

    /// <summary>
    /// grabs a pin from the empty inventory button
    /// </summary>
    /// <param name="item"></param>
    public virtual void GrabPlacedItem(CarriedItemBehavior item)
    {
        if (carriedItem != null)
        {
            ReturnCarriedItemToInventory();
        }

        PickUpItem(item);
    }

    #endregion

    #region Cancelling

    /// <summary>
    /// drops the pin that the player is holding
    /// </summary>
    private void DropHeldItem()
    {
        //quit out if youre not holding anything
        if (carriedItem == null)
        {
            return;
        }

        ItemHolderSlot target = null;

        //if on controller, grab the slot we are selecting
        if (InputManager.Instance.ControllerIsEnabled)
        {
            if (ControllerUIService.Instance.currentlySelected is ItemHolderSlot slot)
            {
                target = slot;
            }
        }
        else
        {
            //check to see if the pin is over a tile
            PointerEventData tempEventData = new(EventSystem.current);
            tempEventData.position = carriedItem.transform.position;

            List<RaycastResult> raycastResults = new();

            EventSystem.current.RaycastAll(tempEventData, raycastResults);

            //if we hit anything with the raycast
            if (raycastResults.Count > 0)
            {
                //if we hit an item, grab its parent - if not, grab the slot
                if (raycastResults[0].gameObject.GetComponent<CarriedItemBehavior>() != null)
                {
                    target = raycastResults[0].gameObject.GetComponent<CarriedItemBehavior>().Parent;
                }
                else
                {
                    target = raycastResults[0].gameObject.GetComponent<ItemHolderSlot>();
                }

            }
        }

        //if we hit nothing, return it to the tile
        if (target == null)
        {
            ReturnItemToItsTile();
            return;
        }

        //if we try to place it in the inventory
        if (carriedItem.Owner == target)
        {
            ReturnCarriedItemToInventory();
            return;
        }

        //if target isnt holding anything, place the pin in the tile
        if (target.heldItem == null)
        {
            PlaceItemInTile(carriedItem, target);
            return;
        }

        CarriedItemBehavior targetItem = target.heldItem;
        CarriedItemBehavior carried = carriedItem;

        //if the target is the owner of the item its holding
        if (targetItem.Owner == target)
        {
            ReturnCarriedItemToInventory();
            UIPublicEvents.SelectSpecificTile?.Invoke(target);
            PickUpItem(targetItem);
        }
        else
        {
            //if the carried item came from its owner, return targetitem to its owner
            if (itemsPrevTile == carried.Owner)
            {
                PlaceItemInTile(targetItem, targetItem.Owner);
            }
            else
            {
                PlaceItemInTile(targetItem, itemsPrevTile);
            }

            PlaceItemInTile(carried, target);
            /*
                        PickUpItem(targetItem);
                        //if its not, swap the items
                        PlaceItemInTile(carried, target);*/
        }
    }

    #endregion

    #region Misc

    /// <summary>
    /// sets the item as the new carried pin
    /// </summary>
    /// <param name="item"></param>
    protected virtual void PickUpItem(CarriedItemBehavior item)
    {
        carriedItem = item;

        itemsPrevTile = item.Parent;
        item.Parent.UnequipItem();

        item.Parent = null;
        item.gameObject.SetActive(true);
        item.transform.SetParent(carriedItemParent);
        item.StartPinMoving();
    }
    #endregion

    #region Placing pins in tile

    /// <summary>
    /// places a specific pin in. Tile = null means it will return to the inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="tile">null = return to inventory</param>
    public virtual void PlaceItemInTile(CarriedItemBehavior item, ItemHolderSlot tile = null)
    {
        if (item == null)
        {
            return;
        }

        if (tile == null)
        {
            tile = item.Owner;
        }

        //if we place the carried pin, we arent carrying it anymore
        if (carriedItem == item)
        {
            carriedItem = null;
            itemsPrevTile = null;
        }

        //if its on a tile, unequip it from that tile
        if (item.Parent != null && item.Parent != item.Owner)
        {
            item.Parent.UnequipItem();
        }

        item.Parent = tile;
        item.StopMoving();
        tile.SetNewItemInTile(item);

        UIPublicEvents.SelectSpecificTile?.Invoke(tile);
    }

    /// <summary>
    /// Shorthand function for returning the currently carried pin back to its inventory slot
    /// </summary>
    public void ReturnCarriedItemToInventory()
    {
        PlaceItemInTile(carriedItem);
    }

    /// <summary>
    /// places a pin back on the tile it was from
    /// used (mostly) for controller
    /// </summary>
    private void ReturnItemToItsTile()
    {
        if (carriedItem != null)
        {
            if (itemsPrevTile == null || itemsPrevTile.heldItem != null)
            {
                ReturnCarriedItemToInventory();
            }
            else
            {
                PlaceItemInTile(carriedItem, itemsPrevTile);
            }
        }
    }

    #endregion
}
