using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerUIService : BaseService
{
    public static ControllerUIService Instance;

    private GameObject defaultSelectedButton;

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

    private async Awaitable SetupPublicEvents()
    {
        InputPublicEvents.ControllerEnabled += SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled += DeselectUI;
        await Task.CompletedTask;
    }

    private void OnDestroy()
    {
        InputPublicEvents.ControllerEnabled -= SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled -= DeselectUI;
    }

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

    public void SetDefaultSelectedGameobject(GameObject selectedObj)
    {
        defaultSelectedButton = selectedObj;
    }

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
            FindAnyObjectByType<UpgradeControllerSupportManager>().SelectDefaultInventoryPin();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    
}
