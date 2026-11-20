/*
* Author: Brenden
* Contributors: Tyler
* Last Modified: 11/6/2026
* Summary: Weapon Pickups on the ground
* To Do:   N/A
*/
using UnityEngine;

public class weaponOnGround : MonoBehaviour
{
    public BaseWeaponScriptable weapon;
    public GameObject DroppedWeaponPrefab;

    private bool inRange;
    private bool inPickup;
    private MidRunDataManager MRM;

    /// <summary>
    /// spawns in the model of the weapon
    /// </summary>
    private void Start()
    {
        weapon = StaticDataManager.Instance.GetRandomWeapon();
        Instantiate(weapon.weaponModel, this.transform).transform.localScale = new Vector3(20, 20, 20);
    }


    /// <summary>
    /// makes the chest listen to the public events for interaction
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += TryPickup;
        MRM = MidRunDataManager.Instance;
    }

    /// <summary>
    /// Removes the listener to the event call
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= TryPickup;
    }

    /// <summary>
    /// detects if the player is close enough to interact with the chest
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inRange = true;
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
            inRange = false;
            inPickup = false;
        }
    }

    /// <summary>
    /// if in range grab try and equip weapon
    /// </summary>
    private void TryPickup()
    {
        if (MRM == null)
        {
            MRM = MidRunDataManager.Instance;
        }
        if (inPickup)
        {
            inPickup = false;
        }
        else
        {
            if (inRange)
            {
                int i = MRM.equippedWeapons.FindIndex(x => x == null);

                if (i >= 0)
                {
                    GenericPublicEvents.EquipWeaponInSlot?.Invoke(i, weapon);
                    Destroy(this.gameObject);
                }
                else
                {
                    inPickup = true;
                    MRM.weaponSwappingContainer = this;
                    UIPublicEvents.UpdateUIManagerStack?.Invoke(UiMenuType.WeaponSwapping);
                }
            }
        }

    }

    /// <summary>
    /// replaces the spawned dropped weapon with this one
    /// </summary>
    /// <param name="i"></param>
    public void ReplaceDroppedWeapon(int i = -1)
    {
        if (inPickup && i != -1)
        {
            Instantiate(DroppedWeaponPrefab, transform.position, transform.rotation).GetComponent<weaponOnGround>().weapon = MRM.equippedWeapons[i];
            GenericPublicEvents.EquipWeaponInSlot?.Invoke(i, weapon);
            Destroy(this.gameObject);
        }
    }

}
