/*
* Author: Tyler
* Contributors:
* Last Modified:10/13/2026
* Summary: This service controls settings menu. may be broken up in the future depending on what the settings are
* To Do:   N/A
*/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsMenuController : MenuBase
{
    #region MenuRefs
    private enum shownSettings
    {
        None,
        MenuRefs,
        DisplayRefs
    }

    [SerializeField]
    private shownSettings inspectorSettings;

    [SerializeField]
    [ShowIf(nameof(inspectorSettings), shownSettings.MenuRefs)]
    private List<GameObject> settingsPages;

    private GameObject previousSettingsPage;


    [ShowIf(nameof(inspectorSettings), shownSettings.MenuRefs)]
    [SerializeField]
    private List<Button> topOfSettingsPages;

    private int currentlySelectedTab;
    #endregion

    #region MenuSettings

    /// <summary>
    /// returns all showings to default
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        UIPublicEvents.SettingsMenuOpened?.Invoke();
        SwitchTab(0);
    }


    /// <summary>
    /// subscribes to public events
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        InputPublicEvents.SettingsMoveTabLeft += MoveTabLeft;
        InputPublicEvents.SettingsMoveTabRight += MoveTabRight;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputPublicEvents.SettingsMoveTabLeft -= MoveTabLeft;
        InputPublicEvents.SettingsMoveTabRight -= MoveTabRight;
    }

    /// <summary>
    /// triggers when the menu closed
    /// </summary>
    protected override void CloseMenu()
    {
        UIPublicEvents.SettingsMenuClosed?.Invoke();

        base.CloseMenu();

    }

    /// <summary>
    /// Closes the menu
    /// </summary>
    public void TriggerCloseMenu()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

    /// <summary>
    /// Changes the currently showing settings tab
    /// </summary>
    /// <param name="i"></param>
    public void SwitchTab(int i)
    {
        if (i < 0 || i >= settingsPages.Count || i >= topOfSettingsPages.Count)
        {
            return;
        }

        settingsPages[i].SetActive(true);
        EventSystem.current.SetSelectedGameObject(topOfSettingsPages[i].gameObject);

        if (previousSettingsPage != null)
        {
            previousSettingsPage.SetActive(false);
        }
        
        previousSettingsPage = settingsPages[i];
        currentlySelectedTab = i;
    }
    
    /// <summary>
    /// Switches the current showing settings tab one to the left
    /// </summary>
    private void MoveTabLeft()
    {
        if (currentlySelectedTab <= 0)
        {
            SwitchTab(settingsPages.Count - 1);
        }
        else
        {
            SwitchTab(currentlySelectedTab - 1);
        }
    }

    /// <summary>
    /// switches the current showing tab one to the right
    /// </summary>
    private void MoveTabRight()
    {
        if (currentlySelectedTab >= settingsPages.Count)
        {
            SwitchTab(0);
        }
        else
        {
            SwitchTab(currentlySelectedTab + 1);
        }
    }

    #endregion
}
