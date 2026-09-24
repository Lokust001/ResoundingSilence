/*
* Author: Tyler
* Contributors:
* Last Modified: 09/23/2026
* Summary: This service controls the mouse's triggers on ui elements
* To Do:   N/A
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseUIService : BaseService
{
    public static MouseUIService Instance;

    private Clickable previouslyHoveredObj;

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

        await SetUpPublicEvents();
    }

    /// <summary>
    /// sets up all public events
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        InputPublicEvents.MouseMoved += UpdateMousePos;
        InputPublicEvents.ShootPressed += Click;

        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from all public events
    /// </summary>
    private void OnDestroy()
    {
        InputPublicEvents.MouseMoved -= UpdateMousePos;
        InputPublicEvents.ShootPressed -= Click;
    }

    /// <summary>
    /// calls hover/unhover on any clickable the mouse is hovering over
    /// </summary>
    /// <param name="NewMousePos"></param>
    private void UpdateMousePos(Vector2 NewMousePos)
    {
        //raycast and return the top clickable
        Clickable raycastResultClickable = null;
        PointerEventData tempEventData = new(EventSystem.current);
        tempEventData.position = NewMousePos;

        List<RaycastResult> raycastResults = new();

        EventSystem.current.RaycastAll(tempEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            if (raycastResults[0].gameObject.GetComponent<Clickable>() != null)
            {
                raycastResultClickable = raycastResults[0].gameObject.GetComponent<Clickable>();
            }
        }

        //unhover over the previously held clickable if its different from what youre hovering over now
        if (raycastResultClickable != previouslyHoveredObj)
        {
            if (previouslyHoveredObj != null)
            {
                previouslyHoveredObj.UnHoveredOver();
                previouslyHoveredObj = null;
            }

            //sets the next clickable
            if (raycastResultClickable != null)
            {
                previouslyHoveredObj = raycastResultClickable;
                previouslyHoveredObj.HoveredOver();
            }
        }

        
    }

    /// <summary>
    /// triggers the onclick of the object the mouse is hovering over
    /// </summary>
    private void Click()
    {
        //click on the object you have rn
        if (previouslyHoveredObj != null)
        {
            previouslyHoveredObj.ClickedOn();
        }
    }
}
