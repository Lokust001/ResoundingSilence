/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/27/2026
* Summary: The singleton manager for all objectives throughout the game
* To Do:   Add more objective progression as needed.
*/
using NaughtyAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SideObjectiveManager : BaseService
{
    public static SideObjectiveManager Instance;

    public SideObjective.ObjectiveType boardType;

    [ShowIf(nameof(boardType), SideObjective.ObjectiveType.HuntObjective)]
    [Header("Hunt Board Objectives")]
    public List<SideObjective> currentHuntObjectives;

    public List<SideObjective> completedObjectives;

    //Can be expanded to include gather, destination, or other objectives
    /// <summary>
    /// Implement singleton interface and overwrite referenced SO's with non-refs instead
    /// </summary>
    public override async Awaitable InitService()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        GenericPublicEvents.EnemyDies += AttemptHuntObjectivesProgression;
        completedObjectives = new();

        for (int i = 0; i < currentHuntObjectives.Count; i++)
        {
            currentHuntObjectives[i] = currentHuntObjectives[i].CreateNonRefCopy<SideObjective>();
        }

        await base.InitService();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Removes listeners after manager is destroyed
    /// </summary>
    private void OnDestroy()
    {
        GenericPublicEvents.EnemyDies -= AttemptHuntObjectivesProgression;
    }

    /// <summary>
    /// Adds a new objective to an objectivelist depending on what kind of quest it is
    /// </summary>
    /// <param name="sideObjective"></param>
    public void AddNewSideObjective(SideObjective sideObjective) 
    {

        switch (sideObjective.objectiveType)
        {
            case SideObjective.ObjectiveType.HuntObjective:
                currentHuntObjectives.Add(sideObjective);
                break;
            default:
                Debug.LogError("Invalid Objective Type Detected!");
                break;
        }

    }

    /// <summary>
    /// Returns a boolean of whether or not the objective given has been finished
    /// </summary>
    /// <param name="sideObjectiveToCheck"></param>
    /// <returns></returns>
    public bool CheckObjectiveCompleted(SideObjective sideObjectiveToCheck) 
    {
        return completedObjectives.Contains(sideObjectiveToCheck);
    }

    /// <summary>
    /// Called after an enemy death to advance current hunt objectives if any are fulfilled
    /// </summary>
    /// <param name="huntedEnemyData"></param>
    public void AttemptHuntObjectivesProgression(BaseEnemyScriptable huntedEnemyData) 
    {
        for (int i = currentHuntObjectives.Count - 1; i >= 0; i--) 
        {
            var obj = currentHuntObjectives[i];
            if (obj.ProgressHuntObjective(huntedEnemyData)) 
            {
                completedObjectives.Add(obj);
                currentHuntObjectives.Remove(obj);
            }
        }
    }
}
