/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/5/2026
* Summary: The objective ScriptableObject that can be tailored to HuntObjectives and much more
* To Do:   Add more objectives as needed.
*/
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptables/Objective")]
public class SideObjective : BaseScriptableObject
{
    public enum ObjectiveType 
    {
        None,
        HuntObjective
    }

    public enum HuntObjective 
    {
        AllEnemies,
        RangedEnemies,
        MeleeEnemies,
        OneEnemyTypeOnly
    }
    [Header("Objective Type Selection")]
    public ObjectiveType objectiveType;

    [ShowIf(nameof(objectiveType), ObjectiveType.HuntObjective)]
    [Header("Hunt Objective Enemy Target")]
    public HuntObjective huntObjectiveType;

    [ShowIf(nameof(huntObjectiveType), HuntObjective.OneEnemyTypeOnly)]
    [Header("Enemy Type to Hunt")]
    public BaseEnemyScriptable.EnemyType enemyType;
    
    [ShowIf(nameof(objectiveType), ObjectiveType.HuntObjective)]
    public int huntedEnemiesGoal;

    [ShowIf(nameof(objectiveType), ObjectiveType.HuntObjective)]
    public int huntEnemiesProgress = 0;

    /// <summary>
    /// Attempts to progress a hunt enemies objective based on 
    /// the type of enemy to be hunted
    /// </summary>
    /// <param name="enemyData"></param>
    public bool ProgressHuntObjective(BaseEnemyScriptable enemyData) 
    {
        //Base case for AllEnemies
        bool progress = true;
        switch (huntObjectiveType)
            //cases where progress can be evaluated to false
        {
            case HuntObjective.RangedEnemies:
                progress = enemyData.IsShooter();
                break;
            case HuntObjective.MeleeEnemies:
                progress = enemyData.IsMelee();
                break;
            case HuntObjective.OneEnemyTypeOnly:
                progress = enemyData.enemyType == enemyType;
                break;
        }

        //It doesn't always progress because an enemy may be the wrong type of HuntObjective
        if (progress)
        {
            huntEnemiesProgress++;
            UIPublicEvents.UpdateHUDHuntObjs?.Invoke(this);
        }
            

        return huntEnemiesProgress >= huntedEnemiesGoal;
    }
    
    /// <summary>
    /// Calculates the tooltip for the objective
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    public string GetSideObjectiveTooltipText()
    {
        string retStr = "";

        switch (objectiveType)
        {
            case ObjectiveType.None:
                throw new System.Exception("Side Objective's obj type is None.");
            case ObjectiveType.HuntObjective:
                retStr = GetHuntTooltipText();
                break;
        }
        return retStr;
    }

    /// <summary>
    /// Calculates the hunt tooltip text
    /// </summary>
    /// <returns></returns>
    private string GetHuntTooltipText()
    {
        string retStr = $"Defeat {huntedEnemiesGoal} ";

        switch (huntObjectiveType)
        {
            case HuntObjective.AllEnemies:
                break;
            case HuntObjective.RangedEnemies:
                retStr += $"Ranged ";
                break;
            case HuntObjective.MeleeEnemies:
                retStr += $"Melee ";
                break;
            case HuntObjective.OneEnemyTypeOnly:
                retStr += $"{enemyType.ToString()} ";
                break;
        }

        retStr += "Enemies";

        return retStr;
    }
}
