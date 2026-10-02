/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 9/29/2026
 * Brief: Testing script for testing weapon stuff against the player.
 * TODO: Delete this once the crossbow is hooked up to the player. 
 *       PlayerController was checked out at the time of making this.
 * ***************************************************************************/
using TMPro;
using UnityEngine;

public class DummyPlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private TMP_Text healthText;

    /// <summary>
    /// TEMPORARY - writes the amount to heal above this objects head
    /// </summary>
    /// <param name="healthToHeal"></param>
    public void Heal(float healthToHeal)
    {
        healthText.text = $"Healed for <color=green>{healthToHeal}</color>";
    }
}
