/*
* Author: Tyler
* Contributors: Brenden
* Last Modified: 11/20/2026
* Summary: This service stores all the player's data. May be merged with the midrundatamanager if we feel its redundant
* To Do:   N/A
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class PlayerManager : BaseManager
{
    public static PlayerManager Instance;

    [SerializeField]
    private PlayerController playerPrefab;

    [HideInInspector]
    public PlayerController player;

    [HideInInspector]
    public List<BaseWeaponAbilityBehaviour> WeaponsGameObjects { get; private set; } = new(2);

    [SerializeField] private float weaponSwapCooldown;

    [HideInInspector] public bool weaponOnePulledOut;
    private bool canSwap;

    #region Stat Buff Interactable Numbers
    private int DamageBuff;
    private int Firerate;
    private int ProjectileSpeed;
    private int MaxHealth;
    private int MoveSpeed;
    private int DashDistance;
    private int AttackSize;
    private int AbilityCooldown;
    #endregion

    /// <summary>
    /// Stores the data that the player gets from the buff object and then sends it to the weapons
    /// </summary>
    /// <param name="value"></param>
    /// <param name="StatChange"></param>
    public void GetBuffObject(int value, BuffObject.PermStatBuff StatChange)
    {
        switch (StatChange)
        {
            case BuffObject.PermStatBuff.none:
                break;
            case BuffObject.PermStatBuff.Damage:
                DamageBuff += value;
                foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(DamageBuff, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.FireRate:
                Firerate += value;
                foreach(BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if(i != null)
                    {
                        i.weaponStats.GetBIchange(Firerate, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.ProjectileSpeed:
                ProjectileSpeed += value;
                foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(ProjectileSpeed, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.MaxHealth:
                MidRunDataManager.Instance.MaxPlayerHealth += value;
                MidRunDataManager.Instance.ChangePlayerHealth(value);
                break;
            case BuffObject.PermStatBuff.MovementSpeed:
                MoveSpeed += value;
                foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(MoveSpeed, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.DashDistance:
                DashDistance += value;
                foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(DashDistance, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.AttackSize:
                AttackSize += value;
                foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(AttackSize, StatChange);
                    }
                }
                break;
            case BuffObject.PermStatBuff.AbilityCooldown:
                AbilityCooldown = Mathf.CeilToInt(AbilityCooldown * ((100 - value) / 100f)); foreach (BaseWeaponAbilityBehaviour i in WeaponsGameObjects)
                {
                    if (i != null)
                    {
                        i.weaponStats.GetBIchange(AbilityCooldown, StatChange);
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// inits the manager and sets up public events
    /// </summary>
    /// <returns></returns>
    public async override Awaitable InitManager()
    {
        await base.InitManager();

        WeaponsGameObjects.Add(null);
        WeaponsGameObjects.Add(null);
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
    /// subscribes to all the public events 
    /// </summary>
    /// <returns></returns>
    private async Awaitable SetUpPublicEvents()
    {
        InputPublicEvents.SwapWeaponPressed += swapWeapon;
        GenericPublicEvents.StartGamePressed += SpawnPlayer;
        GenericPublicEvents.EquipWeaponInSlot += spawnNewWeapon;

        weaponOnePulledOut = true;
        canSwap = true;
        DamageBuff = 0;
        Firerate = 0;
        ProjectileSpeed = 0;
        MaxHealth = 0;
        MoveSpeed = 0;
        DashDistance = 0;
        AttackSize = 0;
        AbilityCooldown = 100;
        
        await Task.CompletedTask;
    }

    /// <summary>
    /// unsubscribes from public events
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.StartGamePressed -= SpawnPlayer;
        InputPublicEvents.SwapWeaponPressed -= swapWeapon;
        GenericPublicEvents.EquipWeaponInSlot -= spawnNewWeapon;
    }

    /// <summary>
    /// set the weapon to true that should be true
    /// </summary>
    private void swapWeapon()
    {
        if (WeaponsGameObjects[1] != null && canSwap)
        {
            weaponOnePulledOut = !weaponOnePulledOut;
            if (weaponOnePulledOut)
            {
                WeaponsGameObjects[0].EnableWeapon();
                WeaponsGameObjects[1].DisableWeapon();
                GenericPublicEvents.SetCurrentWeapon?.Invoke(WeaponsGameObjects[0]);
            }
            else
            {
                WeaponsGameObjects[0].DisableWeapon();
                WeaponsGameObjects[1].EnableWeapon();
                GenericPublicEvents.SetCurrentWeapon?.Invoke(WeaponsGameObjects[1]);
            }
            StartCoroutine(WeaponSwapCooldown());
        }
    }

    /// <summary>
    /// Spawns in the weapon prefab when you pick up a new weapon
    /// </summary>
    public void spawnNewWeapon(int index, BaseWeaponScriptable scriptable)
    {
        Destroy(WeaponsGameObjects[index]);
        GameObject weapon = Instantiate(scriptable.weaponBehavPrefab, new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.transform.position.z)/*<-temp*/, player.transform.rotation, player.transform);
        weapon.GetComponent<BaseWeaponBehaviour>().ThisWeaponData = scriptable;
        WeaponsGameObjects[index] = weapon.GetComponent<BaseWeaponAbilityBehaviour>();
        WeaponsGameObjects[index].weaponStats.GetBIchange(AbilityCooldown, BuffObject.PermStatBuff.AbilityCooldown); 
        WeaponsGameObjects[index].weaponStats.GetBIchange(AttackSize, BuffObject.PermStatBuff.AttackSize); 
        WeaponsGameObjects[index].weaponStats.GetBIchange(DashDistance, BuffObject.PermStatBuff.DashDistance); 
        WeaponsGameObjects[index].weaponStats.GetBIchange(MoveSpeed, BuffObject.PermStatBuff.MovementSpeed); 
        WeaponsGameObjects[index].weaponStats.GetBIchange(ProjectileSpeed, BuffObject.PermStatBuff.ProjectileSpeed); 
        WeaponsGameObjects[index].weaponStats.GetBIchange(DamageBuff, BuffObject.PermStatBuff.Damage);
        WeaponsGameObjects[index].weaponStats.GetBIchange(Firerate, BuffObject.PermStatBuff.FireRate);
        if (index == 0 && !weaponOnePulledOut)
        {
            WeaponsGameObjects[0].DisableWeapon();
            WeaponsGameObjects[0].weaponModel.SetActive(false);
        }
        else if (index == 1 && weaponOnePulledOut)
        {
            WeaponsGameObjects[1].weaponModel.SetActive(false);
            WeaponsGameObjects[1].DisableWeapon();
        }
        else
        {
            weapon.GetComponent<BaseWeaponAbilityBehaviour>().activeWeapon = true;
        }
    }

    /// <summary>
    /// make it so the player can't switch too much
    /// </summary>
    /// <returns></returns>
    private IEnumerator WeaponSwapCooldown()
    {
        canSwap = false;
        yield return new WaitForSecondsRealtime(weaponSwapCooldown);
        canSwap = true;
    }

    /// <summary>
    /// spawns in the player prefab
    /// </summary>
    private void SpawnPlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab,
                             MapManager.Instance.spawnedMap.PlayerSpawnpoint.position,
                             Quaternion.identity,
                             transform);
        WeaponsGameObjects[0] = player.GetComponentInChildren<BaseWeaponAbilityBehaviour>();
        WeaponsGameObjects[0].activeWeapon = true;
        GenericPublicEvents.PlayerSpawnedIn?.Invoke(player);

        MidRunDataManager.Instance.connectHermit();
    }
}
