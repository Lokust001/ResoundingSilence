/*
* Author: Tyler
* Contributors: Brenden
* Last Modified: 11/11/2026
* Summary: This service stores all the player's data. May be merged with the midrundatamanager if we feel its redundant
* To Do:   N/A
*/

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
    public List<BaseWeaponBehaviour> WeaponsGameObjects { get; private set; } = new(2);

    private bool weaponOnePulledOut;
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
    /// 
    /// </summary>
    private void swapWeapon()
    {
        if (WeaponsGameObjects[1] != null)
        {
            weaponOnePulledOut = !weaponOnePulledOut;
            if (weaponOnePulledOut)
            {
                WeaponsGameObjects[0].gameObject.SetActive(true);
                WeaponsGameObjects[1].gameObject.SetActive(false);
            }
            else
            {
                WeaponsGameObjects[0].gameObject.SetActive(false);
                WeaponsGameObjects[1].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void spawnNewWeapon(int index, BaseWeaponScriptable scriptable)
    {
        GameObject weapon = Instantiate(scriptable.weaponBehavPrefab, player.transform);
        weapon.GetComponent<BaseWeaponBehaviour>().ThisWeaponData = scriptable;
        WeaponsGameObjects[index] = weapon.GetComponent<BaseWeaponBehaviour>();
        if(index == 0 && !weaponOnePulledOut)
        {
            weapon.SetActive(false);
        }
        else if(index == 1 && weaponOnePulledOut)
        {
            weapon.SetActive(false);
        }
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
        WeaponsGameObjects[0] = player.GetComponentInChildren<BaseWeaponBehaviour>();
        GenericPublicEvents.PlayerSpawnedIn?.Invoke(player);
    }
}
