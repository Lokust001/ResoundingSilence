using System.Collections;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;

public class PinWorldObject : MonoBehaviour
{
    public PinScriptable pin;
    public float Speed;
    public float DelayTime;

    private void Start()
    {
        StartCoroutine(FlightDelay());
    }

    private IEnumerator FlightDelay()
    {
        yield return new WaitForSecondsRealtime(DelayTime);
        StartCoroutine(FlyToPlayer());
    }

    private IEnumerator FlyToPlayer()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameObject.FindAnyObjectByType<PlayerController>().gameObject.transform.position, Speed);
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && other.name != "RangedArea")
        {
            MidRunDataManager.Instance.AddPinToInventory(pin);
            Destroy(this.gameObject);
        }
    }
}
