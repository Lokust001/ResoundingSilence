using System.Collections;
using UnityEngine;

public class PinWorldObject : MonoBehaviour
{
    public PinScriptable pin;
    public float Speed;

    private void Start()
    {
        StartCoroutine(FlyToPlayer());
    }

    private IEnumerator FlyToPlayer()
    {
        Vector3.MoveTowards(transform.position, GameObject.FindAnyObjectByType<PlayerController>().gameObject.transform.position, Speed);
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            MidRunDataManager.Instance.AddPinToInventory(pin);
        }
    }
}
