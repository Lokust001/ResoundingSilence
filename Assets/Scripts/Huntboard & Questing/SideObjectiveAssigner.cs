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

    private RewardGiver rewarder;

    private void Awake()
    {
        objective = objective.CreateNonRefCopy<SideObjective>();
        rewarder = transform.root.GetComponentInChildren<RewardGiver>();
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
