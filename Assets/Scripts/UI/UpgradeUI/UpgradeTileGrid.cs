using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class UpgradeTileGrid
{
    [OnValueChanged(nameof(UpdateGridHeightCount)), AllowNesting, Range(0, 10)]
    public int height;

    [OnValueChanged(nameof(UpdateList)), AllowNesting, Range(0, 10)]
    public int width;

    [Header("Grid tiles"), Tooltip("This is the upgrade grid - 0, 0 is the top left element and it goes to the right.")]
    public List<GridRow> rows;

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
}
