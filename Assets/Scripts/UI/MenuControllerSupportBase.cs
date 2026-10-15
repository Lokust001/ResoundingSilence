/*
* Author: Tyler
* Contributors:
* Last Modified: 10/14/2026
* Summary: This is the base of all the ui menus that need specialized controller support
* To Do:   N/A
*/

using UnityEngine;

public class MenuControllerSupportBase : MenuBase

{
    [HideInInspector]
    public ControllerSupportedClickable carriedItem;

    /// <summary>
    /// virtual. moves a selected item in a direction
    /// </summary>
    /// <param name="target"></param>
    /// <param name="direction"></param>
    public virtual void MoveSelectedItem(ControllerSupportedClickable target, int direction)
    {

    }

    /// <summary>
    /// Selects the default item in the menu
    /// </summary>
    public virtual void SelectDefault()
    {

    }
}
