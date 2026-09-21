/*
* Author: Tyler
* Contributors:
* Last Modified: 09/18/2026
* Summary: Manages the upgrade grid and menu. 
* To Do:   inventory, populate an already initialized grid.
*/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuController : MenuBase
{
    [SerializeField]
    private UpgradeTileGrid testGrid;

    [SerializeField]
    private Transform gridContainer;

    [SerializeField]
    private UpgradeTileBehavior tilePrefab;

    
    private List<UpgradeTileBehavior> tilesInGrid = new List<UpgradeTileBehavior>();

    [SerializeField]
    private GameObject inventory;

    private int gridHeight;
    private int gridWidth;

    /// <summary>
    /// Initializes the grid
    /// </summary>
    [Button("InitMenu")]
    public override void InitMenu()
    {
        base.InitMenu();
        InitGrid();
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
    private void InitGrid()
    {
        if (tilesInGrid.Count > 0)
        {
            return;
        }

        gridHeight = testGrid.height;
        gridWidth = testGrid.width;

        GridLayoutGroup layoutG = gridContainer.GetComponent<GridLayoutGroup>();

        if (layoutG == null)
        {
            throw new System.Exception("Grid Container doesnt have a layout group");
        }

        //sets the number of columns
        layoutG.constraintCount = gridWidth;

        for (int h =  0; h < gridHeight; h++)
        {
            for (int w = 0;  w < gridWidth; w++)
            {
                UpgradeTileBehavior tempTile = Instantiate(tilePrefab, gridContainer);
                tempTile.InitTile(testGrid.rows[h].rowTiles[w], new Vector2Int(w, h));
                tilesInGrid.Add(tempTile);
            }
        }

        UIPublicEvents.UpgradeGridInitialized?.Invoke();
    }

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
}
