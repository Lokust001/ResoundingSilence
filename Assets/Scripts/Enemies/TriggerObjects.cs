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
    public event Action<bool> PlayerEnterOrExit;

    /// <summary>
    /// Propagates its OnTriggerEnter event onto other listeners attached to ChildTriggerActivated or PlayerTriggerActivated
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
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

    /// <summary>
    /// Draws a wiresphere for visualization and differentiaion purposes when the object is selected in the scene
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = triggerShapeColor;
        if(GetComponent<SphereCollider>())
            Gizmos.DrawWireSphere(transform.position, GetComponent<SphereCollider>().radius * transform.root.localScale.y);
        if(GetComponent<MeshCollider>())
            Gizmos.DrawWireMesh(GetComponent<MeshCollider>().sharedMesh, transform.position, transform.rotation, transform.localScale);
    }
}
