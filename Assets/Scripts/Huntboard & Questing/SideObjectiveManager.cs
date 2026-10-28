/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/27/2026
* Summary: The singleton manager for all objectives throughout the game
* To Do:   Add more objective progression as needed.
*/
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SideObjectiveManager : MonoBehaviour
{
    public static SideObjectiveManager Instance;

    public SideObjective.ObjectiveType boardType;

    [ShowIf(nameof(boardType), SideObjective.ObjectiveType.HuntObjective)]
    [Header("Hunt Board Objectives")]
    public List<SideObjective> currentHuntObjectives;

    public List<SideObjective> completedHuntObjectives;

    //Can be expanded to include gather, destination, or other objectives
    /// <summary>
    /// Implement singleton interface and create a non-ref copy of all objectives
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        var nonRefCopyList = completedHuntObjectives = new List<SideObjective>();
        foreach (var item in currentHuntObjectives)
        {
            nonRefCopyList.Add(item.CreateNonRefCopy<SideObjective>());
        }

        currentHuntObjectives = nonRefCopyList;
    }

    /// <summary>
    /// Called after an enemy death to advance current hunt objectives if any are fulfilled
    /// </summary>
    /// <param name="huntedEnemyData"></param>
    public void AttemptHuntObjectivesProgression(BaseEnemyScriptable huntedEnemyData) 
    {
        bool canRemoveActive = false;
        foreach (SideObjective obj in currentHuntObjectives) 
        {
            canRemoveActive = obj.ProgressHuntObjective(huntedEnemyData);
            if (canRemoveActive) 
            {
                completedHuntObjectives.Add(obj);
            }
        }

        if(canRemoveActive)
            currentHuntObjectives.RemoveAll(obj => obj.huntedEnemiesGoal >= obj.huntEnemiesProgress);
    }
}
