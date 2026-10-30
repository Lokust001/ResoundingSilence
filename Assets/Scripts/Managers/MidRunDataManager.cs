/*
* Author: Tyler
* Contributors: Brenden
* Last Modified: 10/21/2026
* Summary: This is a database that will hold all of the data needed mid run.
* To Do:   N/A
*/

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class MidRunDataManager : BaseManager
{
    private enum Settings
    {
        None,
        General,
        GeneralPlayer,
        Pins,
        Exp,
        Health,
        Dash
    }

    [SerializeField]
    private Settings settings;

    public static MidRunDataManager Instance;


    [ShowIf(nameof(settings), Settings.General)]
    public int Money;

    [SerializeField]
    private GameObject tempDeathMenu;

    [ShowIf(nameof(settings), Settings.Health)]
    public float CurrentPlayerHealth;

    [ShowIf(nameof(settings), Settings.Health)]
    public float MaxPlayerHealth;

    [ShowIf(nameof(settings), Settings.Health)]
    public float HealingPotency;

    [ShowIf(nameof(settings), Settings.Exp)]
    public float CurrentPlayerEXP;

    [ShowIf(nameof(settings), Settings.Exp)]
    public float ExpNeededToLevelUp;

    
    [SerializeField]
    [ShowIf(nameof(settings), Settings.Pins)]
    public List<PinScriptable> PinScriptableObjects;

    
    public List<PinScriptable> pinInventory { get; private set; } = new();

    public List<BaseWeaponScriptable> equippedWeapons { get; private set; } = new(2);

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

        CurrentPlayerHealth = MaxPlayerHealth;

        UnequipAllWeapons();
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
        UIPublicEvents.InventoryUpdated?.Invoke();
        return pinInventory.Count - 1;
    }

    /// <summary>
    /// Equips a given weapon in a specific slot
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="weapon"></param>
    public void EquipWeaponInSlot(int slot, BaseWeaponScriptable weapon)
    {
        if (slot < 0 || slot >= equippedWeapons.Count)
        {
            return;
        }

        equippedWeapons[slot] = weapon;
    }

    /// <summary>
    /// Unequips a given weapon in a specific slot
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public BaseWeaponScriptable UnequipWeapon(int slot)
    {
        if (slot < 0 || slot >= equippedWeapons.Count)
        {
            return null;
        }

        BaseWeaponScriptable temp = equippedWeapons[slot];
        equippedWeapons[slot] = null;
        return temp;
    }

    /// <summary>
    /// unequips every weapon
    /// </summary>
    public void UnequipAllWeapons()
    {
        equippedWeapons.Clear();
        equippedWeapons.Add(null);
        equippedWeapons.Add(null);
    }


    /// <summary>
    /// currently just picks a random PinScriptable to be set, can be swiched to add other types of randomness like legendary pins or if certain drops would be specifically a vitality pin, temp for the time being
    /// </summary>
    /// <returns></returns>
    public PinScriptable PickDroppedPin()
    {
        return PinScriptableObjects[Random.Range(0, PinScriptableObjects.Count)];
    }

    /// <summary>
    /// Used to heal the player while using the healing potency that you can get from buttons
    /// </summary>
    /// <param name="amountToChange"></param>
    public void ChangePlayerHealth(int amountToChange)
    {
        if (amountToChange > 0)
        {
            //ceil cus we are healing the player
            amountToChange = Mathf.CeilToInt(HealingPotency * amountToChange);
        }

        CurrentPlayerHealth += amountToChange;

        if (CurrentPlayerHealth <= 0)
        {
            InputManager.Instance.DisablePlayerInputs();
        }

        UIPublicEvents.UpdatePlayerHealthSlider?.Invoke();
    }

    /// <summary>
    /// temporary function
    /// </summary>
    public void KillPlayer()
    {
        Instantiate(tempDeathMenu, UIManager.Instance.GetMainCanvas().transform);
        
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Used to gain XP after defeating an enemy
    /// </summary>
    /// <param name="xpAmount"></param>
    public void GainXP(int xpAmount)
    {
        CurrentPlayerEXP += xpAmount;

        if(CurrentPlayerEXP >= ExpNeededToLevelUp)
        {
            CurrentPlayerEXP -= ExpNeededToLevelUp;
            //level up player. This will probably need to move to the health bar controller so
            //the player levels up when the bar reaches full as opposed to before
        }

        UIPublicEvents.UpdatePlayerEXPSlider?.Invoke();
    }
}
