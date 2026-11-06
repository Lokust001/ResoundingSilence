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


    public override void InitMenu()
    {
        base.InitMenu();
        firstWeaponSelect = true;
        defaultWeaponSelectPos = weaponSelectionSlider.transform.position;
    }

    protected override void MenuOpenedSucessfully()
    {
        base.MenuOpenedSucessfully();
       
        InputPublicEvents.MovePressed += MoveFirstFocus;

        for (int i = 0; i < MidRunDataManager.Instance.equippedWeapons.Count; i++)
        {
            if (MidRunDataManager.Instance.equippedWeapons[i] != null)
            {
                weaponSelectButtons[i].GetComponentInChildren<TMP_Text>().text = MidRunDataManager.Instance.equippedWeapons[i].WeaponName;
            }
        }
    }

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

    public void MoveWeaponSelectionSlider(int index)
    {
        if (!weaponSelectionSlider.gameObject.activeInHierarchy)
        {
            return;
        }

        weaponSelectionSlider.transform.position = new Vector2( weaponSelectButtons[index].transform.position.x, defaultWeaponSelectPos.y);
    }

    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        if (firstWeaponSelect)
        {
            firstWeaponSelect = false;
            weaponSelectionSlider.gameObject.SetActive(true);
        }
        InputPublicEvents.MovePressed -= MoveFirstFocus;
        weaponSelectionSlider.transform.position = defaultWeaponSelectPos;
    }
}
