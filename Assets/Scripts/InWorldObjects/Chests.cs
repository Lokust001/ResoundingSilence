using UnityEngine;

public class Chests : MonoBehaviour
{
    public int Goldmax;
    public int Goldmin;
    public int PinMax;
    public int PinMin;
    public GameObject emptyPinGameObject;
    [Tooltip("% chance to get weapon")] public float WeaponDropChance;

    private bool inChestRange;
    private MidRunDataManager MRM;

    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += TryOpenChest;
        MRM = MidRunDataManager.Instance;
    }

    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= TryOpenChest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inChestRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inChestRange = false;
        }
    }

    private void TryOpenChest()
    {
        if (MRM == null)
        {
            MRM = MidRunDataManager.Instance;
        }
        if (inChestRange)
        {
            MRM.Money += Random.Range(Goldmin, Goldmax + 1);
            int PinSpawn = Random.Range(PinMin, PinMax + 1);
            for (int i = 0; i < PinSpawn; i++)
            {
                GameObject temp = Instantiate(emptyPinGameObject, transform.position + new Vector3(1, 0, 1), transform.rotation, transform);
                temp.GetComponent<PinWorldObject>().pin = MRM.PickDroppedPin();
            }
            if (Random.Range(0, 100) < WeaponDropChance)
            {
                //TODO drop Weapon
            }
        }
    }
}
