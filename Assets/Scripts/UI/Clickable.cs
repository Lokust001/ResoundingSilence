/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: Contains virtual functions to be inherited by any ui obejct that is clickable
* To Do:   N/A
*/

using UnityEngine;

public class Clickable : MonoBehaviour
{
    /// <summary>
    /// triggers when this object is clicked on
    /// </summary>
    public virtual void ClickedOn()
    {

    }

    /// <summary>
    /// triggers when this object is hovered over
    /// </summary>
    public virtual void HoveredOver()
    {

    }

    /// <summary>
    /// triggers when this object is no longer hovered over
    /// </summary>
    public virtual void UnHoveredOver()
    {

    }
}
