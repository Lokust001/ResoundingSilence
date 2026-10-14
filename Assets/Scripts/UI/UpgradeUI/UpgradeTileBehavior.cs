/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: Controls an individual tile on the upgrade grid.
* To Do:   N/A
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class UpgradeTileBehavior : PinHolderSlot
{
    public bool isActive;
    public Vector2Int coords;

    public List<UpgradeTileBehavior> adjacentTiles;

    public UpgradeTileData tileData;


    #region Tile Generation

    /// <summary>
    /// Initializes the tile - called when this tile initializes
    /// </summary>
    /// <param name="active">if the tile is on or off</param>
    /// <param name="coords">the tiles xy positiion in the grid</param>
    public void InitTile()
    {

        UIPublicEvents.UpgradeGridInitialized += GridInitialized;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        UIPublicEvents.UpgradeGridInitialized -= GridInitialized;
    }

    /// <summary>
    /// triggers when all tiles have been initialized
    /// </summary>
    private void GridInitialized()
    {
        adjacentTiles = UtilityFunctions.GetAdjacentTiles<UpgradeTileBehavior>(coords, 
            controller.tilesInGrid.ToList(),
             controller.currentlyEnabledGrid.height,
            controller.currentlyEnabledGrid.width);
       
    }

    /// <summary>
    /// Sets the tile data that this tilebehavior has
    /// </summary>
    /// <param name="data"></param>
    public void SetTileData(UpgradeTileData data = null)
    {
        if (data == null)
        {
            isActive = false;

            coords = new Vector2Int(-1, -1);

            //temporary
            GetComponent<Image>().enabled = false;

            GetComponent<Image>().color = Color.white;

            data = null;
            return;
        }

        tileData = data;

        isActive = data.isActive;

        this.coords = data.coords;
        gameObject.name = $"Upgrade Tile: {coords.x}, {coords.y}";

        adjacentTiles = new(8);

        GetComponent<Image>().enabled = isActive;

        if (data.glyph != null)
        {
            GetComponent<Image>().color = data.glyph.glyphColor;
        }
    }

    #endregion

    #region PinStuff

    /// <summary>
    /// sets the pin in this tile
    /// </summary>
    /// <param name="pin"></param>
    public override void SetNewPinInTile(PinItemBehavior pin)
    {
        base.SetNewPinInTile(pin);
        
        tileData.SetPin(pin.pinData);
        UIPublicEvents.PinChangedOnWeapon?.Invoke(pin.pinData);
    }

    /// <summary>
    /// function gets called whenever a pin becomes unequipped from a tile
    /// </summary>
    public override void UnequipPin()
    {
        base.UnequipPin();
        pin = null;
        tileData.SetPin(null);
        UIPublicEvents.PinChangedOnWeapon?.Invoke(null);
    }

    /// <summary>
    /// returns the neighbors of this tile. Returns as controllersupportclickables
    /// </summary>
    /// <returns></returns>
    public override List<ControllerSupportedClickable> getNeighbors()
    {
        List<ControllerSupportedClickable> returnList = new();

        for (int i = 0; i < adjacentTiles.Count; i++)
        {
            returnList.Add(adjacentTiles[i]);
        }

        return returnList;
    }

    #endregion
}
