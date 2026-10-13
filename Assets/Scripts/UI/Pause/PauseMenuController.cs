using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MenuBase
{
    [SerializeField]
    private GameObject QuitConfirmPopup;

    [SerializeField]
    private Button QuitConfirmCancelButton;

    protected override void MenuOpenedSucessfully()
    {
        QuitConfirmPopup.SetActive(false);
        base.MenuOpenedSucessfully();
        
    }

    public void Resume()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    public void ToggleSettings()
    {
        UIPublicEvents.UpdateUIManagerStack(UiMenuType.Settings);
    }

    public void ToggleQuit()
    {
        QuitConfirmPopup.SetActive(!QuitConfirmPopup.activeInHierarchy);

        if (QuitConfirmPopup.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(QuitConfirmCancelButton.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedGameobject);
        }
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }
}
