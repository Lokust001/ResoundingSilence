/*
* Author: Tyler
* Contributors:
* Last Modified: 09/17/2026
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

    #endregion
}
