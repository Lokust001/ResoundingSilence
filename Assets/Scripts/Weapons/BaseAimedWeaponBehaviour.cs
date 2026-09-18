/******************************************************************************
 * Author: Brad Dixon
 * Contributors: 
 * Last Modified: 9/18/2026
 * Brief: Handles the base functions that all aimed weapons will use
 * TODO:
 * ***************************************************************************/
using UnityEngine;

public class BaseAimedWeaponBehaviour : MonoBehaviour
{
    //Public so the upgrade manager can find the reference
    [HideInInspector] public BaseWeaponScriptable ThisWeaponData;
    [SerializeField] protected int weaponDataID;

    /// <summary>
    /// Gets a reference to the copy of the weapon's data
    /// </summary>
    virtual protected void Start()
    {
        ThisWeaponData = StaticDataManager.Instance.GetWeaponAtID(weaponDataID);
    }
}
