/* Author: Dalsten Yan
 * Contributors:
 * Last Modified: 9/28/26
 * Summary: Helper script allowing for any player-contacting collider to be able to propagate their actions up to another script
 * TODO: More as needed
 */
using System;
using UnityEngine;

public class TriggerObjects : MonoBehaviour
{
    public event Action<Collider> ChildTriggerActivated;

    /// <summary>
    /// Propagates its OnTriggerEnter event onto other listeners attached to ChildTriggerActivated
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player)) 
        {
            ChildTriggerActivated?.Invoke(other);
        }
    }
}
