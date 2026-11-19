/*
* Author: Tyler
* Contributors: Brad Dixon
* Last Modified: 11/9/2026
* Summary: Scriptable object for the abilities. Will havbe specific abilities derive from this as needed, but these 
*          vars are on every ability scriptable
* To Do:   N/A
*/

using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Scriptables/AbilityBase")]
public class AbilityBaseScriptable : BaseScriptableObject
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

    public Sprite AbilityImage;
    public Sprite BackgroundImage;

    public List<StatValues> PrimaryStats = new List<StatValues>();
    public List<StatValues> SecondaryStats = new List<StatValues>();

    [HideInInspector] public int AbilityDamage = 0;
    [HideInInspector] public int BaseAbilityDamage = 0;
    [HideInInspector] public int AbilityDamageTicks = 0;
    [HideInInspector] public int BaseAbilityDamageTicks = 0;
    [HideInInspector] public float AbilityDuration = 0;
    [HideInInspector] public float BaseAbilityDuration = 0;
    [HideInInspector] public float AbilityAggroRange = 0;
    [HideInInspector] public float BaseAbilityAggroRange = 0;
    [HideInInspector] public float AbilityPlayerSpeed = 0;
    [HideInInspector] public float BaseAbilityPlayerSpeed = 0;
    [HideInInspector] public float AbilityWeaponSpeed = 0;
    [HideInInspector] public float BaseAbilityWeaponSpeed = 0;
    [HideInInspector] public float AbilityLifeSteal = 0;
    [HideInInspector] public float BaseAbilityLifeSteal = 0;
    [HideInInspector] public float AbilityAOESize = 1;
    [HideInInspector] public float BaseAbilityAOESize = 1;
    [HideInInspector] public float AbilityDelay = 0;
    [HideInInspector] public float BaseAbilityDelay = 0;
    [HideInInspector] public float AbilityEnemyKnockback = 0;
    [HideInInspector] public float BaseAbilityEnemyKnockback = 0;
    [HideInInspector] public float AbilityPlayerKnockback = 0;
    [HideInInspector] public float BaseAbilityPlayerKnockback = 0;

    public float AbilityCooldown;
    public float AbilityEndLag;
    public float AbilityRange;

    public bool HasProjectile;
    [ShowIf(nameof(HasProjectile))]
    public BurstData AbilityProjectile;

    public GameObject AbilityVFX;

    public bool HasPreview;
    [ShowIf(nameof(HasPreview))]
    public PreviewShape PreviewData;

    public bool PersistingAbility;
    public bool CancelOnRecast;
    public LayerMask AbilityLayerMask;

    [HideInInspector] public Vector3 BaseAbilitySize;
    [HideInInspector] public Vector3 AbilityPos;

    public AnimationPath AbilityAnim;
    [ShowIf(nameof(HasPreview))]
    public AnimationPath AbilityReadyAnim;

    /// <summary>
    /// Set all stat values
    /// </summary>
    private void Awake()
    {
        SetAbilityStats(PrimaryStats);
        SetAbilityStats(SecondaryStats);

        if (AbilityProjectile.BurstRangedAttack.AttackProjectiles != null)
        {
            foreach (ProjectileData i in AbilityProjectile.BurstRangedAttack.AttackProjectiles)
            {
                if (i.LifetimeIsRange)
                {
                    i.ProjectileRange = AbilityRange;
                    i.BaseProjectileRange = AbilityRange;
                }
            }
        }
    }

    /// <summary>
    /// Sets the stats for each of the abilities
    /// </summary>
    virtual protected void SetAbilityStats(List<StatValues> stats)
    {
        foreach (StatValues value in stats)
        {
            switch (value.StatType)
            {
                case AbilityBaseScriptable.Stats.Damage:
                    AbilityDamage = Mathf.CeilToInt(value.StatValue);
                    BaseAbilityDamage = AbilityDamage;
                    break;
                case AbilityBaseScriptable.Stats.DamageTicks:
                    AbilityDamageTicks = Mathf.CeilToInt(value.StatValue);
                    BaseAbilityDamageTicks = AbilityDamageTicks;
                    break;
                case AbilityBaseScriptable.Stats.Duration:
                    AbilityDuration = value.StatValue;
                    BaseAbilityDuration = AbilityDuration;
                    break;
                //case AbilityBaseScriptable.Stats.AggroRange:
                //    AbilityAggroRange = value.StatValue;
                //    BaseAbilityAggroRange = AbilityAggroRange;
                //    break;
                //case AbilityBaseScriptable.Stats.PlayerSpeed:
                //    AbilityPlayerSpeed = value.StatValue;
                //    BaseAbilityPlayerSpeed = AbilityPlayerSpeed;
                //    break;
                //case AbilityBaseScriptable.Stats.WeaponSpeed:
                //    AbilityWeaponSpeed = value.StatValue;
                //    BaseAbilityWeaponSpeed = AbilityWeaponSpeed;
                //    break;
                case AbilityBaseScriptable.Stats.LifeSteal:
                    AbilityLifeSteal = value.StatValue;
                    BaseAbilityLifeSteal = AbilityLifeSteal;
                    break;
                case AbilityBaseScriptable.Stats.AOESize:
                    AbilityAOESize = value.StatValue;
                    BaseAbilityAOESize = AbilityAOESize;
                    break;
                case AbilityBaseScriptable.Stats.Delay:
                    AbilityDelay = value.StatValue;
                    BaseAbilityDelay = AbilityDelay;
                    break;
                case AbilityBaseScriptable.Stats.EnemyKnockback:
                    AbilityEnemyKnockback = value.StatValue;
                    BaseAbilityEnemyKnockback = AbilityEnemyKnockback;
                    break;
                case AbilityBaseScriptable.Stats.PlayerKnockback:
                    AbilityPlayerKnockback = value.StatValue;
                    BaseAbilityPlayerKnockback = AbilityPlayerKnockback;
                    break;
                default:
                    Debug.Log(value.StatType);
                    break;
            }
        }
    }
}

[System.Serializable]
public struct StatValues
{
    public AbilityBaseScriptable.Stats StatType;
    public float StatValue;
}

[System.Serializable]
public struct AnimationPath
{
    public AnimationClip AnimClip;
    public int LayerIndex;
    public Animator AnimatorUsed;
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

    //public GameObject AbilityAOE;
    public bool OrbitsPlayer;
    public bool SizeIsRange;
    public bool ScaleXOnly;
    public bool ScaleZOnly;
    public bool KnockBackFromPreviewCenter;
}
