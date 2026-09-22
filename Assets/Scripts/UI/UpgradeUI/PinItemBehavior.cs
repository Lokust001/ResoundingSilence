using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinItemBehavior : MonoBehaviour, IPointerClickHandler
{
    private CanvasGroup raycastBlocker;
    private Image pinSprite;

    public PinScriptable pinData;

    public UpgradeTileBehavior Parent;

    private Coroutine moveCo;

    private void Awake()
    {
        raycastBlocker = GetComponent<CanvasGroup>();
        pinSprite = GetComponent<Image>();
    }

    public void InitPin(PinScriptable pin, UpgradeTileBehavior parent)
    {
        if (pin == null)
        {
            throw new System.Exception("Tried to make a pin that has no pindata");
        }

        pinData = pin;
        Parent = parent;
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;

        //replace with sprite
        pinSprite.color = pinData.pinColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Pin Clicked");
            UIPublicEvents.PinPickedUp?.Invoke(this);
        } 
    }

    public void PinPickedUp()
    {
        raycastBlocker.blocksRaycasts = false; 
        pinSprite.raycastTarget = false;
        moveCo = StartCoroutine(StartMoveCoroutine());
    }

    public void PinPlaced()
    {
        raycastBlocker.blocksRaycasts = true;
        pinSprite.raycastTarget = true;
        StopCoroutine(moveCo);
    }

    private IEnumerator StartMoveCoroutine()
    {
        while (true)
        {
            //replace with moving towards the mouse position
            transform.position = Vector3.zero;
            yield return null;
        }
    }
}
