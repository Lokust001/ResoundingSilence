/*
* Author: Tyler
* Contributors:
* Last Modified: 10/17/2026
* Summary: This is the base of all the items that can be picked up and placed in custom ui menus
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class CarriedItemBehavior : ControllerSupportedClickable
{
    [HideInInspector]
    public ItemHolderSlot Parent;

    [HideInInspector]
    public ItemHolderSlot Owner;

    [SerializeField]
    private Image Highlight;
    private CanvasGroup raycastBlocker;
    protected Image pinSprite;

    public bool CanBePickedUp = true;

    /// <summary>
    /// sets a references
    /// </summary>
    private void Awake()
    {
        raycastBlocker = GetComponent<CanvasGroup>();
        pinSprite = GetComponent<Image>();
    }

    /// <summary>
    /// initializes the pin
    /// </summary>
    /// <param name="parent">if the pin is attached to a tile</param>
    /// <exception cref="System.Exception">Tried to make a pin that has no pindata</exception>
    public virtual void InitItem(ItemHolderSlot owner)
    {
        Parent = owner;
        Owner = owner;
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;
        CanBePickedUp = true;
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
        if (CanBePickedUp)
        {
            UIPublicEvents.ItemPickedUp?.Invoke(this);
        }
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

            UIPublicEvents.UpdateCarriedItemPosition += Teleport;
            ControllerUIService.Instance.MoveCarriedItem();
        }
    }

    /// <summary>
    /// reenables all of the raycasting and turns off the movement
    /// </summary>
    public void StopMoving()
    {
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;
        UnHoveredOver();
        InputPublicEvents.MouseMoved -= Teleport;
        UIPublicEvents.UpdateCarriedItemPosition -= Teleport;
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
