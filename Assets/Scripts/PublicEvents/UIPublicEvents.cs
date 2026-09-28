/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: Public events for the Ui system.
* To Do:   N/A
*/

using System;
using UnityEngine;

public static class UIPublicEvents
{
    public static Action HideOpenMenus;

    public static Action NewMenuOpened;

    public static Action<UiMenuType> UpdateUIManagerStack;


    #region UpgradeMenu
    public static Action UpgradeGridInitialized;

    public static Action<PinItemBehavior> PinPickedUp;

    public static Action UpgradeMenuClosed;

    #endregion
}
