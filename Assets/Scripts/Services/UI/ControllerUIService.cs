/*
* Author: Tyler
* Contributors:
* Last Modified: 10/7/2026
* Summary: This service controls the controller's ability to switch between scenes.
* To Do:   N/A
*/
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerUIService : BaseService
{
    public static ControllerUIService Instance;

    private GameObject defaultSelectedButton;

    private MenuControllerSupportBase controllerSupportMenu;
    public ControllerSupportedClickable currentlySelected;

    private Vector2 currentMoveDir;

    private Coroutine playerMovingInMenu;

    private bool playerCanMove;

    private bool playerHasMovedOnce;

    private bool isOnMoveCooldown;

    private float moveCooldown = 0.1f;

    private float firstMoveCooldown = 0.25f;

    #region setup
    /// <summary>
    /// Initializes the service
    /// </summary>
    /// <returns></returns>
    public override async Awaitable InitService()
    {
        await base.InitService();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        await SetupPublicEvents();
    }

    /// <summary>
    /// Turns on the public events for turning on and off controller
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetupPublicEvents()
    {
        InputPublicEvents.ControllerEnabled += SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled += DeselectUI;
        UIPublicEvents.NewMenuOpened += NewMenuOpened;
        await Task.CompletedTask;
    }

    /// <summary>
    /// turns off the publicv events for turning on and off controller
    /// </summary>
    private void OnDestroy()
    {
        InputPublicEvents.ControllerEnabled -= SetEventSystemSelectedObjToDefault;
        InputPublicEvents.KeyboardMouseEnabled -= DeselectUI;
        UIPublicEvents.NewMenuOpened -= NewMenuOpened;
    }


    #endregion

    #region MenuOpened

    /// <summary>
    /// Checks to see if the right menus opened
    /// </summary>
    private void NewMenuOpened()
    {
        if (!IsMenuUsingCustomControls())
        {
            controllerSupportMenu = null;
            DisableMenuControls();
            return;
        }

        controllerSupportMenu = FindAnyObjectByType<MenuControllerSupportBase>();
        EnableMenuControls();
    }
    
    /// <summary>
    /// turns on the menu controls
    /// </summary>
    private void EnableMenuControls()
    {
        InputPublicEvents.SelectPin += ClickOnSelected;
        InputPublicEvents.PlayerAimed += MoveSelectedObject;
        InputPublicEvents.AimCancelled += PlayerStoppedMoving;
    }

    /// <summary>
    /// turns off the menu controls
    /// </summary>
    private void DisableMenuControls()
    {
        InputPublicEvents.SelectPin -= ClickOnSelected;
        InputPublicEvents.PlayerAimed -= MoveSelectedObject;
        InputPublicEvents.AimCancelled -= PlayerStoppedMoving;
    }

    #endregion

    #region evergreen funcs

    /// <summary>
    /// Turns off the currently selected ui
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void DeselectUI()
    {
        if (InputManager.Instance.ControllerIsEnabled)
        {
            return;
        }

        PlayerStoppedMoving();

        if (EventSystem.current == null)
        {
            throw new System.Exception("No event system found");
        }



        //add to this if statement for every menu i have to make custom controller supp for
        if (IsMenuUsingCustomControls())
        {
            DeselectCurrentObject();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }


    }

    /// <summary>
    /// sets the new default object
    /// </summary>
    /// <param name="selectedObj"></param>
    public void SetDefaultSelectedGameobject(GameObject selectedObj)
    {
        defaultSelectedButton = selectedObj;
    }

    /// <summary>
    /// selects the current default object
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SetEventSystemSelectedObjToDefault()
    {
        if (!InputManager.Instance.ControllerIsEnabled)
        {
            return;
        }

        if (EventSystem.current == null)
        {
            throw new System.Exception("No event system found");
        }

        //add to this if statement for every menu i have to make custom controller supp for
        if (IsMenuUsingCustomControls())
        {
            controllerSupportMenu.SelectDefault();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
        }
    }

    /// <summary>
    /// bool check to see if the menu is a special one
    /// </summary>
    /// <returns></returns>
    private bool IsMenuUsingCustomControls()
    {
        UiMenuType currentMenu = UIManager.Instance.GetCurrentMenu();

        if (currentMenu == UiMenuType.UpgradeMenu ||
            currentMenu == UiMenuType.Tarot)
        {
            return true;
        }

        return false;
    }
    #endregion

    #region Selection

    /// <summary>
    /// deselects the object the player is holding
    /// </summary>
    public void DeselectCurrentObject()
    {
        if (currentlySelected != null)
        {
            currentlySelected.UnHoveredOver();
            currentlySelected = null;
        }
    }

    /// <summary>
    /// Clicks on the selected item
    /// </summary>
    private void ClickOnSelected()
    {
        if (currentlySelected == null)
        {
            return;
        }

        if (controllerSupportMenu.carriedItem == null)
        {
            currentlySelected.ClickedOn();
            MoveCarriedItem();
        }
        else
        {
            InputPublicEvents.PinReleased?.Invoke();
        }
        
    }

    /// <summary>
    /// Selects a specific tile
    /// </summary>
    /// <param name="tile"></param>
    public void SelectSpecficTile(ControllerSupportedClickable tile)
    {
        if (currentlySelected != null)
        {
            currentlySelected.UnHoveredOver();
        }
        currentlySelected = tile;
        currentlySelected.HoveredOver();
        MoveCarriedItem();
    }

    /// <summary>
    /// move the currently carried pin to the top right of the selected object
    /// </summary>
    public void MoveCarriedItem()
    {
        //if this has a listener, throw the public event
        if (UIPublicEvents.UpdateCarriedItemPosition != null)
        {
            Vector3[] Corners = new Vector3[4];
            currentlySelected.GetComponent<RectTransform>().GetWorldCorners(Corners);
            //Rect currentSelRect = currentSelectedItem.GetComponent<RectTransform>().rect;

            UIPublicEvents.UpdateCarriedItemPosition?.Invoke(
                new Vector2(Corners[2].x,
                            Corners[2].y));
        }
    }

    #endregion

    #region Movement

    /// <summary>
    /// attempts to move the held object based on the direction
    /// </summary>
    /// <param name="vecDirection"></param>
    private void MoveSelectedObject(Vector2 vecDirection)
    {
        currentMoveDir = vecDirection;

        //if we aren't already moving, start moving
        if (playerMovingInMenu == null)
        {
            playerMovingInMenu = StartCoroutine(PlayerIsMoving());
            playerCanMove = true;
        }
    }

    /// <summary>
    /// stops the player's movement (if they already are moving)
    /// </summary>
    private void PlayerStoppedMoving()
    {
        if (playerMovingInMenu != null)
        {
            playerCanMove = false;
            playerMovingInMenu = null;

            //stopall ironically only stops the coroutines on this object
            StopAllCoroutines();
            playerHasMovedOnce = false;
            isOnMoveCooldown = false;
        }
    }

    /// <summary>
    /// the coroutine that controls the player's movement throughout the ui menu.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerIsMoving()
    {
        while (playerCanMove)
        {
            //if we are on cooldown, skip to the next frame
            if (isOnMoveCooldown)
            {
                yield return null;
                continue;
            }
            isOnMoveCooldown = true;
            StartCoroutine(StartMoveCooldown());

            //grab the current item's neighbors
            List<ControllerSupportedClickable> neighbors = currentlySelected.getNeighbors();

            //grab where we are looking to move and convert it to an int
            Vector2Int dir = new Vector2Int(Mathf.RoundToInt(currentMoveDir.x), Mathf.RoundToInt(currentMoveDir.y));
            int indexedDirection = UtilityFunctions.ConvertVecIntToIntDirection(dir);

            controllerSupportMenu.MoveSelectedItem(neighbors[indexedDirection], indexedDirection);

            MoveCarriedItem();

            yield return null;
        }
    }

    /// <summary>
    /// the cooldown for moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartMoveCooldown()
    {
        float timer = 0f;

        //if it's the first time the player has moved this cycle, cool down for longer
        float target = playerHasMovedOnce ? moveCooldown : firstMoveCooldown;
        playerHasMovedOnce = true;
        while (timer < target)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        isOnMoveCooldown = false;

    }

    #endregion
}


