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



    private void KBMEnabled()
    {
        DisablePublicEvents();

        PlayerStoppedMoving();

    }

    private void EnablePublicEvents()
    {
        InputPublicEvents.SelectPin += PinSelected;
        InputPublicEvents.PlayerAimed += MoveSelectedObject;
        InputPublicEvents.AimCancelled += PlayerStoppedMoving;
    }

    private void DisablePublicEvents()
    {
        InputPublicEvents.SelectPin -= PinSelected;
        InputPublicEvents.PlayerAimed -= MoveSelectedObject;
        InputPublicEvents.AimCancelled -= PlayerStoppedMoving;

    }

    private void OnDestroy()
    {
        DisablePublicEvents();
        InputPublicEvents.ControllerEnabled += EnablePublicEvents;
        InputPublicEvents.KeyboardMouseEnabled += DisablePublicEvents;
    }

    public void SelectDefaultInventoryPin()
    {
        InventoryPinHolder defaultInventoryPin = menu.inventorySlots[0];

        if (defaultInventoryPin == null)
        {
            throw new System.Exception("Inventory is null");
        }

        SelectInventoryItem(defaultInventoryPin);
    }

    public void DeselectCurrentObject()
    {
        if (currentSelectedItem != null)
        {
            currentSelectedItem.UnHoveredOver();
            currentSelectedItem = null;
        }

    }

    public void SelectInventoryItem(ControllerSupportedClickable inventoryItem)
    {
        currentSelectedItem = inventoryItem;
        prevSelectedInventorySlot = inventoryItem;
        currentSelectedItem.HoveredOver();
    }

    private void PinSelected()
    {
        if (currentSelectedItem != null)
        {
            currentSelectedItem.ClickedOn();
        }
    }

    private void MoveSelectedObject(Vector2 vecDirection)
    {
        currentMoveDirection = vecDirection;
        if (playerMovingInMenu == null)
        {
            playerMovingInMenu = StartCoroutine(PlayerIsMoving());
            playerCanMove = true;
        }
    }

    private void PlayerStoppedMoving()
    {
        if (playerMovingInMenu != null)
        {
            playerCanMove = false;
            playerMovingInMenu = null;
            StopAllCoroutines();
            playerHasMovedOnce = false;
            isOnMoveCooldown = false;
        }
    }

    private IEnumerator PlayerIsMoving()
    {
        while (playerCanMove)
        {
            if (isOnMoveCooldown)
            {
                yield return null;
                continue;
            }
            isOnMoveCooldown = true;
            StartCoroutine(StartMoveCooldown());
            testingList = currentSelectedItem.getNeighbors();

            Vector2Int dir = new Vector2Int(Mathf.RoundToInt(currentMoveDirection.x), Mathf.RoundToInt(currentMoveDirection.y));


            int indexedDirection = UtilityFunctions.ConvertVecIntToIntDirection(dir);

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

    private IEnumerator StartMoveCooldown()
    {
        float timer = 0f;
        float target = playerHasMovedOnce ? moveCooldown : startingMoveCooldown;
        playerHasMovedOnce = true;
        while (timer < target)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        isOnMoveCooldown = false;

    }

    private void CheckIfInventoryScrollNeedsUpdating()
    {
        RectTransform scrollRect = menu.inventoryScrollRect.GetComponent<RectTransform>();
        if (scrollRect == null)
        {
            throw new System.Exception("Scroll rect doesnt have a recttransform");
        }

        RectTransform selRect = currentSelectedItem.GetComponent<RectTransform>();
        RectTransform contentRect = menu.inventory.GetComponent<RectTransform>();

        float SelYPos = Mathf.Abs(selRect.anchoredPosition.y) + selRect.rect.height;
        float scrollViewMinY = contentRect.anchoredPosition.y;
        float scrollViewMaxY = contentRect.anchoredPosition.y + scrollRect.rect.height;

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
