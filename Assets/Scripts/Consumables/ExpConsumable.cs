/* Author: Dalsten Yan
 * Last Modified: 9/28/26
 * Summary: Consumable that gives player XP
 * TODO: More as needed
 */

public class ExpConsumable : ConsumableBehavior
{
    public override void Consume(PlayerController player = null)
    {
        MidRunDataManager.Instance.GainXP((int)consumableValue);
    }
}
