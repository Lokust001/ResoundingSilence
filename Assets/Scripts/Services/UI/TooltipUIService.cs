/*
* Author: Tyler
* Contributors:
* Last Modified: 10/2/2026
* Summary: Manages *nearly* everything related to tooltips
* To Do:   add functionality for auto generated tooltips for more than just the pins
*/

using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TooltipUIService : BaseService
{
    public static TooltipUIService Instance;

    private TMP_Text currentToolTipTextObject;

    /// <summary>
    /// initializes all of the public events and sets up singleton
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
    /// inits public events
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        UIPublicEvents.SetNewTooltipTextObject += SetNewTooltipTextObject;
        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        UIPublicEvents.SetNewTooltipTextObject -= SetNewTooltipTextObject;
    }

    /// <summary>
    /// changes what text object has the tooltip.
    /// </summary>
    /// <param name="text"></param>
    private void SetNewTooltipTextObject(TMP_Text text)
    {
        currentToolTipTextObject = text;
    }

    /// <summary>
    /// deletes the current tooltip.
    /// </summary>
    public void ClearTooltip()
    {
        currentToolTipTextObject.text = "";
    }

    /// <summary>
    /// sets a new tooltip for a given scriptable object
    /// </summary>
    /// <param name="scriptableObject"></param>
    public void RequestTooltip(BaseScriptableObject scriptableObject)
    {
        string outputStr = $"";

        //request auto tooltip based on what derivative of scriptableobject it is
        if (scriptableObject is PinScriptable pin)
        {
            outputStr += $"{RequestPinAutoGenTooltip(pin)}";
        }


        outputStr += $"\n";
        if (scriptableObject.UsePresetTooltip)
        {
            outputStr += $"{scriptableObject.PresetTooltip}";
        }

        currentToolTipTextObject.text = outputStr;
    }

    
    /// <summary>
    /// auto generates the tooltip for the given pin.
    /// </summary>
    /// <param name="pin"></param>
    /// <returns></returns>
    private string RequestPinAutoGenTooltip(PinScriptable pin)
    {
        string outputStr = $"";

        outputStr += $"Gain {pin.ModifierNumber * 100}% {pin.StatToChange}.";

        return outputStr;
    }
}
