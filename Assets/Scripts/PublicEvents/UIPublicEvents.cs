/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: Public events for the Ui system.
* To Do:   N/A
*/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIPublicEvents
{
    public static Action HideOpenMenus;

    public static Action NewMenuOpened;

    public static Action<UiMenuType> UpdateUIManagerStack;

    public static Action CurrentMenuClosed;

    public static Action<ControllerSupportedClickable> SelectSpecificClickable;

    #region PlayerHUD

    public static Action<SideObjective> UpdateHUDHuntObjs;

    public static Action UpdatePlayerHealthSlider;

    public static Action UpdatePlayerEXPSlider;

    public static Action<CooldownUIController.CooldownToUpdate, float> UpdateHUDCooldownUI;
    public static Action<CooldownUIController.CooldownToUpdate> ImmediatelyRefreshCooldown;

    public static Action<Sprite, Sprite, CooldownUIController.CooldownToUpdate> ChangeCooldownImages;

    

    #endregion

    #region UpgradeMenu

    public static Action UpgradeMenuOpened;
    public static Action UpgradeMenuClosed;
    public static Action UpgradeGridInitialized;

    public static Action<Vector2> UpdateCarriedItemPosition;

    public static Action NewPinPlacedInTile;

    public static Action<ItemHolderSlot> SelectSpecificTile;

    public static Action<CarriedItemBehavior> ItemPickedUp;

    public static Action<PinScriptable> PinChangedOnWeapon;

    public static Action InventoryUpdated;

    #endregion

    #region Tooltips

    public static Action<TMP_Text> SetNewTooltipTextObject;

    #endregion

    #region SettingsMenu

    public static Action SettingsMenuOpened;

    public static Action SettingsMenuClosed;

    #endregion

    #region TarotMenu

    public static Action TarotMenuOpened;

    public static Action TarotMenuClosed;

    #endregion

    #region HuntBoard

    public static Action<SideObjective> UpdateHuntBoardData;

    #endregion
}
