/* Author: Dalsten Yan
 * Last Modified: 10/21/26
 * Summary: Abstract class that serves as a template for other consumables and holds shared consumable behavior
 * Contributors:
 */
using UnityEngine;
public abstract class ConsumableBehavior : MonoBehaviour
{
    public float consumableValue;
    /// <summary>
    /// Abstract method that each consumable can have their own implementation
    /// </summary>
    /// <param name="player"></param>
    public abstract void Consume(PlayerController player = null);

    /// <summary>
    /// Activates the consume method when the player enters the trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Consume(player);
        }
    }
}


