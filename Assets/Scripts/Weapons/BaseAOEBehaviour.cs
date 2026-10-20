/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/20/2026
 * Brief: Base functionality for things that will create an AOE
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections;

public class BaseAOEBehaviour : MonoBehaviour
{
    protected BaseWeaponScriptable weaponRef;

    /// <summary>
    /// Gets a reference to the copy of the scriptable object the weapon is using
    /// </summary>
    /// <param name="dataRef"></param>
    public void SetReference(BaseWeaponScriptable dataRef)
    {
        weaponRef = dataRef;
    }
}
