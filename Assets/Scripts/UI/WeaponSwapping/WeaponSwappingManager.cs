/*
* Author: Tyler
* Contributors:
* Last Modified: 11/5/2026
* Summary: Controls the UI menu for weapon swapping
* To Do:   N/A
*/

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponSwappingManager : MenuBase
{
    [SerializeField]
    private Image weaponSelectionSlider;

    [SerializeField]
    private List<Button> weaponSelectButtons;

    private bool firstWeaponSelect = true;
    private Vector2 defaultWeaponSelectPos;

    /// <summary>
    /// initializes the menu
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        firstWeaponSelect = true;
        defaultWeaponSelectPos = weaponSelectionSlider.transform.position;
    }

    /// <summary>
    /// sets the text on the buttons when the menu opens
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        base.MenuOpenedSucessfully();
       
        InputPublicEvents.MovePressed += MoveFirstFocus;
        InputPublicEvents.InteractPressed += CloseMenu;
        EventSystem.current.SetSelectedGameObject(null);
        for (int i = 0; i < MidRunDataManager.Instance.equippedWeapons.Count; i++)
        {
            if (MidRunDataManager.Instance.equippedWeapons[i] != null)
            {
                weaponSelectButtons[i].GetComponentInChildren<TMP_Text>().text = MidRunDataManager.Instance.equippedWeapons[i].WeaponName;
            }
        }
    }

    /// <summary>
    /// waits for players input then sets it properly
    /// </summary>
    /// <param name="aim"></param>
    private void MoveFirstFocus(Vector2 aim)
    {
        int aimDir = Mathf.RoundToInt(aim.x);

        if (Mathf.Abs(aimDir) >= 1)
        {
            InputPublicEvents.MovePressed -= MoveFirstFocus;

            EventSystem.current.SetSelectedGameObject(
                weaponSelectButtons[aimDir < 0 ? 0 : weaponSelectButtons.Count - 1].gameObject);

        }
    }

    /// <summary>
    /// moves the slider of the new weapon
    /// </summary>
    /// <param name="index"></param>
    public void MoveWeaponSelectionSlider(int index)
    {
        if (!weaponSelectionSlider.gameObject.activeInHierarchy)
        {
            return;
        }

        weaponSelectionSlider.transform.position = new Vector2( weaponSelectButtons[index].transform.position.x, defaultWeaponSelectPos.y);
    }

    /// <summary>
    /// closes the menu
    /// </summary>
    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        if (firstWeaponSelect)
        {
            firstWeaponSelect = false;
            weaponSelectionSlider.gameObject.SetActive(true);
        }
        MidRunDataManager.Instance.weaponSwappingContainer = null;
        InputPublicEvents.MovePressed -= MoveFirstFocus;
        InputPublicEvents.InteractPressed -= CloseMenu;
        weaponSelectionSlider.transform.position = defaultWeaponSelectPos;
    }

    public void SelectWeapon(int i)
    {
        MidRunDataManager.Instance.weaponSwappingContainer.ReplaceDroppedWeapon(i);
        CloseMenu();
    }

    private void CloseMenu()
    {
        UIPublicEvents.CurrentMenuClosed?.Invoke();
    }
}
