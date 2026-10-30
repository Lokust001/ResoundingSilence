/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/18/2026
* Summary: Interface that encompasses most enemy-derived behaviors
* To Do:
*/
public interface ISharedEnemyBehaviors
{
    /// <summary>
    /// Method that helps propagate ScriptableObjects into other Scripts that may need them
    /// </summary>
    /// <param name="baseScriptable"></param>
    void ReceiveEnemyData(BaseScriptableObject baseScriptable);

    /// <summary>
    /// De-aggros the enemy in each enemy-derived behavior
    /// </summary>
    void DeAggroEnemy();
}
