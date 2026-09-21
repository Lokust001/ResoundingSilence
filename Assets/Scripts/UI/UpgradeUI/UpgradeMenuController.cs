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
    private GameObject inventory;

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
    private List<UpgradeTileBehavior> tilesInGrid = new List<UpgradeTileBehavior>();
    List<UpgradeTileBehavior> enabledTilesInGrid = new();
    private int gridHeight;
    private int gridWidth;

    private UpgradeTileGrid currentlyEnabledGrid;

    #endregion

    #endregion

    #region Setup

    /// <summary>
    /// Initializes the grid
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        InitGrid();
    }

    /// <summary>
    /// Turns on the testing grid if test mode is on
    /// </summary>
    private void Start()
    {
        if (enableTestMode)
        {
            InitGrid(testGrid);
        }
    }

    /// <summary>
    /// placeholder for future. TODO: make the thing
    /// </summary>
    private void PopulateInventory()
    {
        //populate inventory with owned, unequipped pins
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
    private void InitGrid(UpgradeTileGrid gridToInit = null)
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
                tilesInGrid[i].SetTileData(gridToInit.grid[i]);
            }
            else
            {
                UpgradeTileBehavior tempTile = Instantiate(tilePrefab, gridContainer);
                tilesInGrid.Add(tempTile);
                tempTile.SetTileData(gridToInit.grid[i]);
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
        Debug.Log("Called");
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
                InitGrid(testGrid2);
            }
            else
            {
                InitGrid(testGrid);
            }
        }
    }

    #endregion
}
