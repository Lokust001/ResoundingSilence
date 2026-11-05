/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/20/2026
* Summary: Makes the entity drop loot when it is destroyed
*/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class RewardGiver : MonoBehaviour
{

    [SerializeField]
    private List<Reward> possibleRewards;

    private Vector3 instantiationPosition;

    public void StartGivingRewards(Vector3 dispenseLocation) 
    {
        instantiationPosition = dispenseLocation;
        if (possibleRewards.Count > 0)
            RollRewards();
    }


    /// <summary>
    /// Loops through the loot table and rolls for each loot item independently. 
    /// Loot is acquired if the rolled value is within the range of the random number
    /// </summary>
    private void RollRewards() 
    {
        List<Reward> rewards = new();

        foreach(var loot in possibleRewards) 
        {
            float independentRoll = Random.Range(0, 101) * 0.01f;
            if (independentRoll <= loot.dropChance)
                rewards.Add(loot);
        }

        RewardHandler(rewards);
    }

    /// <summary>
    /// Drops/spawns loot at the position where the enemy dies
    /// </summary>
    /// <param name="gainedRewards"></param>
    private void RewardHandler(List<Reward> gainedRewards) 
    {
        PlayerController playerRef = FindAnyObjectByType<PlayerController>();
        foreach (var reward in gainedRewards)
        {
            switch (reward.type)
            {
                case Reward.RewardType.Gold:
                    int goldAmt = Random.Range(reward.minDropAmt, reward.maxDropAmt + 1);
                    playerRef.GainGold(goldAmt);
                    break;
                case Reward.RewardType.XP:
                    int xpAmt = Random.Range(reward.minDropAmt, reward.maxDropAmt + 1);
                    playerRef.GainXP(xpAmt);
                    break;
                case Reward.RewardType.Weapon:
                case Reward.RewardType.Pin:
                    Instantiate(reward.prefab, instantiationPosition, Quaternion.identity);
                    break;
                default:
                    break;
            }
        }
    }
}
