/*
* Author: Tyler
* Contributors:
* Last Modified:10/7/2026
* Summary: This service controls the pause menu
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuController : MenuBase
{
    [SerializeField]
    private GameObject QuitConfirmPopup;

    [SerializeField]
    private Button QuitConfirmCancelButton;

    /// <summary>
    /// triggers when the menu opens sucessfully
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        QuitConfirmPopup.SetActive(false);
        base.MenuOpenedSucessfully();

        //TODO: stop time. Idk if we want to do time.timescale = 0 cus that might fuck some stuff up
    }

    /// <summary>
    /// Closes back the pause menu
    /// </summary>
    public void Resume()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    /// <summary>
    /// Enables the settings menu
    /// </summary>
    public void ToggleSettings()
    {
        UIPublicEvents.UpdateUIManagerStack(UiMenuType.Settings);
    }

    /// <summary>
    /// opens up the quit confirmation screen
    /// </summary>
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

    /// <summary>
    /// Quits the app
    /// </summary>
    public void ConfirmQuit()
    {
        GenericPublicEvents.ReturnToMainMenu?.Invoke();
    }
}
