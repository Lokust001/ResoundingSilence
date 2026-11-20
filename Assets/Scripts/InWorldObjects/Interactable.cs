/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/6/2026
* Summary: Allows an object to be interacted with via a UnityEvent
*/
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent interactionAction;
    private bool inRange = false;
    /// <summary>
    /// Adds the InteractPressed Action to this script's Interact method
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += Interact;
    }
    /// <summary>
    /// Removes the InteractPressed Action to this script's Interact method
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= Interact;
    }
    /// <summary>
    /// Interacts with the interactable if they are in range
    /// </summary>
    private void Interact()
    {
        if (inRange && UIManager.Instance.GetCurrentMenu() == UiMenuType.PlayerHUD)
        {
            Debug.Log("Interact");
            interactionAction?.Invoke();
        }
    }

    /// <summary>
    /// Sets the inRange flag to true when player is in the trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            inRange = true;
    }

    /// <summary>
    /// Sets the inRange flag to false when player leaves the trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            inRange = false;
    }


}
