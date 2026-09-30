/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/18/2026
* Summary: Interface that propagates date
* To Do:
*/
public interface IEntityDataReceiver
{
    /// <summary>
    /// Method that helps propagate ScriptableObjects into other Scripts that may need them
    /// </summary>
    /// <param name="baseScriptable"></param>
    void SetEntityData(BaseScriptableObject baseScriptable);
}
