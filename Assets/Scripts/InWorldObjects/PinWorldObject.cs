/*
* Author: Brenden
* Contributors:
* Last Modified: 11/13/2026
* Summary: what the pin has when it spawns in and flys to the player for collection
* To Do:   N/A
*/
using System.Collections;
using UnityEngine;

public class PinWorldObject : MonoBehaviour
{
    public PinScriptable pin;
    public float Speed;
    public float DelayTime;

    bool inFlight;

    /// <summary>
    /// calls flight delay when the object is made
    /// </summary>
    private void Start()
    {
        StartCoroutine(FlightDelay());
        inFlight = false;
        pin = MidRunDataManager.Instance.PickDroppedPin();
    }

    /// <summary>
    /// waits for a bit to play something before heading to the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlightDelay()
    {
        yield return new WaitForSecondsRealtime(DelayTime);
        StartCoroutine(FlyToPlayer());
    }

    /// <summary>
    /// flys to the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlyToPlayer()
    {
        inFlight = true;
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameObject.FindAnyObjectByType<PlayerController>().gameObject.transform.position, Speed);
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// when it reaches the player it adds to the manager that keeps track of pins and then deletes itself
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && other.name != "RangedArea" && inFlight)
        {
            MidRunDataManager.Instance.AddPinToInventory(pin);
            Destroy(this.gameObject);
        }
    }
}
