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
        UIPublicEvents.HideOpenMenus += CloseMenu;
        UIPublicEvents.NewMenuOpened += OpenMenu;
    }

    /// <summary>
    /// Mostly empty function that exists to be overwritten with actual functionality dependant on the menu.
    /// Contains error checks. Designed to be at the top of the overridden function, not the bottom.
    /// </summary>
    protected virtual void OpenMenu()
    {
        if (UIManager.Instance.GetCurrentMenu() != menuType) {
            gameObject.SetActive(false);
            return;
        }

        SetUpPublicEvents();
    }

    /// <summary>
    /// Mostly empty function that closes the current menu.
    /// 
    /// Designed to be at the bottom of the overridden function, not the top
    /// </summary>
    protected virtual void CloseMenu()
    {
        TearDownPublicEvents();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// subscribes to all public events not related to initialization.
    /// </summary>
    protected virtual void SetUpPublicEvents()
    {
        
    }

    /// <summary>
    /// unsubscribes form all public events not related to initialization
    /// </summary>
    protected virtual void TearDownPublicEvents()
    {
        
    }

    /// <summary>
    /// unsubscribes from all public events.
    /// </summary>
    protected virtual void OnDestroy()
    {
        UIPublicEvents.HideOpenMenus -= CloseMenu;
        UIPublicEvents.NewMenuOpened -= OpenMenu;
        TearDownPublicEvents();
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
