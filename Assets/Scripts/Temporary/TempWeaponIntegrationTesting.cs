using UnityEngine;

public class TempWeaponIntegrationTesting : MonoBehaviour
{
    private void Awake()
    {
        GenericPublicEvents.StartGamePressed += EquipCrossbow;
    }   

    private void OnDestroy()
    {
        GenericPublicEvents.StartGamePressed -= EquipCrossbow;
    }

    private void EquipCrossbow()
    {
        MidRunDataManager.Instance.EquipWeaponInSlot(0, FindAnyObjectByType<CrossbowBehaviour>().ThisWeaponData);
    }
}
