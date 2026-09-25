/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: stores the data for the tile grid
* To Do:   N/A
*/

using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[System.Serializable]
public class UpgradeTileGrid
{
    [Tooltip("X is the minimum number of glyphs this can have (inclusive), Y is the maximum number of glyphs (inclusive)")]
    //will be hidden until my next pr.
    [HideInInspector]
    public Vector2Int NumberOfGlyphRange;

    public BaseWeaponScriptable attachedWeapon;

    [OnValueChanged(nameof(UpdateGridHeightCount)), AllowNesting, Range(0, 10)]
    public int height;

    [OnValueChanged(nameof(UpdateList)), AllowNesting, Range(0, 10)]
    public int width;

    [Header("Grid tiles"), Tooltip("This is the upgrade grid - 0, 0 is the top left element and it goes to the right.")]
    public List<GridRow> rows;

    public List<UpgradeTileData> grid { get; private set; } = new();

    public List<PinScriptable> pins { get; private set; } = new();

    public void Awake()
    {
        UIPublicEvents.UpgradeMenuClosed += SendDatatoWeapon;
    }

    [System.Serializable]
    public class GridRow
    {
        [Tooltip("This is the upgrade grid - 0, 0 is the top left element and it goes to the right.")]
        public List<bool> rowTiles;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="width"></param>
        public GridRow(int width)
        {
            rowTiles = new List<bool>();
            for (int i = 0; i < width; i++)
            {

                rowTiles.Add(false);
            }
        }
        
        /// <summary>
        /// updates the rows with the width so it can be easily changed in the inspector
        /// </summary>
        /// <param name="numberOfTiles"></param>
        public void UpdateGridRowCount(int numberOfTiles)
        {
            if (rowTiles.Count == numberOfTiles)
            {
                return;
            }

            if (rowTiles.Count < numberOfTiles)
            {
                for (int i = rowTiles.Count; i < numberOfTiles; i++)
                {
                    rowTiles.Add(false);
                }
            }
            else
            {
                int temp = numberOfTiles - 1;
                if (temp <= 0)
                {
                    temp = 0;
                }
                rowTiles.RemoveRange(numberOfTiles - 1, rowTiles.Count - numberOfTiles);
            }
        }
    }

    private bool HasBeenInitialized;

    /// <summary>
    /// updates each grid element when the width changes
    /// </summary>
    private void UpdateList()
    {
        foreach (GridRow row in rows)
        {
            row.UpdateGridRowCount(width);
        }
    }

    /// <summary>
    /// updates the number of rows when the grid height changes
    /// </summary>
    public void UpdateGridHeightCount()
    {
        
        if (rows.Count == height || height < 0)
        {
            return;
        }

        if (rows.Count < height)
        {
            for (int i = rows.Count; i < height; i++)
            {
                rows.Add(new(width));
            }
        }
        else
        {
            int temp = height - 1;
            if (temp <= 0)
            {
                temp = 0;
            }
            rows.RemoveRange(temp, rows.Count - height);
        }
    }

    /// <summary>
    /// Initializes the tiledatas in the grid. 
    /// </summary>
    public void InitGrid()
    {
        //prevents multiple initializations
        if (HasBeenInitialized)
        {
            return;
        }
        else
        {
            HasBeenInitialized = true;
        }

        List<UpgradeTileData> enabledUnGlyphedTiles = new();

        //initialize each tile
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                UpgradeTileData tempData = new(new Vector2Int(w, h), rows[h].rowTiles[w]);

                grid.Add(tempData);

                //if the tile is enabled, it can have a glyph on it
                if (rows[h].rowTiles[w])
                {
                    enabledUnGlyphedTiles.Add(tempData);
                }
            }
        }

        //place glyphs randomly
        int glyphsToPlace = Random.Range(minInclusive: NumberOfGlyphRange.x, maxExclusive:NumberOfGlyphRange.y + 1);

        if (glyphsToPlace <= 0)
        {
            return;
        }

        for (int i =  0; i < glyphsToPlace; i++)
        {
            enabledUnGlyphedTiles[Random.Range(0, enabledUnGlyphedTiles.Count)].SetGlyph(StaticDataManager.Instance.GetRandomGlyph());
        }

        
    }

    /// <summary>
    /// gathers all the pins into a list
    /// </summary>
    public void GetPins()
    {
        pins.Clear();
        foreach(var grid in grid)
        {
            if(grid.pin != null)
            {
                pins.Add(grid.pin);
            }
        }
    }

    public void SendDatatoWeapon()
    {
        float damagebuff = 1.0f;
        float AttackSpeedBuff = 1.0f;
        foreach(var pin in pins)
        {
            if(pin.StatToChange == PinScriptable.WeaponStatToChange.bulletDamage)
            {
                damagebuff += pin.ModifierNumber;
            }
            else if(pin.StatToChange == PinScriptable.WeaponStatToChange.attackSpeed)
            {
                AttackSpeedBuff /= pin.ModifierNumber;
            }
        }
        attachedWeapon.updateWeaponSpeed(AttackSpeedBuff);
        attachedWeapon.updateWeaponDamage(damagebuff);
    }
}
