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

    private void TryOpenChest()
    {
        if (inChestRange)
        {
            MRM.Money += Random.Range(Goldmin, Goldmax);
            int PinSpawn = Random.Range(PinMin, PinMax);
            for(int i = 0; i < PinSpawn; i++)
            {
                GameObject temp = Instantiate(emptyPinGameObject);
                temp.GetComponent<PinWorldObject>().pin = MRM.PickDroppedPin();
            }
            if(Random.Range(0, 100) < WeaponDropChance)
            {
                //TODO drop Weapon
            }
        }
    }
}
