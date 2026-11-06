/*
* Author: Brenden
* Contributors:
* Last Modified: 10/21/2026
* Summary: Chest Objects around the world that the player can interact with
* To Do:   N/A
*/
using NUnit.Framework.Constraints;
using UnityEngine;

public class BuffObject : MonoBehaviour
{
    public enum PermStatBuff
    {
        none,
        Damage,
        FireRate,
        ProjectileSpeed,
        MaxHealth,
        MovementSpeed,
        DashDistance,
        AttackSize,
        AbilityCooldown
    }

    private bool inRange;
    private PlayerController Player;
    public PermStatBuff Stat;

    /// <summary>
    /// makes the chest listen to the public events for interaction
    /// </summary>
    private void OnEnable()
    {
        Player = FindAnyObjectByType<PlayerController>();
        inRange = false;
        InputPublicEvents.InteractPressed += TryCollect;
    }

    /// <summary>
    /// Removes the listener to the event call
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.InteractPressed -= TryCollect;
    }

    /// <summary>
    /// detects if the player is close enough to interact with the chest
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inRange = true;
        }
    }

    /// <summary>
    /// detects if the player has left the interaction range
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            inRange = false;
        }
    }

    /// <summary>
    /// gives the player all the stuff that spawns from the chest
    /// </summary>
    private void TryCollect()
    {
        if (inRange)
        {
            switch (Stat)
            {
                case PermStatBuff.none:
                    break;
                case PermStatBuff.Damage:
                    break;
                case PermStatBuff.FireRate:
                    break;
                case PermStatBuff.ProjectileSpeed:
                    break;
                case PermStatBuff.MaxHealth:
                    break;
                case PermStatBuff.MovementSpeed:
                    break;
                case PermStatBuff.DashDistance:
                    break;
                case PermStatBuff.AttackSize:
                    break;
                case PermStatBuff.AbilityCooldown:
                    break;
                default:
                    break;
            }
        }
    }
}
