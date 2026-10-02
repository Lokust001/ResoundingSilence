using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TooltipUIService : BaseService
{
    public static TooltipUIService Instance;

    private TMP_Text currentToolTipTextObject;

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

    private async Awaitable SetUpPublicEvents()
    {
        UIPublicEvents.SetNewTooltipTextObject += SetNewTooltipTextObject;
        await Task.CompletedTask;
    }

    private void OnDestroy()
    {
        UIPublicEvents.SetNewTooltipTextObject -= SetNewTooltipTextObject;
    }

    private void SetNewTooltipTextObject(TMP_Text text)
    {
        currentToolTipTextObject = text;
    }

    public void ClearTooltip()
    {
        currentToolTipTextObject.text = "";
    }

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

    

    private string RequestPinAutoGenTooltip(PinScriptable pin)
    {
        string outputStr = $"";

        outputStr += $"Gain {pin.ModifierNumber * 100}% {pin.StatToChange}.";

        return outputStr;
    }
}
