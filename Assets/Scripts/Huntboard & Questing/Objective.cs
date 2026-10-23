using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Scriptables/Objective")]
public class Objective : BaseScriptableObject
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


}
