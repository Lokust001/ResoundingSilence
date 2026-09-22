/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: Controls an individual tile on the upgrade grid.
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeTileBehavior : MonoBehaviour, IPointerClickHandler
{
    public bool isActive;
    public Vector2Int coords;

    [SerializeField]
    private UpgradeTileBehavior[] adjacentTiles;
    private UpgradeMenuController controller;

    public UpgradeTileData tileData;

    

    #region Tile Generation

    /// <summary>
    /// Initializes the tile - called when this tile initializes
    /// </summary>
    /// <param name="active">if the tile is on or off</param>
    /// <param name="coords">the tiles xy positiion in the grid</param>
    public void InitTile()
    {
        controller = GetComponentInParent<UpgradeMenuController>();

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
        adjacentTiles = controller.getAdjacentTiles(coords);
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

            return;
        }

        isActive = data.isActive;

        this.coords = data.coords;
        gameObject.name = $"Upgrade Tile: {coords.x}, {coords.y}";

        adjacentTiles = new UpgradeTileBehavior[8];

        GetComponent<Image>().enabled = isActive;
    }

    #endregion

    #region PinStuff

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            controller.PlacePinInTile(this);
        }
    }

    public void SetNewPinInTile(PinItemBehavior pin)
    {
        tileData.SetPin(pin.pinData);
    }

    public void UnequipPin()
    {

    }

    #endregion
}
