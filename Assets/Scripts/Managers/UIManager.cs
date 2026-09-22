/*
* Author: Tyler
* Contributors:
* Last Modified: 09/17/2026
* Summary: Manages which UI menus are open at any given time. 
*          If there's anything that pertains to the UI as a whole, it's here.
* To Do:   N/A
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UIManager : BaseManager
{
    #region Variables

    #region InspectorFacing

    [SerializeField]
    private Canvas mainCanvasPrefab;

    [SerializeField]
    private List<MenuBase> menuPrefabs = new List<MenuBase>();

    #endregion

    #region Private
    private Canvas mainCanvas;

    private List<MenuBase> instantiatedMenus = new List<MenuBase>();
    private List<MenuBase> unInstantiatedMenus = new List<MenuBase>();

    public static UIManager Instance;

    private Stack<UiMenuType> currentlyOpenedMenus = new Stack<UiMenuType>();

    #endregion

    #endregion

    #region Setup

    /// <summary>
    /// Initializes the UI systems.
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

        
        await SetUpCanvas();
        await SetUpMenus();
        await SetupPublicEvents();


        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates the canvas
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpCanvas()
    {
        mainCanvas = Instantiate(mainCanvasPrefab, transform);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Sets up all of the lists with the menus in them to spread 
    /// the instantiation load throughout the game
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpMenus()
    {
        //ensures there aren't any menus open at base (this statement should always return false)
        if (instantiatedMenus.Count > 0)
        {
            foreach (MenuBase menu in instantiatedMenus)
            {
                Destroy(menu.gameObject);
            }

            instantiatedMenus.Clear();
        }
        

        foreach (MenuBase menuPrefab in menuPrefabs)
        {
            unInstantiatedMenus.Add(menuPrefab);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// All public event subscriptions go here
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetupPublicEvents()
    {
        GenericPublicEvents.AllManagersInitialized += GameStarted;
        UIPublicEvents.UpdateUIManagerStack += UpdateUiManagerStack;

        await Task.CompletedTask;
    }

    /// <summary>
    /// all public event unsubscriptions go here.
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.AllManagersInitialized -= GameStarted;
        UIPublicEvents.UpdateUIManagerStack -= UpdateUiManagerStack;
    }

    #endregion

    #region Wrappers

    /// <summary>
    /// opens the main menu once all managers are initialized
    /// </summary>
    private void GameStarted()
    {
        UpdateUiManagerStack(UiMenuType.MainMenu);
    }

    #endregion

    #region SwappingMenus

    /// <summary>
    /// Updates the currentyOpenedMenus stack
    /// </summary>
    /// <param name="menuType"> the new menu to open </param>
    /// <exception cref="System.Exception"></exception>
    private void UpdateUiManagerStack(UiMenuType menuType = UiMenuType.None)
    { 
        //edge case exceptions
        if (menuType == UiMenuType.None)
        {
            throw new System.Exception($"Tried to open a menu with type {menuType}");
        }
        if (currentlyOpenedMenus.Count > 0 && menuType == currentlyOpenedMenus.Peek())
        {
            throw new System.Exception($"Tried to open a menu thats already open: {menuType}");
        }

        //hide the menus that are open rn
        UIPublicEvents.HideOpenMenus?.Invoke();

        //updates the stack with the newest menu type
        currentlyOpenedMenus.Push(menuType);

        //opens all of the new menus
        OpenMenus();
    }

    /// <summary>
    /// opens all menus that have the menu type of the top of currentlyOpenedMenus
    /// </summary>
    private void OpenMenus()
    {
        //opens all existing menus
        foreach (MenuBase menu in instantiatedMenus)
        {
            if (menu.GetMenuType() == currentlyOpenedMenus.Peek())
            {
                menu.gameObject.SetActive(true);
            }
        }

        //just a temporary list to move the needed menuprefabs from uninstantiatedMenus to instantiatedMenus
        List<MenuBase> tempSwapList = new List<MenuBase>();

        //grabs all menus with the right type from the menu prefabs
        foreach (MenuBase unInstantiatedMenu in unInstantiatedMenus)
        {
            if (unInstantiatedMenu.GetMenuType() == currentlyOpenedMenus.Peek())
            {
                tempSwapList.Add(unInstantiatedMenu);
                
            }
        }

        //removes the prefab from the list and spawns in the menu
        foreach (MenuBase menuPrefab in tempSwapList)
        {
            unInstantiatedMenus.Remove(menuPrefab);
            MenuBase instantiatedMenu = Instantiate(menuPrefab, mainCanvas.transform);
            instantiatedMenus.Add(instantiatedMenu);

            //sets up the menu
            instantiatedMenu.InitMenu(); 
        }

        //tells all the menus that theyve been opened
        UIPublicEvents.NewMenuOpened?.Invoke();
    }

    /// <summary>
    /// Closes all menus that are open right now and opens the next menu
    /// </summary>
    public void CloseCurrentMenus()
    {
        if (currentlyOpenedMenus.Count <= 1)
        {
            throw new System.Exception("Tried to close the menu thats the last menu in the stack");
        }

        UIPublicEvents.HideOpenMenus?.Invoke();
        currentlyOpenedMenus.Pop();
        UpdateUiManagerStack(currentlyOpenedMenus.Peek());
    }

    #endregion

    #region GettersSetters

    /// <summary>
    /// Gets the reference to the mainCanvas
    /// </summary>
    /// <returns></returns>
    public Canvas GetMainCanvas()
    {
        return mainCanvas;
    }

    /// <summary>
    /// Gets the menuType that is currently open
    /// </summary>
    /// <returns></returns>
    public UiMenuType GetCurrentMenu()
    {
        return currentlyOpenedMenus.Peek();
    }

    /// <summary>
    /// Returns whether or not the player is currently in a fullscreen menu or in the game world proper.
    /// </summary>
    /// <returns></returns>
    public bool CurrentlyInFullscreenMenu()
    {
        if (currentlyOpenedMenus.Count <= 0)
        {
            throw new System.Exception("Tried to check if in full screen - currently opened menus has no items in it");
        }

        return (currentlyOpenedMenus.Peek() == UiMenuType.MainMenu ||
                currentlyOpenedMenus.Peek() == UiMenuType.UpgradeMenu);
    }

    #endregion
}

public enum UiMenuType
{
    None,
    MainMenu,
    UpgradeMenu
}
