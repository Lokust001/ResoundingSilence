/*
* Author: Brenden
* Contributors:
* Last Modified: 10/21/2026
* Summary: Chest Objects around the world that the player can interact with
* To Do:   N/A
*/
using UnityEngine;

public class Chests : MonoBehaviour
{
    public int Goldmax;
    public int Goldmin;
    public int PinMax;
    public int PinMin;
    public GameObject emptyWeaponGameObject;
    public GameObject emptyPinGameObject;
    [Tooltip("% chance to get weapon")] public float WeaponDropChance;

    private bool inChestRange;
    private MidRunDataManager MRM;
    private bool chestOpened;

    /// <summary>
    /// makes the chest listen to the public events for interaction
    /// </summary>
    private void OnEnable()
    {
        chestOpened = false;
        InputPublicEvents.InteractPressed += TryOpenChest;
        MRM = MidRunDataManager.Instance;
    }

    /// <summary>
    /// Removes the listener to the event call
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= TryOpenChest;
    }

    /// <summary>
    /// detects if the player is close enough to interact with the chest
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inChestRange = true;
        }
    }

    /// <summary>
    /// detects if the player has left the interaction range
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inChestRange = false;
        }
    }

    /// <summary>
    /// gives the player all the stuff that spawns from the chest
    /// </summary>
    private void TryOpenChest()
    {
        if (MRM == null)
        {
            MRM = MidRunDataManager.Instance;
        }
        if (inChestRange && !chestOpened)
        {
            MRM.Money += Random.Range(Goldmin, Goldmax + 1);
            int PinSpawn = Random.Range(PinMin, PinMax + 1);
            for (int i = 0; i < PinSpawn; i++)
            {
                GameObject temp = Instantiate(emptyPinGameObject, transform.position + new Vector3(1, 0, 1), transform.rotation, transform);
            }
            if (Random.Range(0, 100) < WeaponDropChance)
            {
                GameObject emptyWeapon = Instantiate(emptyWeaponGameObject, transform.position + new Vector3(1, 0, 1), transform.rotation, transform);
            }
            chestOpened = true;
        }
    }
}
