using System.Collections;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class PinWorldObject : MonoBehaviour
{
    public PinScriptable pin;
    public float Speed;
    public float DelayTime;

    /// <summary>
    /// calls flight delay when the object is made
    /// </summary>
    private void Start()
    {
        StartCoroutine(FlightDelay());
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
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && other.name != "RangedArea")
        {
            MidRunDataManager.Instance.AddPinToInventory(pin);
            Destroy(this.gameObject);
        }
    }
}
