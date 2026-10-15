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
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuController : MenuControllerSupportBase
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
    private UpgradeTileBehavior pinsTile;

    private List<PinItemBehavior> pinsOnNonEnabledGrids = new();

    #endregion

    #region Controller
    private enum FocusType
    {
        None,
        Inventory,
        Grid,
        Tarot
    }

    private FocusType currentFocus;

    private ControllerSupportedClickable prevSelectedInventorySlot;

    private ControllerSupportedClickable prevSelectedGridSlot;

    private ControllerSupportedClickable prevSelectedTarotCard;

    private RectTransform inventoryContentRect;

    private RectTransform inventoryScrollRectTransform;
    #endregion

    #endregion

    #region Setup

    /// <summary>
    /// Returns the carried item as a pin
    /// </summary>
    /// <returns></returns>
    private PinItemBehavior GetCarriedPin()
    {
        return carriedItem as PinItemBehavior;
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
    protected override void MenuClosedSucessfully()
    {
        ReturnPinToItsTile();
        UIPublicEvents.UpgradeMenuClosed?.Invoke();
        base.MenuClosedSucessfully();
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

        InputPublicEvents.SwapFocusToInventory += MoveFocusToInventory;
        InputPublicEvents.SwapFocusToGrid += MoveFocusToGrid;
        InputPublicEvents.SwapFocusToTarot += MoveFocusToTarot;
        UIPublicEvents.SelectSpecificTile += SelectSpecificTile;
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

        InputPublicEvents.SwapFocusToInventory -= MoveFocusToInventory;
        InputPublicEvents.SwapFocusToGrid -= MoveFocusToGrid;
        UIPublicEvents.SelectSpecificTile -= SelectSpecificTile;
        InputPublicEvents.SwapFocusToTarot -= MoveFocusToTarot;
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

        if (carriedItem != null)
        {
            GetCarriedPin().StopPinMoving();
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
        if (carriedItem != null)
        {
            PinItemBehavior pin = GetCarriedPin();
            pin.StopPinMoving();

            //if the new item is in the inventory
            if (item.Parent == item.Owner)
            {
                //set the current item to its owner
                ReturnCarriedPinToInventory();
            }
            else
            {
                //set the current item to the tile of the new one
                PlacePinInTile(pin, item.Parent);
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
        if (GetCarriedPin() != null)
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
        if (GetCarriedPin() == null)
        {
            return;
        }

        PinHolderSlot target = null;

        if (InputManager.Instance.ControllerIsEnabled)
        {
            if (ControllerUIService.Instance.currentlySelected is PinHolderSlot slot)
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
        PinItemBehavior pin = GetCarriedPin();

        if (target == null)
        {
           
            if (pin.Parent == null)
            {
                ReturnCarriedPinToInventory();
            }
            else
            {
                PlacePinInTile(pin, pin.Parent);
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
            PlacePinInTile(pin, tile);
        }
        else if (target is InventoryPinHolder inventorySlot)
        {
            ReturnCarriedPinToInventory();
            if (inventorySlot.pin.Parent == inventorySlot)
            {
                PickUpPin(inventorySlot.pin);
            }
            UIPublicEvents.SelectSpecificTile?.Invoke(inventorySlot);
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
        carriedItem = item;
        PinItemBehavior pin = GetCarriedPin();

        if (pin.Parent is UpgradeTileBehavior tile)
        {
            pinsTile = tile;
            tile.UnequipPin();
        }

        pin.Parent = null;



        pin.gameObject.SetActive(true);
        pin.transform.SetParent(draggingParent);
        pin.StartPinMoving();

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
        if (carriedItem == pin)
        {
            carriedItem = null;
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
        PinItemBehavior pin = GetCarriedPin();
        PlacePinInTile(pin, pin.Owner);
    }

    /// <summary>
    /// places a pin back on the tile it was from
    /// used (mostly) for controller
    /// </summary>
    private void ReturnPinToItsTile()
    {
        if (carriedItem != null)
        {
            if (pinsTile == null)
            {
                ReturnCarriedPinToInventory();
            }
            else
            {
                PinItemBehavior pin = GetCarriedPin();
                PlacePinInTile(pin, pinsTile);
            }
        }
        
    }

    #endregion

    #region ControllerSupport


    #region movement


    /// <summary>
    /// moves the selected item to the right new item
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public override void MoveSelectedItem(ControllerSupportedClickable target, int direction)
    {
        if (target == null)
        {
            //if we are in the inventory and move right
            if (currentFocus == FocusType.Inventory && direction == 2)
            {
                MoveFocusToGrid();
            }
            else if (currentFocus == FocusType.Grid)
            {
                if (direction == 6)
                {
                    MoveFocusToInventory();
                }
                if (direction == 4)
                {
                    MoveFocusToTarot();
                }
            }
            else if (currentFocus == FocusType.Tarot)
            {
                if (direction == 0 || direction == 1)
                {
                    MoveFocusToGrid();
                }
                if (direction == 7)
                {
                    MoveFocusToInventory();
                }
            }
        }
        //different behavior based on what we are
        else if (target is InventoryPinHolder invSlot)
        {
            ControllerUIService.Instance.DeselectCurrentObject();
            SelectInventoryItem(invSlot);

            if (inventoryContentRect == null)
            {
                inventoryContentRect = inventory.GetComponent<RectTransform>();
            }

            if (inventoryScrollRectTransform == null)
            {
                inventoryScrollRectTransform = inventoryScrollRect.GetComponent<RectTransform>();
            }

            inventoryContentRect.anchoredPosition = UtilityFunctions.GetNewScrollRectAnchoredPos(
                inventoryScrollRectTransform,
                ControllerUIService.Instance.currentlySelected.GetComponent<RectTransform>(),
                inventoryContentRect
                );
        }

        else if (target is UpgradeTileBehavior origTile)
        {
            if (origTile.isActive)
            {
                ControllerUIService.Instance.DeselectCurrentObject();
                SelectGridItem(origTile);
            }

            UpgradeTileBehavior NextGridTileInDirection = GetNextGridTile(origTile, direction);

            if (NextGridTileInDirection == null)
            {
                if (direction == 6)
                {
                    MoveFocusToInventory();
                }
                if (direction == 4)
                {
                    MoveFocusToTarot();
                }
            }
            else
            {
                ControllerUIService.Instance.DeselectCurrentObject();
                SelectGridItem(NextGridTileInDirection);
            }
        }

        else if (target is UpgradeMenuTarotCardUI tarot)
        {
            ControllerUIService.Instance.DeselectCurrentObject();
            SelectTarotCard(tarot);
        }
    }



    #endregion

    #region Selection

    /// <summary>
    /// selects the default inventory pin as the default
    /// </summary>
    public override void SelectDefault()
    {
        MoveFocusToInventory();
    }

    /// <summary>
    /// changes the selected object to be a specific tile
    /// </summary>
    /// <param name="slot"></param>
    private void SelectSpecificTile(PinHolderSlot slot)
    {
        if (slot is UpgradeTileBehavior tile)
        {
            currentFocus = FocusType.Grid;
            SelectGridItem(tile);
        }
        if (slot is InventoryPinHolder inv)
        {
            currentFocus = FocusType.Inventory;
            SelectInventoryItem(inv);
        }

        ControllerUIService.Instance.MoveCarriedItem();
    }
    #endregion

    #region Grid

    /// <summary>
    /// Moves the player's cursor to the grid
    /// </summary>
    private void MoveFocusToGrid()
    {
        currentFocus = FocusType.Grid;
        SelectLastSelectedGridTile();
    }

    /// <summary>
    /// Selects the last selected grid tile.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SelectLastSelectedGridTile()
    {
        //if youve selected an inventory slot, selects that one
        if (prevSelectedGridSlot != null)
        {
            SelectGridItem(prevSelectedGridSlot);
            return;
        }

        UpgradeTileBehavior defaultGridPin = tilesInGrid.FirstOrDefault(x => x.isActive == true);

        if (defaultGridPin == null)
        {
            throw new System.Exception("Grid has no active tiles");
        }

        SelectGridItem(defaultGridPin);
    }

    /// <summary>
    /// selects grid tile
    /// </summary>
    /// <param name="gridItem"></param>
    public void SelectGridItem(ControllerSupportedClickable gridItem)
    {
        prevSelectedGridSlot = gridItem;
        ControllerUIService.Instance.SelectSpecficTile(gridItem);
    }

    /// <summary>
    /// moves along an axis until it either returns an active grid tile or a null element (wall)
    /// </summary>
    /// <param name="startingTileInclusive"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private UpgradeTileBehavior GetNextGridTile(UpgradeTileBehavior startingTileInclusive, int direction)
    {
        if (startingTileInclusive == null || startingTileInclusive.isActive)
        {
            return startingTileInclusive;
        }

        return GetNextGridTile(startingTileInclusive.adjacentTiles[direction], direction);
    }

    #endregion

    #region Inventory

    /// <summary>
    /// moves the player's cursor to the inventory
    /// </summary>
    public void MoveFocusToInventory()
    {
        currentFocus = FocusType.Inventory;

        SelectLastSelectedInventoryPin();
    }

    /// <summary>
    /// Selects the last selected inventory pin.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SelectLastSelectedInventoryPin()
    {
        //if youve selected an inventory slot, selects that one
        if (prevSelectedInventorySlot != null)
        {
            SelectInventoryItem(prevSelectedInventorySlot);
            return;
        }

        InventoryPinHolder defaultInventoryPin = inventorySlots[0];

        if (defaultInventoryPin == null)
        {
            throw new System.Exception("Inventory is null");
        }

        SelectInventoryItem(defaultInventoryPin);
    }

    /// <summary>
    /// selects an inventory item
    /// </summary>
    /// <param name="inventoryItem"></param>
    public void SelectInventoryItem(ControllerSupportedClickable inventoryItem)
    {
        prevSelectedInventorySlot = inventoryItem;
        ControllerUIService.Instance.SelectSpecficTile(inventoryItem);
    }

    #endregion

    #region Tarot

    /// <summary>
    /// moves the cursor to the tarot card section
    /// </summary>
    private void MoveFocusToTarot()
    {
        currentFocus = FocusType.Tarot;
        SelectLastSelectedTarotTile();
    }

    /// <summary>
    /// Selects the last selected tarot card.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SelectLastSelectedTarotTile()
    {
        //if youve selected an inventory slot, selects that one
        if (prevSelectedTarotCard != null)
        {
            SelectTarotCard(prevSelectedTarotCard);
            return;
        }

        UpgradeMenuTarotCardUI defaultTarotCard = tarotSlots[0];

        if (defaultTarotCard == null)
        {
            throw new System.Exception("Grid has no active tiles");
        }

        SelectTarotCard(defaultTarotCard);
    }

    /// <summary>
    /// selects a tarot card
    /// </summary>
    /// <param name="tarotCard"></param>
    public void SelectTarotCard(ControllerSupportedClickable tarotCard)
    {
        prevSelectedTarotCard = tarotCard;
        ControllerUIService.Instance.SelectSpecficTile(tarotCard);
    }

    #endregion

    #endregion
}
