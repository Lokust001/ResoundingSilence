using TMPro;
using System.Collections.Generic;
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

    private void Awake()
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
                break;
            default:
                break;
        }

    }



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
                break;
            default:
                break;
        }
    }

    #region Resolution

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

    private void ChangeResolution(int indexOfResolution)
    {
        Resolution target = filteredResolutions[indexOfResolution];
        Screen.SetResolution(target.width, target.height, Screen.fullScreen);
    }

    #endregion

    #region Fullscreen

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

    private void ChangeFullscreenMode(int index)
    {
        Screen.fullScreenMode = fullscreenOptions[index];
        UIPublicEvents.FullscreenChanged?.Invoke();
    }

    #endregion
}