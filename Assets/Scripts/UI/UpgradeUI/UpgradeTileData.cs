/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: Holds the data for the tiles on the upgrade grid. Does nothing on its own, just holds data
* To Do:   N/A
*/

using UnityEngine;

[System.Serializable]
public class UpgradeTileData
{

    public bool isActive;

    public GlyphScriptable glyph;

    public PinScriptable pin;

    public Vector2Int coords;

    /// <summary>
    /// Basic constructor
    /// </summary>
    public UpgradeTileData()
    {
        isActive = false;
        glyph = null;
        pin = null;
        coords = new(-1, -1);
    }

    /// <summary>
    /// Constructor that takes in some variables
    /// </summary>
    /// <param name="coordinates">the coordinates of the tile</param>
    /// <param name="active">if the tile is active or not</param>
    /// <param name="setGlyph">if the tile has a glyph</param>
    public UpgradeTileData(Vector2Int coordinates, bool active = false,  GlyphScriptable setGlyph = null)
    {
        coords = coordinates;
        isActive = active;
        glyph = setGlyph;
        pin = null;
    }

    /// <summary>
    /// sets the glyph in the tile
    /// </summary>
    /// <param name="glyph"></param>
    public void SetGlyph(GlyphScriptable glyph)
    {
        this.glyph = glyph;
    }

    /// <summary>
    /// sets the pin in the tile. TODO: make the glyph modify the pin if it has one.
    /// </summary>
    /// <param name="pin"></param>
    public void SetPin(PinScriptable pin)
    {
        this.pin = pin;
    }
}
