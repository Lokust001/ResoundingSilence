/*
* Author: Tyler
* Contributors:
* Last Modified: 10/2/2026
* Summary: Utility script. Is just for math functoins to save space and time
* To Do:   N/A
*/

using System;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityFunctions
{
    /// <summary>
    /// Returns a list of all adjacent tiles of a given coordinate in a given list
    /// </summary>
    /// <typeparam name="T"> The type to return. </typeparam>
    /// <param name="coords"> The coordinates of the center tile. </param>
    /// <param name="collection"> The list of objects to grab adjacency from. </param>
    /// <param name="height"> How tall the grid is. </param>
    /// <param name="width"> How wide the grid is. </param>
    /// <param name="countDiagonals"> If you want the returned list to include diagonals, this should be true. True by default.</param>
    /// <returns></returns>
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

    /// <summary>
    /// gets adjacent tiles given the index, not the coords
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"> the index of the item in the list </param>
    /// <param name="collection"> The list of objects to grab adjacency from. </param>
    /// <param name="height"> How tall the grid is. </param>
    /// <param name="width"> How wide the grid is. </param>
    /// <param name="countDiagonals"> If you want the returned list to include diagonals, this should be true. True by default.</param>
    /// <returns></returns>
    public static List<T> GetAdjacentTiles<T>(int index,
                                              List<T> collection,
                                              int height, int width,
                                              bool countDiagonals = true)
    {
        Vector2Int coords = new Vector2Int(index % width, index / width);

        return GetAdjacentTiles<T>(coords, collection, height, width, countDiagonals);
    }

    /// <summary>
    /// helper function - returns the vector 2 direction when given a number.
    /// </summary>
    /// <param name="i">0 = north, then moving clockwise with 7 = northwest. </param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
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
    /// converts a vec2int into the directional int. 0 = north, then moving clockwise with 7 = northwest.
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static int ConvertVecIntToIntDirection(Vector2Int dir)
    {
        if (dir.x == 0 && dir.y == 1)
        {
            return 0;
        }
        if (dir.x == 1 && dir.y == 1)
        {
            return 1;
        }
        if (dir.x == 1 && dir.y == 0)
        {
            return 2;
        }
        if (dir.x == 1 && dir.y == -1)
        {
            return 3;
        }
        if (dir.x == 0 && dir.y == -1)
        {
            return 4;
        }
        if (dir.x == -1 && dir.y == -1)
        {
            return 5;
        }

        //has to be in this order or else it skips over to -1. idk why
        if (dir.x == -1 && dir.y == 1)
        {
            return 7;
        }
        if (dir.x == -1 && dir.y == 0)
        {
            return 6;
        }
        


        return -1;
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
    private static T GetTile<T>(Vector2Int coords, List<T> collection, int height, int width)
    {
        if (coords.x < 0 || coords.x >= width ||
            coords.y < 0 || coords.y >= height)
        {
            return default(T);
        }

        return GetTile(coords.x + (coords.y * width), collection);
    }
}

