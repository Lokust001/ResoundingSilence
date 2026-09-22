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
    private PinItemBehavior pinItemPrefab;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform inventory;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.References)]
    private Transform draggingParent;

    #endregion

    #region testing

    [SerializeField]
    [HorizontalLine(4, EColor.Red)]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private bool enableTestMode;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private UpgradeTileGrid testGrid;

    [SerializeField]
    [ShowIf(nameof(settings), ShownSettings.Testing)]
    private UpgradeTileGrid testGrid2;

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
    List<UpgradeTileBehavior> enabledTilesInGrid = new();
    private int gridHeight;
    private int gridWidth;

    private UpgradeTileGrid currentlyEnabledGrid;

    [HideInInspector]
    public PinItemBehavior CarriedPin;

    private List<PinItemBehavior> newlyEquippedPins;

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
        OpenGrid(testGrid);
        PopulateInventory();
    }

    /// <summary>
    /// subscribes to all public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.PinPickedUp += SetCarriedPin;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        UIPublicEvents.PinPickedUp -= SetCarriedPin;
    }

    /// <summary>
    /// placeholder for future. TODO: make the thing
    /// </summary>
    private void PopulateInventory()
    {
        foreach (PinScriptable pin in MidRunDataManager.Instance.pinInventory)
        {
            PinItemBehavior temp = Instantiate(pinItemPrefab, inventory);
            temp.InitPin(pin, null);
        }
    }

    /// <summary>
    /// placeholder for future. TODO: make the thing
    /// </summary>
    private void PopulateGrid()
    {
        //turn on/off tiles as needed.
        //populate grid tiles with glyphs
        //populate grid with already equipped pins
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
        if (enableTestMode)
        {
            if (currentlyEnabledGrid == testGrid)
            {
                OpenGrid(testGrid2);
            }
            else
            {
                OpenGrid(testGrid);
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

            //if item was on a tile
            if (item.Parent != null)
            {
                
                item.Parent.SetNewPinInTile(CarriedPin);
            }
            else
            {
                //places it in the inventory if swapping with an item in the inventory
                CarriedPin.Parent = null;
                CarriedPin.transform.SetParent(inventory);
            }
        }

        CarriedPin = item;
        CarriedPin.PinPickedUp();
        CarriedPin.transform.SetParent(draggingParent);
        //turn on/off a canvas group over the inventory that trashes the held item
    }

    /// <summary>
    /// Sets the tile to have a pin
    /// </summary>
    /// <param name="tile"></param>
    public void PlacePinInTile(UpgradeTileBehavior tile)
    {
        if (CarriedPin == null)
        {
            return;
        }

        CarriedPin.PinPlaced();
        tile.SetNewPinInTile(CarriedPin);
        CarriedPin.Parent = tile;
        CarriedPin.transform.SetParent(tile.transform);

        CarriedPin = null;
    }

    #endregion
}
