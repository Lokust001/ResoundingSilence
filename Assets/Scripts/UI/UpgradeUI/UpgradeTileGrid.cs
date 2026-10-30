/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: stores the data for the tile grid
* To Do:   N/A
*/
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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
        UIPublicEvents.PinChangedOnWeapon += NewPinSet;
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
        int glyphsToPlace = Random.Range(minInclusive: NumberOfGlyphRange.x, maxExclusive: NumberOfGlyphRange.y + 1);

        if (glyphsToPlace <= 0)
        {
            return;
        }

        for (int i = 0; i < glyphsToPlace; i++)
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
        foreach (var grid in grid)
        {
            if (grid.pin != null)
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
        float damagebuff = 1.0f;
        float AttackSpeedBuff = 1.0f;
        float lifestealBuff = 1.0f;
        float BurstBuff = 1.0f;
        float AbilityDamageBuff = 1.0f;
        float AbilityCooldown = 1.0f;
        float ProjectileSpeed = 1.0f;
        float DashBuff = 1.0f;
        float DamageResistance = 1.0f;
        float MoveSpeedBuff = 1.0f;
        float HealingPotency = 1.0f;
        float knockbackAmount = 1.0f;
        float regenAmount = 1.0f;
        GetPins();
        foreach (var pin in pins)
        {
            UpgradeTileData thisTile = grid.FirstOrDefault(x => x.pin == pin);
            switch (pin.StatToChange)
            {
                case PinScriptable.WeaponStatToChange.none:
                    continue;
                case PinScriptable.WeaponStatToChange.BulletDamage:
                    damagebuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.AttackSpeed:
                    AttackSpeedBuff *= (100 - GetBuffAmount(pin, thisTile)) / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.Lifesteal:
                    lifestealBuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.BurstDamage:
                    BurstBuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.AbilityDamage:
                    AbilityDamageBuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.AbilityCooldown:
                    AbilityCooldown *= (100 - GetBuffAmount(pin, thisTile)) / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.projectileSpeed:
                    ProjectileSpeed += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.DashDistance:
                    DashBuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.DamageResistance:
                    DamageResistance *= (100 - GetBuffAmount(pin, thisTile)) / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.MoveSpeed:
                    MoveSpeedBuff += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.HealingUp:
                    HealingPotency += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
                case PinScriptable.WeaponStatToChange.GlassCannon:
                    damagebuff += (GetBuffAmount(pin, thisTile) / 100);
                    DamageResistance *= pin.SecondModifier / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.knockbackResistance:
                    knockbackAmount *= (100 - GetBuffAmount(pin, thisTile)) / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.ThickSkin:
                    DamageResistance *= (100 - GetBuffAmount(pin, thisTile)) / 100;
                    HealingPotency -= pin.SecondModifier / 100;
                    continue;
                case PinScriptable.WeaponStatToChange.Regen:
                    regenAmount += (GetBuffAmount(pin, thisTile) / 100);
                    continue;
            }
        }

        foreach (UpgradeTileData tile in grid)
        {
            if (tile.GlyphActive)
            {
                switch (tile.glyph.Type)
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
                        HealingPotency = HealingPotency + (tile.glyph.GlyphChangeAmount / 100);
                        break;
                    case GlyphScriptable.GlyphType.StatusEffect:
                        //ToDo Add the extra StatusEffect Buffs
                        break;
                }
            }
        }

        attachedWeapon.UpdateWeaponSpeed(AttackSpeedBuff);
        attachedWeapon.UpdateWeaponDamage(damagebuff);
        attachedWeapon.UpdateLifestealPercent(lifestealBuff);
        attachedWeapon.AddBurst((int)((BurstBuff * 100) / 100));
        attachedWeapon.UpdateAbilityDamage(damagebuff);
        attachedWeapon.ProjectileSpeedBoost(ProjectileSpeed);
        attachedWeapon.getPlayerModifiers(DashBuff, DamageResistance, MoveSpeedBuff, HealingPotency, knockbackAmount, regenAmount);
    }

    /// <summary>
    /// checks all the current stats of this current pin and sends it back in as a %
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="thisTile"></param>
    /// <returns></returns>
    public float GetBuffAmount(PinScriptable pin, UpgradeTileData thisTile)
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

        return buffAmount;
    }

    /// <summary>
    /// when a new pin is set check if it was on a glyph that it's compatable with
    /// </summary>
    /// <param name="newpin"></param>
    public void CheckGlyphs(PinScriptable newpin)
    {
        UpgradeTileData tile = grid.FirstOrDefault(x => x.pin == newpin);
        if (tile.glyph != null && newpin.Types[0] == tile.glyph.PinTypeToModify)
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
    public float GetAdjacentBuff(UpgradeTileData tile)
    {
        float BuffAmount = 0;

        AdajacencyTest = UtilityFunctions.GetAdjacentTiles<UpgradeTileData>(tile.coords, grid, height, width);
        if (AdajacencyTest != null)
        {
            if (AdajacencyTest[0] != null && AdajacencyTest[0].pin != null && tile.pin.NorthAdjacency != null && CheckAdjacency(tile.pin.NorthAdjacency, AdajacencyTest[0].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[1] != null && AdajacencyTest[1].pin != null && tile.pin.NorthEastAdjacency != null && CheckAdjacency(tile.pin.NorthEastAdjacency, AdajacencyTest[1].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[2] != null && AdajacencyTest[2].pin != null && tile.pin.EastAdjacency != null && CheckAdjacency(tile.pin.EastAdjacency, AdajacencyTest[2].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[3] != null && AdajacencyTest[3].pin != null && tile.pin.SouthEastAdjacency != null && CheckAdjacency(tile.pin.SouthEastAdjacency, AdajacencyTest[3].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[4] != null && AdajacencyTest[4].pin != null && tile.pin.SouthAdjacency != null && CheckAdjacency(tile.pin.SouthAdjacency, AdajacencyTest[4].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[5] != null && AdajacencyTest[5].pin != null && tile.pin.SouthWestAdjacency != null && CheckAdjacency(tile.pin.SouthWestAdjacency, AdajacencyTest[5].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[6] != null && AdajacencyTest[6].pin != null && tile.pin.WestAdjacency != null && CheckAdjacency(tile.pin.WestAdjacency, AdajacencyTest[6].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;

            if (AdajacencyTest[7] != null && AdajacencyTest[7].pin != null && tile.pin.NorthWestAdjacency != null && CheckAdjacency(tile.pin.NorthWestAdjacency, AdajacencyTest[7].pin.Types))
                BuffAmount += tile.pin.AdjacentTileScaling;
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

    /// <summary>
    /// helper function to check if any of the allowed adjacenent types return true or false
    /// </summary>
    /// <param name="accepted"></param>
    /// <param name="adjacentTypes"></param>
    /// <returns></returns>
    private bool CheckAdjacency(List<PinScriptable.PinType> accepted, List<PinScriptable.PinType> adjacentTypes)
    {
        foreach (PinScriptable.PinType tester in accepted)
        {
            foreach (PinScriptable.PinType type in adjacentTypes)
            {
                if (tester == type)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
