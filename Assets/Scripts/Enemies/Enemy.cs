using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private BaseEnemyScriptable enemyData;

    [SerializeField, Header("Debug Variables")]
    private int dmgToTake;
    void Awake()
    {
        enemyData = enemyData.CreateNonRefCopy<BaseEnemyScriptable>();
        PropagateEnemyData();
    }

    private void Start()
    {
        switch (enemyData.enemyType)
        {
            case BaseEnemyScriptable.EnemyType.BuffEnemy:
                GetComponentInChildren<EnemyBuff>(true).ChargeBuff(); 
                break;
            default:
                GetComponent<EnemyWalk>().StartPlayerSearch();
                break;
        }
    }

    void PropagateEnemyData() 
    {
        foreach (IEntityDataReceiver entity in GetComponentsInChildren<IEntityDataReceiver>(true))
        {
            entity.SetEntityData(enemyData);
        }
    }

    public void ReceiveBuff(float atkModifier, float dmgModifier) 
    {
        enemyData.atkBoostFactor = atkModifier;
        enemyData.dmgReductionFactor = dmgModifier;

        Debug.Log("I've been buffed! My attacks now deal: " + EnemyDealDamage() + "dmg, and if an attack hits me that is " + dmgToTake + " dmg, I only take " + EnemyTakeDamage(dmgToTake));
    }

    public void LoseBuff() 
    {
        enemyData.atkBoostFactor = enemyData.dmgReductionFactor = 0;

        Debug.Log("I've lost my buff! My attacks now deal: " + EnemyDealDamage() + "dmg, and if an attack hits me that is " + dmgToTake + " dmg, I only take " + EnemyTakeDamage(dmgToTake));
    }

    public int EnemyTakeDamage(float damageToTake)
    {
        int finalDmgTaken = Mathf.RoundToInt(damageToTake - (damageToTake * enemyData.dmgReductionFactor));
        enemyData.enemyHealth -= finalDmgTaken;
        return finalDmgTaken;
    }

    public int EnemyDealDamage() 
    {
        return Mathf.RoundToInt(enemyData.enemyATKDmg + (enemyData.enemyATKDmg * enemyData.atkBoostFactor));
    }

    [Button("Dmg to Enemy")]
    public void DebugDealDamage() 
    {
        EnemyTakeDamage(dmgToTake);
        
    }
}

