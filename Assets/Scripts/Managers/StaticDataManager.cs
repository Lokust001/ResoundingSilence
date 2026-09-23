/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: This is a database that will hold all of the static data for the game.
*          Everything here is designed to be read-only.
* To Do:   N/A
*/

using UnityEngine;
using System.Collections.Generic;

public class StaticDataManager : BaseManager
{
    public static StaticDataManager Instance;

    [SerializeField]
    private List<BaseWeaponScriptable> weaponDatas;

    [SerializeField]
    private List<GlyphScriptable> possibleGlyphs;

    /// <summary>
    /// Initializes the manager
    /// </summary>
    /// <returns></returns>
    public override async Awaitable InitManager()
    {
        await base.InitManager();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #region Weapon Getters

    /// <summary>
    /// Returns a copy of the weapondata at the specified id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public BaseWeaponScriptable GetWeaponAtID(int id)
    {
        if (id < 0 || id >= weaponDatas.Count)
        {
            throw new System.Exception("Tried to get a weapon at an id that doesnt exist");
        }

        return weaponDatas[id].CreateNonRefCopy<BaseWeaponScriptable>();
    }

    /// <summary>
    /// Returns a copy of the glyph at the specified id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public GlyphScriptable GetGlyphAtID(int id)
    {
        if (id < 0 || id >= possibleGlyphs.Count)
        {
            throw new System.Exception("Tried to get a weapon at an id that doesnt exist");
        }

        return possibleGlyphs[id].CreateNonRefCopy<GlyphScriptable>();
    }

    /// <summary>
    /// Returns a copy of a random glyph.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public GlyphScriptable GetRandomGlyph()
    {
        if (possibleGlyphs.Count <= 0)
        {
            throw new System.Exception("Tried to get a random glyph when none exist.");
        }

        return possibleGlyphs[Random.Range(0, possibleGlyphs.Count)].CreateNonRefCopy<GlyphScriptable>();
    }

    #endregion
}
