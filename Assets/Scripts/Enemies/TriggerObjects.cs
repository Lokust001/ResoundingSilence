using System;
using UnityEngine;

public class TriggerObjects : MonoBehaviour
{

    public event Action<Collider> ChildTriggerActivated;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.TryGetComponent<PlayerController>(out PlayerController player)) 
        {
            ChildTriggerActivated?.Invoke(other);
        }
    }
}
