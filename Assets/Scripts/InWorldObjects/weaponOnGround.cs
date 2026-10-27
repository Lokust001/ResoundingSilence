/*
* Author: Brenden
* Contributors:
* Last Modified: 10/21/2026
* Summary: Weapon Pickups on the ground
* To Do:   N/A
*/
using System.Data.SqlTypes;
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
        Instantiate(weapon.weaponModel, this.transform).transform.localScale = new Vector3(20,20,20);
    }


    /// <summary>
    /// makes the chest listen to the public events for interaction
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += TryPickup;
        InputPublicEvents.AbilityOnePressed += ReplaceWeaponOne;
        InputPublicEvents.AbilityTwoPressed += ReplaceWeaponTwo;
        MRM = MidRunDataManager.Instance;
    }

    /// <summary>
    /// Removes the listener to the event call
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= TryPickup;
        InputPublicEvents.AbilityOnePressed -= ReplaceWeaponOne;
        InputPublicEvents.AbilityTwoPressed -= ReplaceWeaponTwo;
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
        if (inRange)
        {
            if (MRM.equippedWeapons[1] == null)
            {
                MRM.equippedWeapons[1] = weapon;
                Destroy(this.gameObject);
            }
            else
            {
                inPickup = true;
            }
        }
    }

    /// <summary>
    /// takes the input and replaces weapon based on weapon selection
    /// </summary>
    private void ReplaceWeaponOne()
    {
        if (inPickup)
        {
            Instantiate(DroppedWeaponPrefab, transform.position, transform.rotation).GetComponent<weaponOnGround>().weapon = MRM.equippedWeapons[0];
            MRM.equippedWeapons[0] = weapon;
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// takes the input and replaces weapon based on weapon selection
    /// </summary>
    private void ReplaceWeaponTwo()
    {
        if (inPickup)
        {
            Instantiate(DroppedWeaponPrefab, transform.position, transform.rotation).GetComponent<weaponOnGround>().weapon = MRM.equippedWeapons[1];
            MRM.equippedWeapons[1] = weapon;
            Destroy(this.gameObject);
        }
    }

}
