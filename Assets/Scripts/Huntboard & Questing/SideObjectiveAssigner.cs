/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/5/2026
* Summary: Allows a SideObjective to be taken up
*/
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SideObjectiveAssigner : MonoBehaviour
{
    [SerializeField]
    private SideObjective objective;

    [SerializeField]
    private RewardGiver rewarder;

    /// <summary>
    /// Creates a non-referencing copy of the objective and tries to find the RewardGiver on this object
    /// </summary>
    private void Awake()
    {
        if (objective != null)
            objective = objective.CreateNonRefCopy<SideObjective>();
        else
            Debug.LogWarning("Warning, a HuntBoardPost does not have a SideObjective assigned");
    }

    /// <summary>
    /// public getter/ref to the sideobjective
    /// </summary>
    /// <returns></returns>
    public SideObjective GetSideObjective()
    {
        return objective;
    }

    /// <summary>
    /// Accepts the objective and starts tracking progress
    /// </summary>
    [Button("Debug Accept Side Objective")]
    public void AcceptSideObjective()
    {
        SideObjectiveManager.Instance.AddNewSideObjective(objective);
    }

    /// <summary>
    /// Returns if the objective was completed
    /// </summary>
    /// <returns></returns>
    public bool CheckObjectiveCompleted()
    {
        return SideObjectiveManager.Instance.CheckObjectiveCompleted(objective);
    }

    /// <summary>
    /// Gives information about the objective's rewards
    /// </summary>
    /// <returns></returns>
    public List<Reward> GetObjectiveRewards()
    {
        return rewarder.possibleRewards;
    }

    /// <summary>
    /// Method to claim the reward, either by directly assigining the reward or creating it in scene
    /// </summary>
    /// 
    [Button("Debug Claim Rewards")]
    public void ClaimRewards()
    {
        rewarder.StartGivingRewards();
    }
}
