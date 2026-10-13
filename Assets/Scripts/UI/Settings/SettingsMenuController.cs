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

    protected override void MenuOpenedSucessfully()
    {
        UIPublicEvents.SettingsMenuOpened?.Invoke();
        SwitchTab(0);
    }

    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        InputPublicEvents.SettingsMoveTabLeft += MoveTabLeft;
        InputPublicEvents.SettingsMoveTabRight += MoveTabRight;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        InputPublicEvents.SettingsMoveTabLeft -= MoveTabLeft;
        InputPublicEvents.SettingsMoveTabRight -= MoveTabRight;
    }

    protected override void CloseMenu()
    {
        UIPublicEvents.SettingsMenuClosed?.Invoke();

        base.CloseMenu();

    }

    public void TriggerCloseMenu()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }

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
