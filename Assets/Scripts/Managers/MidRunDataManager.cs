/*
* Author: Tyler
* Contributors: Brenden, Brad Dixon
* Last Modified: 11/9/2026
* Summary: This is a database that will hold all of the data needed mid run.
* To Do:   N/A
*/

using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct LevelUpData
{
    public float ExpToLevelUp;
    public List<StatIncreaseTypes> LevelUpStats;
}

[System.Serializable]
public struct StatIncreaseTypes
{
    public enum Stats
    {
        Health
    }

    public Stats StatType;
    public float StatChange;
}


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

    private enum ValueChange
    {
        ExactValue,
        Multiply
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
    [SerializeField] ValueChange XpNeededIncrease;

    [ShowIf(nameof(settings), Settings.Exp)]
    [SerializeField] ValueChange LvlUpStatIncrease;

    [ShowIf(nameof(settings), Settings.Exp)]
    [SerializeField] private List<LevelUpData> playerLvls = new List<LevelUpData>();

    [ShowIf(nameof(settings), Settings.Exp)]
    [SerializeField] bool capLvl;

    private int playerLvl;

    [ShowIf(nameof(settings), Settings.Exp)]
    public float CurrentPlayerEXP;

    public float ExpNeededToLevelUp;

    [ShowIf(nameof(settings), Settings.Exp)]
    public int GiveXpAmount;

    [SerializeField]
    [ShowIf(nameof(settings), Settings.Pins)]
    public List<PinScriptable> PinScriptableObjects;


    public List<PinScriptable> pinInventory { get; private set; } = new();

    public List<BaseWeaponScriptable> equippedWeapons { get; private set; } = new(2);

    [HideInInspector]
    public weaponOnGround weaponSwappingContainer;

    [SerializeField]
    private bool EnableTestingMode;

    [SerializeField, ShowIf(nameof(EnableTestingMode))]
    private List<PinScriptable> testingPinInventory = new();

    #region TerotVariables
    bool MagicianBool;
    [SerializeField, Tooltip("Where the player will stop scaling, in %, base is 100")] int MagicianMaxDamageBuff;
    [SerializeField, Tooltip("Where the damage buff will be at after the player takes damage, in %, base is 100")] int MagicianMinDamageBuff;
    [SerializeField, Tooltip("How long between the ticks of buffs, in seconds")] float MagicianTimeBetween;
    [SerializeField, Tooltip("How much gets added each tick off time undamaged, in %, no scaling would be 0")] int MagicianAmountAdded;
    [HideInInspector] public int MagicianDamageBuff = 1;

    bool DevilBool;
    [SerializeField, Tooltip("Where the player will stop scaling, in %, base is 100")] int DevilMaxDamageBuff;
    [SerializeField, Tooltip("Where the damage buff will be at when hitting the min health missing threshold, in %, base is 100")] int DevilMinDamageBuff;
    [SerializeField, Tooltip("what percent health does the player have the max damage boost")] int DevilMaxBuffHealthPercent;
    [SerializeField, Tooltip("What percent health does the player have to start giving damage buff")] int DevilMinBuffHealthPercent;
    [HideInInspector] public int DevilDamageBuff = 1;
    #endregion

    [Button]
    void GiveXP()
    {
        GainXP(GiveXpAmount);
    }

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
        playerLvl = 1;
        ExpNeededToLevelUp = playerLvls[playerLvl - 1].ExpToLevelUp;

        UnequipAllWeapons();

        GenericPublicEvents.EquipWeaponInSlot += EquipWeaponInSlot;

        await Task.CompletedTask;
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
        if (equippedWeapons[slot] == null)
        {
            equippedWeapons[slot] = weapon;
        }
        else
        {
            Debug.Log($"Equipped weapon slot {slot} is swapping " +
                $"{equippedWeapons[slot].WeaponName} for {equippedWeapons[slot].WeaponName}");
        }

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
        if (amountToChange == 0)
        {
            return;
        }

        if (amountToChange < 0 && MagicianBool)
        {
            MagicianDamageBuff = MagicianMinDamageBuff;
        }

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

        if (CurrentPlayerHealth / MaxPlayerHealth < DevilMinBuffHealthPercent / 100)
        {
            if (CurrentPlayerHealth / MaxPlayerHealth < DevilMaxBuffHealthPercent / 100)
            {
                DevilDamageBuff = DevilMaxDamageBuff;
            }
            else
            {
                float differanceBetweenminandmaxbuff = DevilMaxDamageBuff - DevilMinDamageBuff;
                float differanceBetweenminandmaxhealth = DevilMaxBuffHealthPercent - DevilMinBuffHealthPercent;
                float CurrentPlayerHealthpercent = CurrentPlayerHealth / MaxPlayerHealth;

                float percentHealth = CurrentPlayerHealthpercent  - DevilMinBuffHealthPercent;
                float percentBuff = percentHealth / differanceBetweenminandmaxhealth;
                float buffamount = percentBuff * differanceBetweenminandmaxbuff;
                DevilDamageBuff = Mathf.CeilToInt(DevilMinDamageBuff + buffamount);
            }
        }
        else
        {
            DevilDamageBuff = 100;
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
        if (playerLvl >= playerLvls.Count && capLvl)
        {
            return;
        }

        CurrentPlayerEXP += xpAmount;

        if (CurrentPlayerEXP >= ExpNeededToLevelUp)
        {
            CurrentPlayerEXP -= ExpNeededToLevelUp;

            if (playerLvl < playerLvls.Count)
            {
                ++playerLvl;
            }

            ExpNeededToLevelUp = XpNeededIncrease == ValueChange.ExactValue ? playerLvls[playerLvl - 1].ExpToLevelUp : ExpNeededToLevelUp + Mathf.FloorToInt(ExpNeededToLevelUp * ConvertToPercentage(playerLvls[playerLvl - 1].ExpToLevelUp));
            //level up player. This will probably need to move to the health bar controller so
            //the player levels up when the bar reaches full as opposed to before

            foreach (StatIncreaseTypes i in playerLvls[playerLvl - 1].LevelUpStats)
            {
                //If lvl up should affect other stats, add to switch statement
                switch (i.StatType)
                {
                    case StatIncreaseTypes.Stats.Health:
                        float sIncrease = LvlUpStatIncrease == ValueChange.ExactValue ? i.StatChange : Mathf.CeilToInt(MaxPlayerHealth * ConvertToPercentage(i.StatChange));
                        MaxPlayerHealth += sIncrease;
                        CurrentPlayerHealth += sIncrease;
                        break;
                }
            }
        }

        UIPublicEvents.UpdatePlayerEXPSlider?.Invoke();
    }

    /// <summary>
    /// Converts the inspector variable to it's percentage value if it isn't already a percent
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected float ConvertToPercentage(float value)
    {
        return value >= 1 ? value / 100 : value;
    }

    private IEnumerator Magician()
    {
        while (MagicianBool)
        {
            MagicianDamageBuff += MagicianAmountAdded;
            if (MagicianDamageBuff > MagicianMaxDamageBuff)
            {
                MagicianDamageBuff = MagicianMaxDamageBuff;
            }
            yield return new WaitForSecondsRealtime(MagicianTimeBetween);
        }
        yield return null;
    }


}
