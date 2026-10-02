using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions
{
    public static List<T> GetAdjacentTiles<T>(Vector2Int coords,
                                              List<T> collection,
                                              int height, int width,
                                              bool countDiagonals = true)
    {
        List<T> temp = new();

        int count = countDiagonals ? 8 : 4;

        for (int i = 0; i < count; i++)
        {
            temp.Add(GetTile(coords + GetDirectionGivenInt(countDiagonals ? i : i * 2), collection, height, width));
        }
        return temp;
    }

    private static Vector2Int GetDirectionGivenInt(int i = -1)
    {
        if (i < 0 || i >= 8)
        {
            throw new System.Exception($"Tried to get a direction that is out of bounds: tried for direction {i}");
        }

        switch (i)
        {
            case 0:
                return new Vector2Int(0, -1);
            case 1:
                return new Vector2Int(1, -1);
            case 2:
                return new Vector2Int(1, 0);
            case 3:
                return new Vector2Int(1, 1);
            case 4:
                return new Vector2Int(0, 1);
            case 5:
                return new Vector2Int(-1, 1);
            case 6:
                return new Vector2Int(-1, 0);
            case 7:
                return new Vector2Int(-1, -1);
            default:
                throw new System.Exception($"Tried to get a direction that is out of bounds in the " +
                    $"switch statement: tried for direction {i}");
        }
    }

    /// <summary>
    /// grabs a tile given the index in the given list
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private static T GetTile<T>(int index, List<T> collection)
    {
        if (index < 0 || index >= collection.Count)
        {
            return default(T);
        }
        else
        {
            return collection[index];
        }
    }

    /// <summary>
    /// grabs the tile given the coordinates in the given list. width and height are for out of bounds testing
    /// </summary>
    /// <param name="coords"></param>
    /// <returns></returns>
    private static T GetTile<T>(Vector2Int coords, List<T> collection, int width, int height)
    {
        if (coords.x < 0 || coords.x >= width ||
            coords.y < 0 || coords.y >= height)
        {
            return default(T);
        }

        return GetTile(coords.x + (coords.y * width), collection);
    }
}

