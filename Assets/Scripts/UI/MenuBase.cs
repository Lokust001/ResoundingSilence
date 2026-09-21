/*
* Author: Tyler
* Contributors:
* Last Modified: 09/17/2026
* Summary: This is the base of all of the UI menus in the game.
* To Do:   N/A
*/

using UnityEngine;

public class MenuBase : MonoBehaviour
{
    [SerializeField]
    protected UiMenuType menuType;

    /// <summary>
    /// Initializes the menu. Can go anywhere in the overridden function.
    /// </summary>
    public virtual void InitMenu()
    {
        SetUpPublicEvents();
    }

    /// <summary>
    /// Mostly empty function that exists to be overwritten with actual functionality dependant on the menu.
    /// Contains error checks. Designed to be at the top of the overridden function, not the bottom.
    /// </summary>
    protected virtual void OpenMenu()
    {
        if (UIManager.Instance.GetCurrentMenu() != menuType) {
            Debug.Log($"Menu was active with the wrong type " +
                $"open\nCurrentMenu: {UIManager.Instance.GetCurrentMenu()}, this menu type: {menuType}");
            gameObject.SetActive(false);
            return;
        }
    }

    /// <summary>
    /// Mostly empty function that closes the current menu.
    /// 
    /// Designed to be at the bottom of the overridden function, not the top
    /// </summary>
    protected virtual void CloseMenu()
    { 
        gameObject.SetActive(false);
    }

    /// <summary>
    /// subscribes to all public events needed.
    /// </summary>
    protected virtual void SetUpPublicEvents()
    {
        UIPublicEvents.HideOpenMenus += CloseMenu;
        UIPublicEvents.NewMenuOpened += OpenMenu;
    }

    /// <summary>
    /// unsubscribes from all public events.
    /// </summary>
    protected virtual void OnDestroy()
    {
        UIPublicEvents.HideOpenMenus -= CloseMenu;
        UIPublicEvents.NewMenuOpened -= OpenMenu;
    }

    /// <summary>
    /// Gets the menu type of this menu.
    /// </summary>
    /// <returns>This menu's specific menu type </returns>
    public UiMenuType GetMenuType()
    {
        return menuType;
    }
}
