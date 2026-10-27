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

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public Objective.ObjectiveType boardType;

    [ShowIf(nameof(boardType), Objective.ObjectiveType.HuntObjective)]
    [Header("Hunt Board Objectives")]
    public List<Objective> currentHuntObjectives;

    //Can be expanded to include gather, destination, or other objectives

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else { Destroy(this); }
    }

    /// <summary>
    /// Called after an enemy death to advance current hunt objectives if any are fulfilled
    /// </summary>
    /// <param name="huntedEnemyData"></param>
    public void AttemptHuntObjectivesProgression(BaseEnemyScriptable huntedEnemyData) 
    {
        foreach (Objective obj in currentHuntObjectives) 
        {
            obj.ProgressHuntObjective(huntedEnemyData);
        }
    }
}
