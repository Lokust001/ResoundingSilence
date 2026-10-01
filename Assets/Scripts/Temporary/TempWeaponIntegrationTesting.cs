/*
* Author: Tyler
* Contributors:
* Last Modified: 09/30/2026
* Summary: Temporary script to bridge the gap between the upgrade ui and the crossbow behavior
* To Do:   N/A
*/

using UnityEngine;

public class TempWeaponIntegrationTesting : MonoBehaviour
{
    /// <summary>
    /// subscribes to public event
    /// </summary>
    private void Awake()
    {
        GenericPublicEvents.StartGamePressed += EquipCrossbow;
    }   

    /// <summary>
    /// unsubscribes from public event
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.StartGamePressed -= EquipCrossbow;
    }

    /// <summary>
    /// equips the crossbow when the game starts
    /// </summary>
    private void EquipCrossbow()
    {
        MidRunDataManager.Instance.EquipWeaponInSlot(0, FindAnyObjectByType<CrossbowBehaviour>().ThisWeaponData);
    }
}
