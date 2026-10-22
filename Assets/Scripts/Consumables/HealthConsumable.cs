/* Author: Dalsten Yan
 * Last Modified: 10/21/26
 * Summary: Consumable that heals the player
 * Contributors:
 */
public class HealthConsumable : ConsumableBehavior
{
    /// <summary>
    /// Heals the player with the specified consumableValue when they touch the consumable
    /// </summary>
    /// <param name="player"></param>
    public override void Consume(PlayerController player)
    {
        player.HealPlayer((int)consumableValue);
    }
}
