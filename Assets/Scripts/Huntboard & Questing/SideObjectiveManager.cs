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

    public List<SideObjective> completedObjectives;

    //Can be expanded to include gather, destination, or other objectives
    /// <summary>
    /// Implement singleton interface and overwrite referenced SO's with non-refs instead
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

        completedObjectives = new();

        for(int i = 0; i < currentHuntObjectives.Count; i++)
        {
            currentHuntObjectives[i] = currentHuntObjectives[i].CreateNonRefCopy<SideObjective>();
        }
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
