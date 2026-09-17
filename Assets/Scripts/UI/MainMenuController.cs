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
    /// <summary>
    /// starts the game
    /// </summary>
    public void StartGame()
    {
        GenericPublicEvents.StartGamePressed?.Invoke();
    }

    /// <summary>
    /// quits the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    /// <summary>
    /// opens/closes the settings menu
    /// 
    /// DOES NOT WORK RN
    /// </summary>
    public void ToggleSettings()
    {
        Debug.Log("Toggling Settings");
    }

    /// <summary>
    /// opens/closes the credits menu
    /// 
    /// 
    /// DOES NOT WORK RN
    /// </summary>
    public void ToggleCredits()
    {
        Debug.Log("Toggling Credits");
    }
}
