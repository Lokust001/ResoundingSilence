using NUnit.Framework;
using UnityEngine;

public class TrappedChests : MonoBehaviour
{
    public int Goldmax;
    public int Goldmin;
    public int PinMax;
    public int PinMin;
    public GameObject emptyPinGameObject;
    public GameObject emptyWeaponGameObject;
    public EnemySpawner EnemySpawner;

    private bool inChestRange;
    private bool chestOpened;
    private bool encounterStarted;
    private bool encounterFinished;
    private MidRunDataManager MRM;

    /// <summary>
    /// makes the chest listen to the public events for interaction
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.InteractPressed += TryOpenChest;
        GenericPublicEvents.EnemyDies += CheckQuest;
        MRM = MidRunDataManager.Instance;
        chestOpened = false;
        encounterStarted = false;
        encounterFinished = false;
    }

    /// <summary>
    /// Removes the listener to the event call
    /// </summary>
    private void OnDisable()
    {
        GenericPublicEvents.EnemyDies -= CheckQuest;
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

    private void CheckQuest(BaseEnemyScriptable enemy)
    {
        if(EnemySpawner.generatedEnemies.Count == 0)
        {
            //probably a ui popup will go here
            encounterFinished=true;
        }
        else
        {
            encounterFinished = false;
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
        if (inChestRange && !encounterStarted)
        {
            EnemySpawner.SpawnEnemies();
            encounterStarted = true;
        }

        CheckQuest(null);
        if (inChestRange && encounterFinished && !chestOpened)
        {
            MRM.Money += Random.Range(Goldmin, Goldmax + 1);
            int PinSpawn = Random.Range(PinMin, PinMax + 1);
            for (int i = 0; i < PinSpawn; i++)
            {
                GameObject temp = Instantiate(emptyPinGameObject, transform.position + new Vector3(1, 0, 1), transform.rotation, transform);
                temp.GetComponent<PinWorldObject>().pin = MRM.PickDroppedPin();
            }
            GameObject emptyWeapon = Instantiate(emptyWeaponGameObject, transform.position + new Vector3(1, 0, 1), transform.rotation, transform);
            chestOpened=true;
        }
    }
}
