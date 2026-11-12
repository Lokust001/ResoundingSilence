/*
* Author: Tyler
* Contributors: Brenden
* Last Modified: 11/11/2026
* Summary: This service stores all the player's data. May be merged with the midrundatamanager if we feel its redundant
* To Do:   N/A
*/

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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

    private bool weaponOnePulledOut;
    private bool canSwap;

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
        if(index == 0 && !weaponOnePulledOut)
        {
            WeaponsGameObjects[0].DisableWeapon();
            WeaponsGameObjects[0].weaponModel.SetActive(false);
        }
        else if(index == 1 && weaponOnePulledOut)
        {
            WeaponsGameObjects[1].weaponModel.SetActive(false);
            WeaponsGameObjects[1].DisableWeapon();
        }
        else
        {
            weapon.GetComponent<BaseWeaponAbilityBehaviour>().activeWeapon = true;
        }
    }

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
        player = Instantiate(playerPrefab,
                             MapManager.Instance.spawnedMap.PlayerSpawnpoint.position,
                             Quaternion.identity,
                             transform);
        WeaponsGameObjects[0] = player.GetComponentInChildren<BaseWeaponAbilityBehaviour>();
        WeaponsGameObjects[0].activeWeapon = true;
        GenericPublicEvents.PlayerSpawnedIn?.Invoke(player);
    }
}
