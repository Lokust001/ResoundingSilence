using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent interactionAction;
    private bool inRange = false;

    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += Interact;
    }

    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= Interact;
    }

    private void Interact()
    {
        if (inRange) 
        {
            Debug.Log("Interact");
            interactionAction?.Invoke();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>())
            inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
            inRange = false;
    }


}
