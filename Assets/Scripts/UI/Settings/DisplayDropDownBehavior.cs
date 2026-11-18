/*
* Author: Tyler
* Contributors:
* Last Modified:11/18/2026
* Summary: controls all the dropdowns for the display settings
* To Do:   N/A
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DisplayDropDownBehavior : MonoBehaviour
{
    public enum DropdownType
    {
        Fullscreen,
        Resolution,
        TargetDisplay
    }

    [SerializeField]
    private DropdownType thisDropdownType;

    [SerializeField] private TMP_Dropdown dropdown;

    private List<Resolution> filteredResolutions;
    private Resolution[] allResolutions;

    private List<FullScreenMode> fullscreenOptions = new();

    RefreshRate currentRefreshRate = new();
    int currentResIndex = 0;

    private List<DisplayInfo> displays = new();
    private int currentDisplayIndex;

    /// <summary>
    /// sets options when it opens
    /// </summary>
    private void OnEnable()
    {
        switch (thisDropdownType)
        {
            case DropdownType.Fullscreen:
                PopulateFullscreenDropdown();
                break;
            case DropdownType.Resolution:
                UpdateResolutionList();
                UIPublicEvents.TargetDisplayChanged += UpdateResolutionList;
                break;
            case DropdownType.TargetDisplay:
                UpdateTargetDisplayOptions();
                break;
            default:
                break;
        }

    }


    /// <summary>
    /// Called when the player selects a new dropdown option
    /// </summary>
    /// <param name="index"></param>
    public void DropdownOptionSelected(int index)
    {
        switch (thisDropdownType)
        {
            case DropdownType.Fullscreen:
                ChangeFullscreenMode(index);
                break;
            case DropdownType.Resolution:
                ChangeResolution(index);
                break;
            case DropdownType.TargetDisplay:
                _ = DisplayOptionChosen(index);
                break;
            default:
                break;
        }
    }

    #region Resolution

    /// <summary>
    /// updates the dropdown list with resolutions
    /// </summary>
    private void UpdateResolutionList()
    {
        if (thisDropdownType != DropdownType.Resolution)
        {
            return;
        }

        allResolutions = Screen.resolutions;
        filteredResolutions = new();

        currentRefreshRate = Screen.currentResolution.refreshRateRatio;
        dropdown.ClearOptions();

        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].refreshRateRatio == currentRefreshRate)
            {
                filteredResolutions.Add(allResolutions[i]);
            }
        }

        List<string> dropdownOptions = new();

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            Resolution current = filteredResolutions[i];
            string retStr = $"{current.width}x{current.height} {current.refreshRateRatio.value} Hz";

            dropdownOptions.Add(retStr);

            if (current.width == Screen.width && current.height == Screen.height)
            {
                currentResIndex = i;
            }
        }

        dropdown.AddOptions(dropdownOptions);
        dropdown.value = currentResIndex;
        dropdown.RefreshShownValue();
    }

    /// <summary>
    /// changes the current resolution
    /// </summary>
    /// <param name="indexOfResolution"></param>
    private void ChangeResolution(int indexOfResolution)
    {
        Resolution target = filteredResolutions[indexOfResolution];
        Screen.SetResolution(target.width, target.height, Screen.fullScreen);
    }

    #endregion

    #region Fullscreen

    /// <summary>
    /// updates the dropdown list with fullscreen options
    /// </summary>
    private void PopulateFullscreenDropdown()
    {
        List<string> dropdownOptions = new();
        dropdown.ClearOptions();
        fullscreenOptions.Clear();

        dropdownOptions.Add("Fullscreen");
        fullscreenOptions.Add(FullScreenMode.ExclusiveFullScreen);
        dropdownOptions.Add("Borderless Windowed");
        fullscreenOptions.Add(FullScreenMode.FullScreenWindow);
        dropdownOptions.Add("Windowed");
        fullscreenOptions.Add(FullScreenMode.Windowed);

        dropdown.AddOptions(dropdownOptions);
        dropdown.value = (int)Screen.fullScreenMode;
    }

    /// <summary>
    /// changes the fullscreen mode
    /// </summary>
    /// <param name="index"></param>
    private void ChangeFullscreenMode(int index)
    {
        Screen.fullScreenMode = fullscreenOptions[index];
        UIPublicEvents.FullscreenChanged?.Invoke();
    }

    #endregion

    #region TargetDisplay

    /// <summary>
    /// updates the dropdown options with the displays
    /// </summary>
    private void UpdateTargetDisplayOptions()
    {
        Screen.GetDisplayLayout(displays);
        dropdown.ClearOptions();
        List<string> dropOptions = new();
        for (int i = 0; i < displays.Count; i++)
        {
            dropOptions.Add(displays[i].name);
            if (displays[i].Equals(Screen.mainWindowDisplayInfo))
            {
                currentDisplayIndex = i;
            }
        }

        dropdown.AddOptions(dropOptions);
        dropdown.value = currentDisplayIndex;
    }

    /// <summary>
    /// sets the current display
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private async Awaitable DisplayOptionChosen(int index)
    {
        DisplayInfo chosenDisp = displays[index];

        List<DisplayInfo> currentDisplays = new();
        Screen.GetDisplayLayout(currentDisplays);

        if (!currentDisplays.Contains(chosenDisp))
        {
            return;
        }

        int currentMode = ((int)Screen.fullScreenMode);

        //ANGERR
        Screen.fullScreenMode = FullScreenMode.Windowed;
        while (Screen.fullScreenMode != FullScreenMode.Windowed)
        {
            await Task.Yield();
        }

        //ANGER
        AsyncOperation operation = Screen.MoveMainWindowTo(currentDisplays[index], Vector2Int.zero);
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        //FUCK UNITY
        Screen.fullScreenMode = (FullScreenMode)currentMode;
        while (Screen.fullScreenMode != (FullScreenMode)currentMode)
        {
            await Task.Yield();
        }

        UIPublicEvents.TargetDisplayChanged?.Invoke();

        await Task.CompletedTask;
    }

    #endregion
}