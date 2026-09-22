using NaughtyAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MidRunDataManager : BaseManager
{
    public static MidRunDataManager Instance;

    

    public List<PinScriptable> pinInventory { get; private set; } = new();

    [SerializeField]
    private bool EnableTestingMode;

    [SerializeField, ShowIf(nameof(EnableTestingMode))]
    private List<PinScriptable> testingPinInventory = new();

    public override async Awaitable InitManager()
    {
        await base.InitManager();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        pinInventory.Clear();
        if (EnableTestingMode && testingPinInventory.Count > 0)
        {
            foreach (PinScriptable pin in testingPinInventory)
            {
                pinInventory.Add(pin.CreateNonRefCopy<PinScriptable>());
            }
        }
    }

    public PinScriptable RemovePinFromInventory(int id)
    {
        if (id < 0 || id >= pinInventory.Count)
        {
            Debug.Log("Tried to remove a pin at an invalid id");
            return null;
            
        }

        PinScriptable tempPin = pinInventory[id];
        pinInventory.RemoveAt(id);
        return tempPin;
    }

    public PinScriptable RemovePinFromInventory(PinScriptable pin)
    {
        if (!pinInventory.Contains(pin))
        {
            Debug.Log("Tried to remove a pin from the inventory that doesnt exist");
            return null;
        }

        return RemovePinFromInventory(pinInventory.IndexOf(pin));
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pin"></param>
    /// <returns>the id of the pin it just added</returns>
    public int AddPinToInventory(PinScriptable pin)
    {
        pinInventory.Add(pin);
        return pinInventory.Count - 1;
    }
}
