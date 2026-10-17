/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: The behavior for the picking up and putting down of pins
* To Do:   N/A
*/

using UnityEngine;
using UnityEngine.UI;

public class PinItemBehavior : CarriedItemBehavior
{
    public PinScriptable pinData;


    /// <summary>
    /// initializes the pin
    /// </summary>
    /// <param name="pin">the data for the pin</param>
    /// <param name="parent">if the pin is attached to a tile</param>
    /// <exception cref="System.Exception">Tried to make a pin that has no pindata</exception>
    public void InitSetPinData(PinScriptable pin)
    {
        if (pin == null)
        {
            throw new System.Exception("Tried to make a pin that has no pindata");
        }

        pinData = pin;

        if (pinSprite == null)
        {
            pinSprite = GetComponent<Image>();
        }
        pinSprite.color = pinData.pinColor;
    }
}
