/*
* Author: Tyler
* Contributors:
* Last Modified:10/7/2026
* Summary: This service controls the controller's focus for the upgrade menu.
* To Do:   N/A
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeControllerSupportManager : MonoBehaviour
{
    private UpgradeMenuController menu;
    private ControllerSupportedClickable currentSelectedItem;

    private ControllerSupportedClickable prevSelectedInventorySlot;

    private ControllerSupportedClickable prevSelectedGridSlot;

    private ControllerSupportedClickable prevSelectedTarotCard;

    private bool isOnMoveCooldown;
    [SerializeField]
    private float moveCooldown = 0.1f;

    [SerializeField]
    private float startingMoveCooldown = 0.25f;

    [SerializeField]
    private List<ControllerSupportedClickable> testingList = new();

    private Coroutine playerMovingInMenu;
    private bool playerCanMove = false;
    private bool playerHasMovedOnce;

    private Vector2 currentMoveDirection;

    /// <summary>
    /// initializes this script
    /// </summary>
    public void InitSupportManager()
    {
        menu = GetComponent<UpgradeMenuController>();

        InputPublicEvents.ControllerEnabled += EnablePublicEvents;
        InputPublicEvents.KeyboardMouseEnabled += KBMEnabled;

        if (InputManager.Instance.ControllerIsEnabled)
        {
            EnablePublicEvents();
        }
    }


    /// <summary>
    /// triggers when the keyboard and mouse is enabled - temporarily shuts down the menu
    /// </summary>
    private void KBMEnabled()
    {
        DisablePublicEvents();

        PlayerStoppedMoving();

    }

    /// <summary>
    /// turns on the public events that are input specific
    /// </summary>
    private void EnablePublicEvents()
    {
        InputPublicEvents.SelectPin += PinSelected;
        InputPublicEvents.PlayerAimed += MoveSelectedObject;
        InputPublicEvents.AimCancelled += PlayerStoppedMoving;
    }

    /// <summary>
    /// turns on the public events that are input specific
    /// </summary>
    private void DisablePublicEvents()
    {
        InputPublicEvents.SelectPin -= PinSelected;
        InputPublicEvents.PlayerAimed -= MoveSelectedObject;
        InputPublicEvents.AimCancelled -= PlayerStoppedMoving;

    }

    /// <summary>
    /// Tears down public events
    /// </summary>
    private void OnDestroy()
    {
        DisablePublicEvents();
        InputPublicEvents.ControllerEnabled -= EnablePublicEvents;
        InputPublicEvents.KeyboardMouseEnabled -= DisablePublicEvents;
    }

    /// <summary>
    /// Selects the last selected inventory pin.
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    public void SelectLastSelectedInventoryPin()
    {
        //if youve selected an inventory slot, selects that one
        if (prevSelectedInventorySlot != null)
        {
            SelectInventoryItem(prevSelectedInventorySlot);
            return;
        }

        InventoryPinHolder defaultInventoryPin = menu.inventorySlots[0];

        if (defaultInventoryPin == null)
        {
            throw new System.Exception("Inventory is null");
        }

        SelectInventoryItem(defaultInventoryPin);
    }

    /// <summary>
    /// deselects the object the player is holding
    /// </summary>
    public void DeselectCurrentObject()
    {
        if (currentSelectedItem != null)
        {
            currentSelectedItem.UnHoveredOver();
            currentSelectedItem = null;
        }

    }

    /// <summary>
    /// selects an inventory item
    /// </summary>
    /// <param name="inventoryItem"></param>
    public void SelectInventoryItem(ControllerSupportedClickable inventoryItem)
    {
        currentSelectedItem = inventoryItem;
        prevSelectedInventorySlot = inventoryItem;
        currentSelectedItem.HoveredOver();
    }

    /// <summary>
    /// clicks on a pin
    /// currently disabled
    /// </summary>
    private void PinSelected()
    {
        /*if (currentSelectedItem != null)
        {
            currentSelectedItem.ClickedOn();
        }*/
    }

    /// <summary>
    /// attempts to move the held object based on the direction
    /// </summary>
    /// <param name="vecDirection"></param>
    private void MoveSelectedObject(Vector2 vecDirection)
    {
        currentMoveDirection = vecDirection;

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
            testingList = currentSelectedItem.getNeighbors();

            //grab where we are looking to move and convert it to an int
            Vector2Int dir = new Vector2Int(Mathf.RoundToInt(currentMoveDirection.x), Mathf.RoundToInt(currentMoveDirection.y));
            int indexedDirection = UtilityFunctions.ConvertVecIntToIntDirection(dir);

            //if we have a place to move to, move
            if (testingList[indexedDirection] != null)
            {
                DeselectCurrentObject();

                if (testingList[indexedDirection] is InventoryPinHolder invSlot)
                {
                    SelectInventoryItem(invSlot);
                    CheckIfInventoryScrollNeedsUpdating();
                }

            }

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
        float target = playerHasMovedOnce ? moveCooldown : startingMoveCooldown;
        playerHasMovedOnce = true;
        while (timer < target)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        isOnMoveCooldown = false;

    }

    /// <summary>
    /// updates the inventory scroll rect based on where the selected object is
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private void CheckIfInventoryScrollNeedsUpdating()
    {
        RectTransform scrollRect = menu.inventoryScrollRect.GetComponent<RectTransform>();
        if (scrollRect == null)
        {
            throw new System.Exception("Scroll rect doesnt have a rect transform");
        }

        RectTransform selRect = currentSelectedItem.GetComponent<RectTransform>();
        RectTransform contentRect = menu.inventory.GetComponent<RectTransform>();

        float SelYPos = Mathf.Abs(selRect.anchoredPosition.y) + selRect.rect.height;
        float scrollViewMinY = contentRect.anchoredPosition.y;
        float scrollViewMaxY = contentRect.anchoredPosition.y + scrollRect.rect.height;

        //move scroll rect to fit with the selected object. 
        if (SelYPos > scrollViewMaxY)
        {
            float newY = SelYPos - scrollRect.rect.height;
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, newY);
        }
        else if (Mathf.Abs(selRect.anchoredPosition.y) < scrollViewMinY)
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, Mathf.Abs(selRect.anchoredPosition.y) - 100);
        }
    }
}
