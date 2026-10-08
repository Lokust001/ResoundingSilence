/*
* Author: Tyler
* Contributors:
* Last Modified: 10/7/2026
* Summary: This service controls the controller's ability to switch between scenes.
* To Do:   N/A
*/

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerUIService : BaseService
{
    public static ControllerUIService Instance;

    private GameObject defaultSelectedButton;


    /// <summary>
    /// Initializes the service
    /// </summary>
    /// <returns></returns>
    public override async Awaitable InitService()
    {
        await base.InitService();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        await SetupPublicEvents();
    }

    /// <summary>
    /// Turns on the public events for turning on and off controller
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetupPublicEvents()
    {
        InputPublicEvents.ControllerEnabled += SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled += DeselectUI;
        await Task.CompletedTask;
    }

    /// <summary>
    /// turns off the publicv events for turning on and off controller
    /// </summary>
    private void OnDestroy()
    {
        InputPublicEvents.ControllerEnabled -= SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled -= DeselectUI;
    }

    /// <summary>
    /// Turns off the currently selected ui
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void DeselectUI()
    {
        if (InputManager.Instance.ControllerIsEnabled)
        {
            return;
        }

        if (EventSystem.current == null)
        {
            throw new System.Exception("No event system found");
        }



        //add to this if statement for every menu i have to make custom controller supp for
        if (UIManager.Instance.GetCurrentMenu() == UiMenuType.UpgradeMenu)
        {
            FindAnyObjectByType<UpgradeControllerSupportManager>().DeselectCurrentObject();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }


    }

    /// <summary>
    /// sets the new default object
    /// </summary>
    /// <param name="selectedObj"></param>
    public void SetDefaultSelectedGameobject(GameObject selectedObj)
    {
        defaultSelectedButton = selectedObj;
    }

    /// <summary>
    /// selects the current default object
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SetEventSystemSelectedObjToDefault()
    {
        if (!InputManager.Instance.ControllerIsEnabled)
        {
            return;
        }

        if (EventSystem.current == null)
        {
            throw new System.Exception("No event system found");
        }

        //add to this if statement for every menu i have to make custom controller supp for
        if (UIManager.Instance.GetCurrentMenu() == UiMenuType.UpgradeMenu)
        {
            FindAnyObjectByType<UpgradeControllerSupportManager>().SelectLastSelectedInventoryPin();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }


}
