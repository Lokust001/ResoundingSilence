/*
* Author: Tyler
* Contributors:
* Last Modified: 09/17/2026
* Summary: Contains all of the button functions for the main menu.
* To Do:   N/A
*/

using UnityEngine;

public class MainMenuController : MenuBase
{
    public void StartGame()
    {
        GenericPublicEvents.StartGamePressed?.Invoke();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void ToggleSettings()
    {
        Debug.Log("Toggling Settings");
    }

    public void ToggleCredits()
    {
        Debug.Log("Toggling Credits");
    }
}
