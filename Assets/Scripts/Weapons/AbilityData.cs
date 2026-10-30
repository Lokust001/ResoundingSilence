/******************************************************************************
 * Author: Brad Dixon
 * Contributors:
 * Last Modified: 10/30/2026
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
        //AggroRange,
        //PlayerSpeed,
        //WeaponSpeed,
        LifeSteal,
        AOESize,
        Delay,
        EnemyKnockback,
        PlayerKnockback
    }

    public List<StatValues> PrimaryStats = new List<StatValues>();
    public List<StatValues> SecondaryStats = new List<StatValues>();

    [HideInInspector] public int AbilityDamage = 0;
    [HideInInspector] public int AbilityDamageTicks = 0;
    [HideInInspector] public float AbilityDuration = 0;
    [HideInInspector] public float AbilityAggroRange = 0;
    [HideInInspector] public float AbilityPlayerSpeed = 0;
    [HideInInspector] public float AbilityWeaponSpeed = 0;
    [HideInInspector] public float AbilityLifeSteal = 0;
    [HideInInspector] public float AbilityAOESize = 1;
    [HideInInspector] public float AbilityDelay = 0;
    [HideInInspector] public float AbilityEnemyKnockback = 0;
    [HideInInspector] public float AbilityPlayerKnockback = 0;

    public float AbilityCooldown;
    public float AbilityEndLag;
    public float AbilityRange;
    public bool HasPreview;
    public PreviewShape PreviewData;

    public bool PersistingAbility;
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
