/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: stores the data for the tile grid
* To Do:   N/A
*/

using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UpgradeTileGrid
{
    [Tooltip("X is the minimum number of glyphs this can have (inclusive), Y is the maximum number of glyphs (inclusive)")]
    public Vector2Int NumberOfGlyphRange;

    public BaseWeaponScriptable attachedWeapon;

    [OnValueChanged(nameof(UpdateGridHeightCount)), AllowNesting, Range(0, 10)]
    public int height;

    [OnValueChanged(nameof(UpdateList)), AllowNesting, Range(0, 10)]
    public int width;

    [Header("Grid tiles"), Tooltip("This is the upgrade grid - 0, 0 is the top left element and it goes to the right.")]
    public List<GridRow> rows;

    public PlayerController playerController;

    public List<UpgradeTileData> grid { get; private set; } = new();

    public List<PinScriptable> pins { get; private set; } = new();

    public List<UpgradeTileData> AdajacencyTest { get; private set; } = new();

    /// <summary>
    /// subscride to the event
    /// </summary>
    public void Awake()
    {
        
    }

    /// <summary>
    /// unsubscride to the event
    /// </summary>
    public void OnDestroy()
    {
        UIPublicEvents.PinChangedOnWeapon -= NewPinSet;
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

        UIPublicEvents.PinChangedOnWeapon += NewPinSet;

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

    /// <summary>
    /// wrapper to call all the data changing info when a new pin is set
    /// </summary>
    /// <param name="pin"></param>
    public void NewPinSet(PinScriptable pin)
    {
        CheckGlyphs(pin);
        SendDatatoWeapon(pin);
    }

    /// <summary>
    /// Collects all the data from the pins on the weapon and sends it to buff the weapon stats
    /// </summary>
    public void SendDatatoWeapon(PinScriptable newpin)
    {
        UpgradeTileData data = grid.FirstOrDefault(x  => x.pin == newpin);

        float damagebuff = 1.0f;
        float AttackSpeedBuff = 1.0f;
        float lifestealBuff = 1.0f;
        GetPins();
        foreach(var pin in pins)
        {
            UpgradeTileData thisTile = grid.FirstOrDefault(x => x.pin == pin);
            if (pin.StatToChange == PinScriptable.WeaponStatToChange.BulletDamage)
            {
                float buffAmount = pin.ModifierNumber;
                if(pin.AdditiveScaling)
                {
                    buffAmount += GetAdjacentBuff(thisTile);
                }
                else
                {
                    buffAmount *= (1 + (GetAdjacentBuff(thisTile)/100));
                }

                if ((thisTile.GlyphActive && thisTile.glyph.Type == GlyphScriptable.GlyphType.DoubleThisGlyph) || doubleGlyphAdjacent(thisTile))
                {
                    buffAmount *= 2;
                }
                damagebuff += (buffAmount / 100);
                
            }
            else if(pin.StatToChange == PinScriptable.WeaponStatToChange.AttackSpeed)
            {
                float buffAmount = pin.ModifierNumber;
                if(pin.AdditiveScaling)
                {
                    buffAmount += GetAdjacentBuff(thisTile);
                }
                else
                {
                    buffAmount *= (1 + (GetAdjacentBuff(thisTile) / 100));
                }

                if ((thisTile.GlyphActive && thisTile.glyph.Type == GlyphScriptable.GlyphType.DoubleThisGlyph) || doubleGlyphAdjacent(thisTile))
                {
                    buffAmount *= 2;
                }
                AttackSpeedBuff *= (100 - buffAmount) / 100;
            }
            else if (pin.StatToChange == PinScriptable.WeaponStatToChange.Lifesteal)
            {
                float buffAmount = pin.ModifierNumber;
                if (pin.AdditiveScaling)
                {
                    buffAmount += GetAdjacentBuff(thisTile);
                }
                else
                {
                    buffAmount *= (1 + (GetAdjacentBuff(thisTile) / 100));
                }

                if ((thisTile.GlyphActive && thisTile.glyph.Type == GlyphScriptable.GlyphType.DoubleThisGlyph) || doubleGlyphAdjacent(thisTile))
                {
                    buffAmount *= 2;
                }
                lifestealBuff += (buffAmount / 100);
            }
        }

        foreach(UpgradeTileData tile in grid)
        {
            if (tile.GlyphActive)
            {
                switch(tile.glyph.Type)
                {
                    case 0:
                        break;
                    case GlyphScriptable.GlyphType.CooldownReduction:
                        //ToDo add this to the cooldown of the weapons
                        break;
                    case GlyphScriptable.GlyphType.DamageBuff:
                        damagebuff *= 1 + (tile.glyph.GlyphChangeAmount / 100);
                        break;
                    case GlyphScriptable.GlyphType.HealthingReceived:
                        playerController.healingPotency = 1 + (tile.glyph.GlyphChangeAmount / 100);
                        break;
                    case GlyphScriptable.GlyphType.StatusEffect: 
                        //ToDo Add the extra StatusEffect Buffs
                        break;
                }
            }
        }

        attachedWeapon.updateWeaponSpeed(AttackSpeedBuff);
        attachedWeapon.updateWeaponDamage(damagebuff);
        attachedWeapon.updateLifestealPercent(lifestealBuff);
    }


    /// <summary>
    /// when a new pin is set check if it was on a glyph that it's compatable with
    /// </summary>
    /// <param name="newpin"></param>
    public void CheckGlyphs(PinScriptable newpin)
    {
        UpgradeTileData tile = grid.FirstOrDefault(x => x.pin == newpin);
        if(tile.glyph != null && newpin.Type == tile.glyph.PinTypeToModify)
        {
            tile.GlyphActive = true;
        }
        else
        {
            tile.GlyphActive = false;
        }
    }

    /// <summary>
    /// funtion to go through all the adjacent tiles and if there pins of the same type adjacent it stores how much of a buff the pin will get and returns that amount
    /// </summary>
    /// <param name="tile"></param>
    /// <returns> % amount buff that revieved by the adjacent tiles</returns>
    public int GetAdjacentBuff(UpgradeTileData tile)
    {
        int BuffAmount = 0;
        AdajacencyTest = UtilityFunctions.GetAdjacentTiles<UpgradeTileData>(tile.coords, grid, height, width);
        foreach (var currentTile in AdajacencyTest)
        {
            if (currentTile != null && currentTile.pin != null && tile.pin.Type == currentTile.pin.Type)
            {
                BuffAmount += tile.pin.AdjacentTileScaling;
            }
        }
        return BuffAmount;
    }

    /// <summary>
    /// Checks if adjacent tile to the current tile has a glyph the doubles the effects of adjacent tiles
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool doubleGlyphAdjacent(UpgradeTileData tile)
    {
        List<UpgradeTileData> temp = UtilityFunctions.GetAdjacentTiles<UpgradeTileData>(tile.coords, grid, height, width);
        foreach (var currentTile in temp)
        {
            if (currentTile != null && currentTile.GlyphActive && currentTile.glyph.Type == GlyphScriptable.GlyphType.DoubleAdjacentGlyphs)
            {
                return true;
            }
        }
        return false;
    }
}
