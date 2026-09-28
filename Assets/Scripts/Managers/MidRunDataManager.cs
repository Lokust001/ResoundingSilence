/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: This is a database that will hold all of the data needed mid run.
* To Do:   N/A
*/

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

    /// <summary>
    /// initializes the manager
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// removes a specific pin from the inventory at the provided id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>the pin that got removed</returns>
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

    /// <summary>
    /// removes a pin from the inventory given the specific pin to remove
    /// </summary>
    /// <param name="pin"></param>
    /// <returns>the pin that got removed</returns>
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
    /// adds a pin to the inventory
    /// </summary>
    /// <param name="pin"></param>
    /// <returns>the id of the pin it just added</returns>
    public int AddPinToInventory(PinScriptable pin)
    {
        pinInventory.Add(pin);
        return pinInventory.Count - 1;
    }
}
