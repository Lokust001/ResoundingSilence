/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/26/2026
 * Brief:
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AbilityData
{
    public string AbilityName;

    public enum Stats
    {
        Damage,
        DamageTicks,
        Duration,
        AggroRange,
        PlayerSpeed,
        WeaponSpeed,
        LifeSteal,
        AOESize,
        Delay,
        EnemyKnockback,
        PlayerKnockback
    }

    public List<StatValues> PrimaryStats = new List<StatValues>();
    public List<StatValues> SecondaryStats = new List<StatValues>();

    [HideInInspector] public int AbilityDamage;
    [HideInInspector] public int AbilityDamageTicks;
    [HideInInspector] public float AbilityDuration;
    [HideInInspector] public float AbilityAggroRange;
    [HideInInspector] public float AbilityPlayerSpeed;
    [HideInInspector] public float AbilityWeaponSpeed;
    [HideInInspector] public float AbilityLifeSteal;
    [HideInInspector] public float AbilityAOESize;
    [HideInInspector] public float AbilityDelay;
    [HideInInspector] public float AbilityEnemyKnockback;
    [HideInInspector] public float AbilityPlayerKnockback;

    public float AbilityCooldown;
    public float AbilityEndLag;
    public float AbilityRange;
    public bool HasPreview;
    public GameObject AbilityPreview;
    public LayerMask AbilityLayerMask;

    public AnimationClip AbilityAnim;
    public AnimationClip AbilityReadyAnim;
}

[System.Serializable]
public struct StatValues
{
    public AbilityData.Stats StatType;
    public float StatValue;
}
