/*
* Author: Tyler
* Contributors:
* Last Modified: 09/18/2026
* Summary: Manages the upgrade grid and menu. 
* To Do:   inventory, populate an already initialized grid.
*/

using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuController : MenuBase
{
    #region vars

    #region Setup
    private enum ShownSettings
    {
        None,
        References,
        Testing
    }

    [SerializeField]
    private ShownSettings settings;

    #endregion

    #region refs

    [SerializeField]
    [HorizontalLine(4, EColor.Green)]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform gridContainer;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private UpgradeTileBehavior tilePrefab;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private InventoryPinHolder inventoryPinSlot;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private PinItemBehavior pinItemPrefab;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform inventory;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private ScrollRect inventoryScrollRect;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform draggingParent;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform pinsOnOtherGridsParent;

    #endregion

    #region testing

    [SerializeField]
    [HorizontalLine(4, EColor.Red)]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private bool enableTestMode;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private List<UpgradeTileGrid> testingGrids;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private List<PinScriptable> testInventory;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private List<GlyphScriptable> testListOfPossibleGlyphs;
    #endregion

    #region private
    private List<UpgradeTileBehavior> tilesInGrid = new();

    private List<PinItemBehavior> inventoryPins = new();
    private List<InventoryPinHolder> inventorySlots = new();
    private int gridHeight;
    private int gridWidth;

    private UpgradeTileGrid currentlyEnabledGrid;

    [HideInInspector]
    public PinItemBehavior CarriedPin;

    private List<PinItemBehavior> pinsOnNonEnabledGrids = new();

    #endregion

    #endregion

    #region Setup

    /// <summary>
    /// Initializes the grid
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();

    }

    /// <summary>
    /// Populates everything once the menu is opened.
    /// </summary>
    protected override void OpenMenu()
    {
        base.OpenMenu();
        PopulateInventory();
        OpenGrid(testingGrids[0]);
        
    }

    /// <summary>
    /// throws out an events when the menu is closed
    /// </summary>
    protected override void CloseMenu()
    {
        UIPublicEvents.UpgradeMenuClosed?.Invoke();

        base.CloseMenu();
    }

    /// <summary>
    /// subscribes to all public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.PinPickedUp += SetCarriedPin;
        InputPublicEvents.ShootReleased += DropHeldPin;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.PinPickedUp -= SetCarriedPin;
        InputPublicEvents.ShootReleased -= DropHeldPin;
    }

    /// <summary>
    /// placeholder for future. TODO: make the thing
    /// </summary>
    private void PopulateInventory()
    {
        //replace with a system that checks if it sees any you already have soon
        foreach (PinScriptable pin in MidRunDataManager.Instance.pinInventory)
        {
            InventoryPinHolder tempSlot = Instantiate(inventoryPinSlot, inventory);

            PinItemBehavior temp = Instantiate(pinItemPrefab, tempSlot.transform);

            tempSlot.InitSlot(temp);
            temp.InitPin(pin, tempSlot);
            inventoryPins.Add(temp);
            inventorySlots.Add(tempSlot);
        }
    }

    /// <summary>
    /// Turns on and off the pins on the other grid
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void UpdatePinVisibility()
    {
        pinsOnNonEnabledGrids.Clear();

        //replace testingGrids with the list of equipped weapons
        foreach (UpgradeTileGrid GridImLookingAt in testingGrids)
        {
            //grabs all of the tiledatas that have a pin.
            List<UpgradeTileData> tilesWithPins = GridImLookingAt.grid.Where(x => x.pin != null).ToList();
            foreach (UpgradeTileData tileData in tilesWithPins)
            {
                PinItemBehavior pinItem = inventoryPins.Find(x => x.pinData == tileData.pin);

                if (pinItem == null)
                {
                    throw new System.Exception("Tried to find a pin thats not in the players inventory");
                }

                //turns on the pins on the current grid and turns off the pins on the other grid(s)
                if (GridImLookingAt == currentlyEnabledGrid)
                {
                    pinItem.transform.SetParent(pinItem.Parent.transform);
                    pinItem.gameObject.SetActive(true);
                }
                else
                {
                    pinItem.transform.SetParent(pinsOnOtherGridsParent);
                    pinsOnNonEnabledGrids.Add(pinItem);
                    pinItem.gameObject.SetActive(false);
                }

                
            }
        }
    }

    /// <summary>
    /// Initializes the grid for the first time. TODO: replace testGrid with the grid of the actual weapon.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void OpenGrid(UpgradeTileGrid gridToInit = null)
    {
        gridHeight = gridToInit.height;
        gridWidth = gridToInit.width;

        GridLayoutGroup layoutG = gridContainer.GetComponent<GridLayoutGroup>();

        if (layoutG == null)
        {
            throw new System.Exception("Grid Container doesnt have a layout group");
        }

        //sets the number of columns
        layoutG.constraintCount = gridWidth;
        gridToInit.InitGrid();

        for (int i = 0; i < gridToInit.grid.Count; i++)
        {
            if (tilesInGrid.Count > i)
            {
                //use the tile i have
                tilesInGrid[i].gameObject.SetActive(true);
                tilesInGrid[i].SetTileData(gridToInit.grid[i]);

            }
            else
            {
                UpgradeTileBehavior tempTile = Instantiate(tilePrefab, gridContainer);
                tempTile.InitTile();
                tilesInGrid.Add(tempTile);
                tempTile.SetTileData(gridToInit.grid[i]);
            }
        }

        if (gridToInit.grid.Count < tilesInGrid.Count)
        {
            for (int i = gridToInit.grid.Count; i < tilesInGrid.Count; i++)
            {
                tilesInGrid[i].gameObject.SetActive(false);
            }
        }

        currentlyEnabledGrid = gridToInit;
        UIPublicEvents.UpgradeGridInitialized?.Invoke();
        UpdatePinVisibility();
    }

    #endregion

    #region Getters

    /// <summary>
    /// grabs the adjacent tiles from a given set of coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns>0 - north, 1 - northeast, 2 - east, 3 - southeast, 4 - south, 5 - southwest, 6 - west, 7 - northwest </returns>
    public UpgradeTileBehavior[] getAdjacentTiles(Vector2Int coords)
    {
        UpgradeTileBehavior[] temp = new UpgradeTileBehavior[8];

        //check north
        temp[0] = GetTile(new Vector2Int(coords.x, coords.y - 1));

        //check northeast
        temp[1] = GetTile(new Vector2Int(coords.x + 1, coords.y - 1));

        //check east
        temp[2] = GetTile(new Vector2Int(coords.x + 1, coords.y));

        //check southeast
        temp[3] = GetTile(new Vector2Int(coords.x + 1, coords.y + 1));

        //check south
        temp[4] = GetTile(new Vector2Int(coords.x, coords.y + 1));

        //check southwest
        temp[5] = GetTile(new Vector2Int(coords.x - 1, coords.y + 1));

        //check west
        temp[6] = GetTile(new Vector2Int(coords.x - 1, coords.y));

        //check northwest
        temp[7] = GetTile(new Vector2Int(coords.x - 1, coords.y - 1));

        return temp;
    }

    /// <summary>
    /// grabs a tile given the index in the tilesInGrid list
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public UpgradeTileBehavior GetTile(int index)
    {
        if (index < 0 || index >= tilesInGrid.Count)
        {
            return null;
        }
        else
        {
            return tilesInGrid[index];
        }
    }

    /// <summary>
    /// grabs the tile given the coordinates
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    public UpgradeTileBehavior GetTile(Vector2Int coords)
    {
        if (coords.x < 0 || coords.x >= gridWidth ||
            coords.y < 0 || coords.y >= gridHeight)
        {
            return null;
        }

        return GetTile(coords.x + (coords.y * gridWidth));
    }
    #endregion

    #region ButtonFuncs

    /// <summary>
    /// Changes which grid is active. 
    /// </summary>
    public void SwapGrid()
    {
        if (CarriedPin != null)
        {
            CarriedPin.PinPlaced();
            PlaceCarriedPinInTile(CarriedPin.Owner);
        }

        if (enableTestMode)
        {
            int enabledGrid = testingGrids.IndexOf(currentlyEnabledGrid);
            if (enabledGrid == testingGrids.Count - 1)
            {
                OpenGrid(testingGrids[0]);
            }
            else
            {
                OpenGrid(testingGrids[enabledGrid + 1]);
            }
        }
    }

    #endregion

    #region Misc

    /// <summary>
    /// called from trying to pick up an item - not called from an empty tile
    /// </summary>
    /// <param name="item"></param>
    private void SetCarriedPin(PinItemBehavior item)
    {

        //unmodifies the pin if it modifies it at all.
        if (item.Parent != null)
        {
            item.Parent.UnequipPin();
            
        }

        //places the currently held pin in the tile of the pin you want to pick up
        if (CarriedPin != null)
        {
            CarriedPin.PinPlaced();

            //if the new item is in the inventory
            if (item.Parent == item.Owner)
            {
                //set the current item to its owner
                PlaceCarriedPinInTile(CarriedPin.Owner);
                //CarriedPin.Owner.SetNewPinInTile(CarriedPin);
            }
            else
            {
                //set the current item to the tile of the new one
                PlaceCarriedPinInTile(item.Parent);
                //item.Parent.SetNewPinInTile(CarriedPin);
            }
        }
        PickUpPin(item);

        //turn on/off a canvas group over the inventory that trashes the held item
    }

    /// <summary>
    /// force grabs a pin
    /// </summary>
    /// <param name="item"></param>
    public void GrabPlacedPin(PinItemBehavior item)
    {
        if (CarriedPin != null)
        {
            PlaceCarriedPinInTile(CarriedPin.Owner);
        }

        //unequip pins properly
        if (pinsOnNonEnabledGrids.Contains(item))
        {
            //I LOVE LINQ
            //grabs the tile that the item is attached to 

            //grabs all the tiles in all the grids that are disabled
            List<UpgradeTileData> nonEnabledGrids = testingGrids.Where(x => x != currentlyEnabledGrid).SelectMany(x => x.grid).ToList();

            //grabs all the tiles that have pins on them, then grabs the specific tile that has the pin.
            UpgradeTileData tilePinIsEquippedTo = nonEnabledGrids.Where(x => x.pin != null).FirstOrDefault(x => x.pin == item.pinData);

            if (tilePinIsEquippedTo == null)
            {
                throw new System.Exception("PinsNotOnEnabledGrids has some issues with resetting");
            }

            //replace with the proper way to remove a pin given the tile data eventually
            tilePinIsEquippedTo.SetPin(null);
            item.Parent = null;
            pinsOnNonEnabledGrids.Remove(item);
            
        }

        PickUpPin(item);
    }

    /// <summary>
    /// sets the item as the new carried pin
    /// </summary>
    /// <param name="item"></param>
    private void PickUpPin(PinItemBehavior item)
    {
        CarriedPin = item;
        CarriedPin.Parent = null;
        CarriedPin.gameObject.SetActive(true);
        CarriedPin.transform.SetParent(draggingParent);
        CarriedPin.PinPickedUp();
        ToggleInventoryScrollability(false);
    }

    /// <summary>
    /// Sets the tile to have a pin
    /// </summary>
    /// <param name="tile"></param>
    public void PlaceCarriedPinInTile(PinHolderSlot tile)
    {
        if (CarriedPin == null)
        {
            return;
        }

        CarriedPin.Parent = tile;
        CarriedPin.PinPlaced();
        tile.SetNewPinInTile(CarriedPin);

        CarriedPin = null;
        ToggleInventoryScrollability(true);
    }
    
    /// <summary>
    /// returns the parameter pin to the inventory
    /// </summary>
    /// <param name="item"></param>
    public void ReturnPinToInventory(PinItemBehavior item)
    {
        if (item.Parent != null && item.Parent != item.Owner)
        {
            item.Parent.UnequipPin();
        }

        item.Parent = null;
        item.Owner.SetNewPinInTile(item);
        ToggleInventoryScrollability(true);
    }

    /// <summary>
    /// drops the pin that the player is holding
    /// </summary>
    private void DropHeldPin()
    {
        //quit out if youre not holding anything
        if (CarriedPin == null)
        {
            return;
        }

        //check to see if the pin is over a tile
        PointerEventData tempEventData = new(EventSystem.current);
        tempEventData.position = CarriedPin.transform.position;

        List<RaycastResult> raycastResults = new();

        EventSystem.current.RaycastAll(tempEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject.GetComponent<UpgradeTileBehavior>() != null)
            {
                PlaceCarriedPinInTile(raycastResults[0].gameObject.GetComponent<UpgradeTileBehavior>());
                return;
            }
            else if (raycastResults[0].gameObject.GetComponent<PinItemBehavior>() != null)
            {
                PinItemBehavior pinInSlot = raycastResults[0].gameObject.GetComponent<PinItemBehavior>();
                PinHolderSlot tile = pinInSlot.Parent;
                //send that pin to inventory
                ReturnPinToInventory(pinInSlot);

                if (tile != null)
                {
                    if (tile is UpgradeTileBehavior)
                    {
                        PlaceCarriedPinInTile(tile);
                        return;
                    }
                    else
                    {
                        PlaceCarriedPinInTile(CarriedPin.Owner);
                        return;
                    }
                }
                //place carried pin here
                
            }
            else if (raycastResults[0].gameObject.GetComponent<InventoryPinHolder>() != null)
            {
                PlaceCarriedPinInTile(CarriedPin.Owner);
                return;
            }
        }

        if (CarriedPin.Parent == null)
        {
            PlaceCarriedPinInTile(CarriedPin.Owner);
        }
        else
        {
            PlaceCarriedPinInTile(CarriedPin.Parent);
        }
    }

    /// <summary>
    /// enables and disables the inventory scrollability
    /// </summary>
    /// <param name="canScroll"></param>
    private void ToggleInventoryScrollability(bool canScroll = true)
    {
        inventoryScrollRect.StopMovement();
        inventoryScrollRect.enabled = canScroll;
    }
    #endregion
}
