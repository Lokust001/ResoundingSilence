/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/5/2026
* Summary: Allows a SideObjective to be taken up
*/
using UnityEngine;

public class SideObjectiveAssigner : MonoBehaviour
{
    [SerializeField]
    private SideObjective objective;

    private void Awake()
    {
        objective = objective.CreateNonRefCopy<SideObjective>();
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
    /// Method to claim the reward, either by directly assigining the reward or creating it in scene
    /// </summary>
    public void ClaimRewards()
    {
        GetComponent<RewardGiver>().StartGivingRewards(transform.position);
    }
}
