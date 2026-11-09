/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/20/2026
* Summary: Makes the entity drop loot when it is destroyed
*/

using System.Collections.Generic;
using UnityEngine;

public class RewardGiver : MonoBehaviour
{

    public List<Reward> possibleRewards;
    [SerializeField, Header("Drag Transforms in here")]
    [Tooltip("The transforms where gameobjects will be instantiated and can be picked up by the player. If empty, will use own transform")]
    private List<Transform> dropItemsTransforms;

    [Header("[Debug]")]
    public bool logRNGNumber;
    public bool logRewardedQty;

    /// <summary>
    /// Fail-safe if no transforms were specified
    /// </summary>
    private void Awake()
    {
        if (dropItemsTransforms.Count == 0)
            dropItemsTransforms.Add(transform.root);
    }

    /// <summary>
    /// Public method to initiate reward dispensing
    /// </summary>
    public void StartGivingRewards()
    {
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

        foreach (var loot in possibleRewards)
        {
            int independentRoll = Random.Range(1, 101);
            if (logRNGNumber)
                Debug.Log($"Rolled: {independentRoll} against the reward ({loot.type}'s) {loot.dropChance} chance");
            if (independentRoll <= loot.dropChance)
                rewards.Add(loot);
        }

        RewardHandler(rewards);
    }

    /// <summary>
    /// Drops/spawns loot at various positions or adds them directly to the player
    /// </summary>
    /// <param name="gainedRewards"></param>
    private void RewardHandler(List<Reward> gainedRewards)
    {

        if (gainedRewards.Count > dropItemsTransforms.Count)
            InsufficientTransformsFailsafe(gainedRewards.Count - dropItemsTransforms.Count);

        Stack<Transform> rewardDropLocations = new(dropItemsTransforms);


        foreach (var reward in gainedRewards)
        {
            int rewardedQuantity = Random.Range(reward.minDropAmt, reward.maxDropAmt + 1);

            if (logRewardedQty)
                Debug.Log($"Quantity of {reward.type} rolled to be {rewardedQuantity} in a range of [{reward.minDropAmt} , {reward.maxDropAmt}]");

            switch (reward.type)
            {
                case Reward.RewardType.Gold:
                    MidRunDataManager.Instance.Money += rewardedQuantity;
                    break;
                case Reward.RewardType.XP:
                    MidRunDataManager.Instance.GainXP(rewardedQuantity);
                    break;
                case Reward.RewardType.Weapon:
                case Reward.RewardType.Pin:
                    for (int i = 0; i < rewardedQuantity; i++)
                    {
                        Vector3 instantiatePos = rewardDropLocations.Pop().position;
                        Instantiate(reward.prefab, instantiatePos, Quaternion.identity);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Duplicates the Transform objects in dropItemsTransforms to accomodate the difference between 
    /// the number of rewarded items & the provided spawn locations. It is then set permanently
    /// for the run
    /// </summary>
    /// <param name="missingTransformDiff"></param>
    private void InsufficientTransformsFailsafe(int missingTransformDiff) 
    {
        Debug.LogWarning("Not enough Transforms in RewardGiver to accomodate the given rewards, creating " + missingTransformDiff + " new Transform(s)");

        //This for loop will try to do uniform duplication
        //If dropItemsTransforms has [Transform1, Transform2];
        //Then, it will result in [Transform1, Transform2, Transform1, Transform2, ...]
        for (int i = 0; i < missingTransformDiff; i++) 
        {
            Transform duplicateTransform = dropItemsTransforms[i];
            dropItemsTransforms.Add(duplicateTransform);
        }
    }
}
