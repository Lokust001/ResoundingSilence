using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private BaseEnemyScriptable enemyData;
    void Awake()
    {
        enemyData = enemyData.CreateNonRefCopy<BaseEnemyScriptable>();
        PropagateEnemyData();
        
    }

    void PropagateEnemyData() 
    {
        foreach (IEntityDataReceiver entity in GetComponentsInChildren<IEntityDataReceiver>())
        {
            entity.SetEntityData(enemyData);
        }
    }
}
