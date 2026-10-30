/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/28/2026
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
    public PreviewShape PreviewData;
    public LayerMask AbilityLayerMask;

    [HideInInspector] public Vector3 BaseAbilitySize;
    [HideInInspector] public Vector3 AbilityPos;

    public AnimationPath AbilityAnim;
    public AnimationPath AbilityReadyAnim;
}

[System.Serializable]
public struct StatValues
{
    public AbilityData.Stats StatType;
    public float StatValue;
}

[System.Serializable]
public struct AnimationPath
{
    public AnimationClip AnimClip;
    public int LayerIndex;
}

[System.Serializable]
public struct PreviewShape
{
    public enum Shape
    {
        Square,
        Sphere,
        Cone
    }

    public Shape ShapeType;

    public GameObject AbilityPreview;
    public bool OrbitsPlayer;
    public bool SizeIsRange;
    public bool ScaleXOnly;
    public bool ScaleZOnly;
}
