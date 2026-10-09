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
using TMPro;
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


    [ShowIf(nameof(settings), ShownSettings.References)]
    public Transform inventory;


    [ShowIf(nameof(settings), ShownSettings.References)]
    public ScrollRect inventoryScrollRect;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform draggingParent;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform pinsOnOtherGridsParent;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Button swapGridsButton;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private TMP_Text tooltipTextObject;

    [HideInInspector]
    public UpgradeTileGrid currentlyEnabledGrid;

    [ShowIf(nameof(settings), ShownSettings.References)]
    public List<UpgradeMenuTarotCardUI> tarotSlots = new();

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
    private bool DisableSwappingGrid;

    #endregion

    #region private
    public List<UpgradeTileBehavior> tilesInGrid { get; private set; } = new();

    private List<PinItemBehavior> inventoryPins = new();
    public List<InventoryPinHolder> inventorySlots = new();

    private int gridHeight;
    private int gridWidth;



    [HideInInspector]
    public PinItemBehavior CarriedPin;
    private UpgradeTileBehavior pinsTile;

    private List<PinItemBehavior> pinsOnNonEnabledGrids = new();

    private UpgradeControllerSupportManager supportManager;

    #endregion

    #endregion

    #region Setup

    /// <summary>
    /// Initializes the grid
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        supportManager = GetComponent<UpgradeControllerSupportManager>();
        supportManager.InitSupportManager();
    }

    /// <summary>
    /// Populates everything once the menu is opened.
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        foreach (UpgradeMenuTarotCardUI tarotUI in tarotSlots)
        {
            tarotUI.InitCardUI();
        }

        PopulateInventory();

        if (MidRunDataManager.Instance.equippedWeapons.All(x => x == null))
        {
            OpenGrid(testingGrids[0]);
        }
        else
        {
            OpenGrid(MidRunDataManager.Instance.equippedWeapons.First(x => x != null).upgradeGrid);
        }

        swapGridsButton.interactable = !DisableSwappingGrid;
        UIPublicEvents.UpgradeMenuOpened?.Invoke();
        UIPublicEvents.SetNewTooltipTextObject?.Invoke(tooltipTextObject);

        base.MenuOpenedSucessfully();
    }

    /// <summary>
    /// throws a public event when the upgrade menu closes
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
        InputPublicEvents.PinReleased += DropHeldPin;
        InputPublicEvents.DropPin += ReturnPinToItsTile;
        InputPublicEvents.ControllerEnabled += ReturnPinToItsTile;
        InputPublicEvents.KeyboardMouseEnabled += ReturnPinToItsTile;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    protected override void TearDownPublicEvents()
    {
        base.TearDownPublicEvents();
        UIPublicEvents.PinPickedUp -= SetCarriedPin;
        InputPublicEvents.PinReleased -= DropHeldPin;
        InputPublicEvents.DropPin -= ReturnPinToItsTile;
        InputPublicEvents.ControllerEnabled -= ReturnPinToItsTile;
        InputPublicEvents.KeyboardMouseEnabled -= ReturnPinToItsTile;
    }

    /// <summary>
    /// placeholder for future. TODO: make the thing
    /// </summary>
    private void PopulateInventory()
    {
        foreach (PinScriptable pin in MidRunDataManager.Instance.pinInventory)
        {
            if (inventoryPins.Find(x => x.pinData == pin) == null)
            {
                InventoryPinHolder tempSlot = Instantiate(inventoryPinSlot, inventory);

                PinItemBehavior temp = Instantiate(pinItemPrefab, tempSlot.transform);

                tempSlot.InitSlot(temp);
                temp.InitPin(pin, tempSlot);
                inventoryPins.Add(temp);
                inventorySlots.Add(tempSlot);
                temp.transform.position = tempSlot.transform.position;
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
                tilesInGrid[i].SetTileData(null);
                tilesInGrid[i].gameObject.SetActive(false);
            }
        }

        currentlyEnabledGrid = gridToInit;
        UIPublicEvents.UpgradeGridInitialized?.Invoke();
        UpdatePinVisibility();
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
            CarriedPin.StopPinMoving();
            ReturnCarriedPinToInventory();
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

    #region Input Handling

    #region Picking up

    /// <summary>
    /// called from trying to pick up an item - not called from an empty tile
    /// </summary>
    /// <param name="item"></param>
    private void SetCarriedPin(PinItemBehavior item)
    {
        //places the currently held pin in the tile of the pin you want to pick up
        if (CarriedPin != null)
        {
            CarriedPin.StopPinMoving();

            //if the new item is in the inventory
            if (item.Parent == item.Owner)
            {
                //set the current item to its owner
                ReturnCarriedPinToInventory();
            }
            else
            {
                //set the current item to the tile of the new one
                PlacePinInTile(CarriedPin, item.Parent);
            }
        }

        PickUpPin(item);
    }

    /// <summary>
    /// grabs a pin from the empty inventory button
    /// </summary>
    /// <param name="item"></param>
    public void GrabPlacedPin(PinItemBehavior item)
    {
        if (CarriedPin != null)
        {
            ReturnCarriedPinToInventory();
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

    #endregion

    #region Cancelling

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

        PinHolderSlot target = null;

        if (InputManager.Instance.ControllerIsEnabled)
        {
            if (supportManager.currentSelectedItem is PinHolderSlot slot)
            {
                target = slot;
            }
        }
        else
        {
            //check to see if the pin is over a tile
            PointerEventData tempEventData = new(EventSystem.current);
            tempEventData.position = CarriedPin.transform.position;

            List<RaycastResult> raycastResults = new();

            EventSystem.current.RaycastAll(tempEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                if (raycastResults[0].gameObject.GetComponent<PinItemBehavior>() != null)
                {
                    target = raycastResults[0].gameObject.GetComponent<PinItemBehavior>().Parent;
                }
                else
                {
                    target = raycastResults[0].gameObject.GetComponent<PinHolderSlot>();
                }
                
            }
        }

        if (target == null)
        {
            if (CarriedPin.Parent == null)
            {
                ReturnCarriedPinToInventory();
            }
            else
            {
                PlacePinInTile(CarriedPin, CarriedPin.Parent);
            }
            return;
        }

        //if we hit a tile, place whatever we are carrying in that tile
        if (target is UpgradeTileBehavior tile)
        {
            if (tile.pin != null)
            {
                PlacePinInTile(target.pin, target.pin.Owner);
            }
            PlacePinInTile(CarriedPin, tile);
        }
        else if (target is InventoryPinHolder inventorySlot)
        {
            ReturnCarriedPinToInventory();
        }

    }

    #endregion

    #endregion

    #region Misc

    /// <summary>
    /// sets the item as the new carried pin
    /// </summary>
    /// <param name="item"></param>
    private void PickUpPin(PinItemBehavior item)
    {
        CarriedPin = item;

        if (CarriedPin.Parent is UpgradeTileBehavior tile)
        {
            pinsTile = tile;
            tile.UnequipPin();
        }

        CarriedPin.Parent = null;



        CarriedPin.gameObject.SetActive(true);
        CarriedPin.transform.SetParent(draggingParent);
        CarriedPin.StartPinMoving();

        ToggleInventoryScrollability(false);
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
                    pinItem.transform.position = pinItem.Parent.transform.position;
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
    /// enables and disables the inventory scrollability
    /// </summary>
    /// <param name="canScroll"></param>
    private void ToggleInventoryScrollability(bool canScroll = true)
    {
        inventoryScrollRect.StopMovement();
        inventoryScrollRect.enabled = canScroll;
    }

    #endregion

    #region Placing pins in tile

    /// <summary>
    /// places a specific pin in. Tile = null means it will return to the inventory
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="tile">null = return to inventory</param>
    public void PlacePinInTile(PinItemBehavior pin, PinHolderSlot tile = null)
    {
        if (pin == null)
        {
            return;
        }

        if (tile == null)
        {
            tile = pin.Owner;
        }

        //if we place the carried pin, we arent carrying it anymore
        if (CarriedPin == pin)
        {
            CarriedPin = null;
            pinsTile = null;
        }

        //if its on a tile, unequip it from that tile
        if (pin.Parent != null && pin.Parent != pin.Owner)
        {
            pin.Parent.UnequipPin();
        }

        pin.Parent = tile;
        pin.StopPinMoving();
        tile.SetNewPinInTile(pin);
        ToggleInventoryScrollability(true);
        UIPublicEvents.SelectSpecificTile?.Invoke(tile);
    }

    /// <summary>
    /// Shorthand function for returning the currently carried pin back to its inventory slot
    /// </summary>
    public void ReturnCarriedPinToInventory()
    {
        PlacePinInTile(CarriedPin, CarriedPin.Owner);
    }

    /// <summary>
    /// places a pin back on the tile it was from
    /// used (mostly) for controller
    /// </summary>
    private void ReturnPinToItsTile()
    {
        if (CarriedPin != null)
        {
            if (pinsTile == null)
            {
                ReturnCarriedPinToInventory();
            }
            else
            {
                PlacePinInTile(CarriedPin, pinsTile);
            }
        }
        
    }

    #endregion
}
