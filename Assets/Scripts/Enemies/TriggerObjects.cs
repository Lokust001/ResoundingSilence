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
    [SerializeField]
    private Color triggerShapeColor;

    public event Action<PlayerController> EnemyTriggerActivated;
    public event Action<Enemy, bool> PlayerTriggerActivated;

    /// <summary>
    /// Propagates its OnTriggerEnter event onto other listeners attached to ChildTriggerActivated
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player)) 
        {
            EnemyTriggerActivated?.Invoke(player);
        }
        else if (other.TryGetComponent<Enemy>(out var enemy)) 
        {
            PlayerTriggerActivated?.Invoke(enemy, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player)) 
        {
            EnemyTriggerActivated?.Invoke(player);
        }
        else if (other.TryGetComponent<Enemy>(out var enemy))
        {
            PlayerTriggerActivated?.Invoke(enemy, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerShapeColor;
        Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius);
    }
}
