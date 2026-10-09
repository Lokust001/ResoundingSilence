/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: The behavior for the picking up and putting down of pins
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class PinItemBehavior : ControllerSupportedClickable
{
    [SerializeField]
    private Image Highlight;
    private CanvasGroup raycastBlocker;
    private Image pinSprite;

    public PinScriptable pinData;

    public PinHolderSlot Parent;

    public InventoryPinHolder Owner;

    /// <summary>
    /// setsa references
    /// </summary>
    private void Awake()
    {
        raycastBlocker = GetComponent<CanvasGroup>();
        pinSprite = GetComponent<Image>();
    }

    /// <summary>
    /// initializes the pin
    /// </summary>
    /// <param name="pin">the data for the pin</param>
    /// <param name="parent">if the pin is attached to a tile</param>
    /// <exception cref="System.Exception">Tried to make a pin that has no pindata</exception>
    public void InitPin(PinScriptable pin, InventoryPinHolder owner)
    {
        if (pin == null)
        {
            throw new System.Exception("Tried to make a pin that has no pindata");
        }

        pinData = pin;
        Parent = owner;
        Owner = owner;
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;

        //replace with sprite
        pinSprite.color = pinData.pinColor;
    }

    /// <summary>
    /// picks this up when its clicked on
    /// </summary>
    public override void ClickedOn()
    {
        base.ClickedOn();
        ForcePickUpPin();
    }

    /// <summary>
    /// picks up this pin
    /// </summary>
    public void ForcePickUpPin()
    {
        UIPublicEvents.PinPickedUp?.Invoke(this);
    }

    /// <summary>
    /// runs through all of the logic needed when the pin gets grabbed
    /// </summary>
    public void StartPinMoving()
    {
        raycastBlocker.blocksRaycasts = false;
        pinSprite.raycastTarget = false;
        UnHoveredOver();

        if (!InputManager.Instance.ControllerIsEnabled)
        {
            Teleport(InputManager.Instance.CurrentMousePosition);
            InputPublicEvents.MouseMoved += Teleport;
        }
        else
        {
            
            UIPublicEvents.UpdateCarriedPinPosition += Teleport;
            FindAnyObjectByType<UpgradeControllerSupportManager>().MoveCarriedPin();
        }
        

    }

    /// <summary>
    /// reenables all of the raycasting and turns off the movement
    /// </summary>
    public void StopPinMoving()
    {
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;
        UnHoveredOver();
        InputPublicEvents.MouseMoved -= Teleport;
        UIPublicEvents.UpdateCarriedPinPosition -= Teleport;
    }

    /// <summary>
    /// safety valve to disable the mouse movement
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.MouseMoved -= Teleport;
    }

    /// <summary>
    /// follows the mouse's position
    /// </summary>
    /// <param name="teleportCoords"></param>
    private void Teleport(Vector2 teleportCoords)
    {
        transform.position = teleportCoords;
    }

    /// <summary>
    /// requests a tooltip and highlights when hovered over.
    /// </summary>
    public override void HoveredOver()
    {
        base.HoveredOver();
        Highlight.enabled = true;
        TooltipUIService.Instance.RequestTooltip(pinData);
    }

    /// <summary>
    /// unhighlights when focus leaves this.
    /// </summary>
    public override void UnHoveredOver()
    {
        base.UnHoveredOver();
        Highlight.enabled = false;
    }
}
