/*
* Author: Tyler
* Contributors:
* Last Modified: 09/18/2026
* Summary: Controls an individual tile on the upgrade grid.
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class UpgradeTileBehavior : MonoBehaviour
{
    public bool isActive;
    public Vector2Int coords;
    [SerializeField]
    private UpgradeTileBehavior[] adjacentTiles;
    private UpgradeMenuController controller;

    /// <summary>
    /// Initializes the tile - called when this tile initializes
    /// </summary>
    /// <param name="active">if the tile is on or off</param>
    /// <param name="coords">the tiles xy positiion in the grid</param>
    public void InitTile(bool active = false, Vector2Int coords = new())
    {
        isActive = active;

        this.coords = coords;
        gameObject.name = $"Upgrade Tile: {coords.x}, {coords.y}";

        adjacentTiles = new UpgradeTileBehavior[8];

        controller = GetComponentInParent<UpgradeMenuController>();

        UIPublicEvents.UpgradeGridInitialized += GridInitialized;

        //temp
        if (isActive)
        {
            GetComponent<Image>().color = Color.green;
        }
        
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
}
