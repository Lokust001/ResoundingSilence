/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 09/18/2026
* Summary: This is the base scriptable object for all eneny scriptable objects.
* To Do:   Add more variables as needed.
*/
using UnityEngine;
using NaughtyAttributes;
[CreateAssetMenu(fileName = "BaseEnemyScriptable", menuName = "Scriptable Objects/BaseEnemyScriptable")]
public class BaseEnemyScriptable : BaseScriptableObject
{
    public enum EnemyType
    {
        SingleShooter,
        ConeShooter
    }
    [SerializeField]
    public EnemyType enemyType;
    private bool isShooter() 
    {
        return enemyType == EnemyType.SingleShooter || enemyType == EnemyType.ConeShooter;
    }

    [SerializeField, Header("General Attributes")]
    public string enemyName;
    [SerializeField]
    public float enemyHealth;
    [SerializeField]
    public float timeBetweenAttacks;

    [ShowIf(nameof(isShooter))]
    [Header("All Shooter Attributes")]
    public float bulletTravelSpeed;
    [ShowIf(nameof(isShooter))]
    [SerializeField]
    public float bulletLife;
    [ShowIf(nameof(isShooter))]
    public GameObject bulletPrefab;

    [Header("Cone Shooter Attributes")]
    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    public int bulletsToFire;

    [ShowIf(nameof(enemyType), EnemyType.ConeShooter)]
    public float bulletSpreadRadiusDegrees;
    
}
