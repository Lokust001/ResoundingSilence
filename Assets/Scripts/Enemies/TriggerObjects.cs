/* Author: Dalsten Yan
 * Contributors:
 * Last Modified: 11/13/26
 * Summary: Helper script allowing for any player-contacting collider to be able to propagate their actions up to another script
 * TODO: More as needed
 */
using System;
using UnityEditor.Build;
using UnityEngine;

public class TriggerObjects : MonoBehaviour
{
    private Collider triggerCollider;

    [SerializeField]
    private Color triggerShapeColor;

    public event Action<PlayerController> EnemyTriggerActivated;
    public event Action<Enemy, bool> PlayerTriggerActivated;
    public event Action<bool> PlayerEnterOrExit;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Propagates its OnTriggerEnter event onto other listeners attached to ChildTriggerActivated or PlayerTriggerActivated
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (other.TryGetComponent<PlayerController>(out var player)) 
        {
            EnemyTriggerActivated?.Invoke(player);
            PlayerEnterOrExit?.Invoke(true);
        }
        else if (other.TryGetComponent<Enemy>(out var enemy)) 
        {
            PlayerTriggerActivated?.Invoke(enemy, true);
        }
    }

    /// <summary>
    /// Propagates its OnTriggerExit event onto other listeners attached to ChildTriggertActivated or PlayerTriggerActivated
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (!enabled)
            return;

        if (other.TryGetComponent<PlayerController>(out var player)) 
        {
            EnemyTriggerActivated?.Invoke(player);
            PlayerEnterOrExit?.Invoke(false);
        }
        else if (other.TryGetComponent<Enemy>(out var enemy))
        {
            PlayerTriggerActivated?.Invoke(enemy, false);
        }
    }

    public void SetTriggerEnabled(bool state) 
    {
        triggerCollider.enabled = state;
    }

    /// <summary>
    /// Draws a wiresphere for visualization and differentiaion purposes when the object is selected in the scene
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerShapeColor;
        if(triggerCollider is SphereCollider)
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * transform.root.localScale.y);
        if(triggerCollider is MeshCollider)
            Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh, transform.position, transform.rotation, transform.localScale);
    }
}
