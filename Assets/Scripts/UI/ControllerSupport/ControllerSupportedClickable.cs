/*
* Author: Tyler
* Contributors:
* Last Modified: 10/7/2026
* Summary: Clickable for the upgrade menu
* To Do:   N/A
*/

using System.Collections.Generic;

public class ControllerSupportedClickable : Clickable
{
    /// <summary>
    /// Returns a list of neighbors to this clickable.
    /// 
    /// do not return base when overriding.
    /// </summary>
    /// <returns></returns>
    public virtual List<ControllerSupportedClickable> getNeighbors()
    {
        return new();
    }
}
