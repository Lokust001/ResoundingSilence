/*
* Author: Tyler
* Contributors: Brad Dixon, Brenden
* Last Modified: 11/9/2026
* Summary: This is the base scriptable object for all weapon scriptable objects.
*          Handles the data for the weapons. Acts as a tool to make any kind of weapon.
* To Do:   Add more variables as needed. Change status effects as needed. 
* Commented out variables don't have functionality yet. Will come in a later PR after controller support.
*/

using NaughtyAttributes;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewBaseWeapon", menuName = "Scriptables/Weapons/BaseWeapon")]
public class BaseWeaponScriptable : BaseScriptableObject
{
    #region GeneralData

    private enum EffectType
    {
        Decay,
        Slow,
        Weak,
        Shock,
        Burn
    }

    private enum ShownSettings
    {
        None,
        Lore,
        BaseWeaponData,
        StatusEffects,
        Abilities,
        UpgradeGrid
    }

    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Changing between the options changes what variables are shown in the inspector.")]
    private ShownSettings shownSettings;

    [Tooltip("The layers of things this weapon can hit.")]
    public LayerMask EnemyLayer;

    [ShowIf(nameof(shownSettings), ShownSettings.StatusEffects)]
    [SerializeField]
    [Tooltip("Changing this variable has no impact on gameplay.\n\nIt is purely a navigational tool." +
        "Use to select what type of status effects this weapon will inflict.")]
    private EffectType effectType;

    [ShowIf(nameof(shownSettings), ShownSettings.UpgradeGrid)]
    [Tooltip("This is the grid that will be attached to this kind of weapon when it is made")]
    public UpgradeTileGrid upgradeGrid;

    [Tooltip("This is used by the weapons that are dropped on the ground")]
    public GameObject weaponModel;

    [Tooltip("This is the prefab of the weapon to attach to the player")]
    public GameObject weaponBehavPrefab;

    #endregion

    #region Lore


    [ShowIf(nameof(shownSettings), ShownSettings.Lore)]
    [HorizontalLine(4, EColor.Green)]
    [Tooltip("This is the name that will appear on all of the UI")]
    public string WeaponName;

    #endregion

    #region Weapon

    //#region General Variables

    [HideInInspector] public float AttackSpeed = 1;
    [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    [ReorderableList]
    public List<AttackData> ComboAttacks = new List<AttackData>();

    #endregion

    #region del

    //   [HideInInspector] public List<int> WeaponDamage = new List<int>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    //public List<int> BaseWeaponDamage = new List<int>();

    //   [HideInInspector] public List<float> AttackCooldown = new List<float>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("How much damage the weapon does. Is a list in case the weapon has multiple hits in a combo.")]
    //public List<float> BaseAttackCooldown = new List<float>();

    //   //Defaults to 1 so the animation plays at normal speed.
    //   [HideInInspector] public float AttackSpeed = 1;

    //   [HideInInspector] public List<int> AttackBursts = new List<int>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("When making an attack, how many times the attack should occur. Is a list in case the weapon's combo attacks should cause different amount of bursts.")]
    //public List<int> BaseAttackBursts = new List<int>();

    //   [HideInInspector] public List<float> TimeBetweenBursts = new List<float>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("How much time passes between each burst. Set to 0 for weapons that unleash a burst of attacks at once. Is a list in case you want different timings for combo attacks.")]
    //public List<float> BaseTimeBetweenBursts = new List<float>();

    //   [HideInInspector] public List<float> BurstDamageAmount = new List<float>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("How much % damage the burst does compared to the base attack")]
    //public List<float> BaseBurstDamageAmount = new List<float>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("Whether or not a weapon has lifesteal.")]
    //public bool HasLifesteal;

    //   [ShowIf(nameof(WeaponLifeSteal))]
    //   [Tooltip("How much lifesteal a weapon has. Made as a list in case you want combo attacks to have multiple life steal values.")]
    //public List<float> BaseLifestealAmount = new List<float>();

    //   [HideInInspector] public List<float> LifestealAmount = new List<float>();

    //   [ShowIf(nameof(shownSettings), ShownSettings.BaseWeaponData)]
    //   [Tooltip("If the weapon has an AOE attack attached.")]
    //public bool HasAOEAttack;

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("What the weapon's AOE looks like. List in case we want multiple different kinds.")]
    //public List<GameObject> WeaponAOEs = new List<GameObject>();

    //   [HideInInspector] public List<int> AOEDamage = new List<int>();

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("The most amount of damage the AOE can do.")]
    //public List<int> BaseAOEDamage = new List<int>();

    //   [HideInInspector] public List<float> AOESize = new List<float>();

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("How big the AOE is.")]
    //public List<float> BaseAOESize = new List<float>();

    //   [HideInInspector] public List<float> AOEDuration = new List<float>();

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("How long the AOE lasts.")]
    //public List<float> BaseAOEDuration = new List<float>();

    //   [HideInInspector] public List<int> AOEDamageTicks = new List<int>();

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("How many times the AOE should damage.")]
    //public List<int> BaseAOEDamageTicks = new List<int>();

    //   [HideInInspector] public List<float> AOEDelay = new List<float>();

    //   [ShowIf(nameof(WeaponAOE))]
    //   [Tooltip("How much of a delay there is before the AOE goes off. 0 is instant.")]
    //public List<float> BaseAOEDelay = new List<float>();

    //   #endregion

    //   #region Ranged Variables

    //   [HideInInspector]
    //   public List<GameObject> Projectiles = new List<GameObject>();

    //   [ShowIf(nameof(RangedWeaponSettings))]
    //   [Tooltip("What kind of projectile the weapon fires. A list in case the weapon can fire multiple types of projectiles.")]
    //   public List<GameObject> BaseProjectiles = new List<GameObject>();

    //   [HideInInspector]
    //   public List<float> ProjectileSpeed = new List<float>();

    //   [ShowIf(nameof(RangedWeaponSettings))]
    //   [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile speeds in a combo.")]
    //   public List<float> BaseProjectileSpeed = new List<float>();

    //   [HideInInspector]
    //   public List<float> ProjectileLifetime = new List<float>();

    //   [ShowIf(nameof(RangedWeaponSettings))]
    //   [Tooltip("How fast a projectile flies. Is a list in case the weapon has multiple projectile lifetimes in a combo.")]
    //   public List<float> BaseProjectileLifetime = new List<float>();

    //   [ShowIf(nameof(RangedWeaponSettings))]
    //   [Tooltip("Whether or not the weapon's attacks have pierce.")]
    //   public bool HasPierce;

    //   [HideInInspector]
    //   public List<float> PierceDamageFalloff = new List<float>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    //   [Tooltip("How much damage is lost after piercing a target. Is a list in case you want pierce damage fall off to not be linear.")]
    //   public List<float> BasePierceDamageFalloff = new List<float>();

    //   [HideInInspector]
    //   public List<float> PierceLifetimeFalloff = new List<float>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    //   [Tooltip("How much lifetime is lost after piercing a target. Is a list in case you want pierce lifetime fall off to not be linear.")]
    //   public List<float> BasePierceLifetimeFalloff = new List<float>();

    //   [HideInInspector]
    //   public List<int> PierceAmount = new List<int>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    //   [Tooltip("Set if you want to cap how many enemies the projectile can pierce through. Set to -1 if infinite. " +
    //       "Made as a list in case you want combo attacks to pierce a different amount of enemies.")]
    //   public List<int> BasePierceAmount = new List<int>();

    //   [HideInInspector]
    //   public List<int> MinPierceDamage = new List<int>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(HasPierce))]
    //   [Tooltip("The minimum amount of damage the weapon can be decreased to from piercing. If <= 0, will just destroy when damage is 0. " +
    //       "Made as a list in case you want combo attacks to have varying minimum pierce damage.")]
    //   public List<int> BaseMinPierceDamage = new List<int>();

    //   [ShowIf(nameof(RangedWeaponSettings))]
    //   [Tooltip("Whether or not the projectile ricochets on hit.")]
    //   public bool Ricochets;

    //   [HideInInspector] public List<int> RicochetCount = new List<int>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(Ricochets))]
    //   [Tooltip("How many times the projectile can ricochet.")]
    //   public List<int> BaseRicochetCount = new List<int>();

    //   [HideInInspector] public List<float> RicochetRange = new List<float>();

    //   [ShowIf(EConditionOperator.And, nameof(RangedWeaponSettings), nameof(Ricochets))]
    //   [Tooltip("How close enemies have to be for a ricochet to happen.s")]
    //   public List<float> BaseRicochetRange = new List<float>();
    //   #endregion

    //#endregion

    #endregion

    #region StatusEffects

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How much damage the decay does each time it ticks.")]
    public int DecayDamage;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("The max amount of decay stacks an enemy can have.")]
    public int MaxDecayStacks;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How much time must pass before the next tick of decay damage occurs.")]
    public float DecayDelay;

    [ShowIf(nameof(ViewingDecayEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float DecayDuration;

    [ShowIf(nameof(ViewingSlowEffect))]
    [Tooltip("How much the enemy is slowed by.")]
    public float SlowStrength;

    [ShowIf(nameof(ViewingSlowEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float SlowDuration;

    [ShowIf(nameof(ViewingWeakEffect))]
    [Tooltip("How much the enemy's attack is reduced by.")]
    public float WeakStrength;

    [ShowIf(nameof(ViewingWeakEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float WeakDuration;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much damage the shock does each time it ticks.")]
    public int ShockDamage;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("The max amount of shock stacks an enemy can have.")]
    public int MaxShockStacks;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much time must pass before the next tick of shock damage occurs.")]
    public float ShockDelay;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float ShockDuration;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How much time must pass before the effect chains damage to nearby enemies.")]
    public float ChainDelay;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("How close enemies have to be to each other for the chain to connect.")]
    public float ChainRange;

    [ShowIf(nameof(ViewingShockEffect))]
    [Tooltip("The max amount of enemies that can be hit by one chain.")]
    public int MaxChainCount;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much damage the shock does each time it ticks.")]
    public int BurnDamage;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("The max amount of shock stacks an enemy can have.")]
    public int MaxBurnStacks;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much time must pass before the next tick of shock damage occurs.")]
    public float BurnDelay;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How long the effect lasts for.")]
    public float BurnDuration;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much time must pass before the effect tries to ignite the ground.")]
    public float IgniteDelay;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How big the AOE of the burning ground is.")]
    public float IgniteSize;

    [ShowIf(nameof(ViewingBurnEffect))]
    [Tooltip("How much the ignite chance increase by per stack.")]
    public int IgniteChancePerStack;

    #endregion

    #region Abilities

    [ShowIf(nameof(shownSettings), ShownSettings.Abilities)]
    [Expandable]
    [SerializeField]
    private List<AbilityBaseScriptable> abilities = new();
    [HideInInspector]
    public List<AbilityBaseScriptable> editableAbilities = new();

    
    /// <summary>
    /// Initializes the editableabilities list if it hasnt already been ititialized. Returns the list of editable abilities
    /// </summary>
    /// <returns></returns>
    public List<AbilityBaseScriptable> GetAbilities()
    {
        if (editableAbilities.Count <= 0)
        {
            foreach (AbilityBaseScriptable ability in abilities)
            {
                editableAbilities.Add(ability.CreateNonRefCopy<AbilityBaseScriptable>());
            }
        }

        return editableAbilities;
    }
    #endregion

    #region CustomBools

    /// <summary>
    /// Custom bool for naughty attribute show if
    /// </summary>
    /// <returns></returns>
    //private bool WeaponLifeSteal()
    //{
    //    return shownSettings == ShownSettings.BaseWeaponData && HasLifesteal;
    //}

    ///// <summary>
    ///// Custom bool to show if AOE attack is selected
    ///// </summary>
    ///// <returns></returns>
    //private bool WeaponAOE()
    //{
    //    return shownSettings == ShownSettings.BaseWeaponData && HasAOEAttack;
    //}

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingDecayEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Decay;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingSlowEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Slow;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingWeakEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Weak;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingShockEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Shock;
    }

    /// <summary>
    /// Custom bool for show if
    /// </summary>
    /// <returns></returns>
    private bool ViewingBurnEffect()
    {
        return shownSettings == ShownSettings.StatusEffects && effectType == EffectType.Burn;
    }
    #endregion

    #region Player Modifiers
    PlayerController playerController = null;
    float DashBuff;
    float DamageResistance;
    float MoveSpeedBuff;
    float HealingPotency;
    float knockbackAmount;
    float regenAmount;
    #endregion

    /// <summary>
    /// Sets weapon variables to its base values
    /// </summary>
    public void Awake()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        foreach(AttackData i in ComboAttacks)
        {
            i.BaseWeaponDamage = i.WeaponDamage;
            i.BaseAttackCooldown = i.AttackCooldown;

            foreach(BurstData j in i.BurstAttacks)
            {
                j.BaseTimeTillNextBurst = j.TimeTillNextBurst;
                j.WeaponLifeSteal.BaseLifestealPercent = j.WeaponLifeSteal.LifestealPercent;
                j.WeaponAoe.BaseAoeDamagePercent = j.WeaponAoe.AoeDamagePercent;
                j.WeaponAoe.BaseAoeDamageTicks = j.WeaponAoe.AoeDamageTicks;
                j.WeaponAoe.BaseAoeDelay = j.WeaponAoe.AoeDelay;
                j.WeaponAoe.BaseAoeDuration = j.WeaponAoe.AoeDuration;
                j.WeaponAoe.BaseAoeSize = j.WeaponAoe.AoeSize;
                j.WeaponAoe.BaseMinAoeDamagePercent = j.WeaponAoe.MinAoeDamagePercent;
                j.BurstRangedAttack.BurstPierce.BaseMinPierceDamagePercent = j.BurstRangedAttack.BurstPierce.MinPierceDamagePercent;
                j.BurstRangedAttack.BurstPierce.BasePierceAmount = j.BurstRangedAttack.BurstPierce.PierceAmount;
                j.BurstRangedAttack.BurstPierce.BasePierceDamageFalloffPercent = j.BurstRangedAttack.BurstPierce.PierceDamageFalloffPercent.ToList();
                j.BurstRangedAttack.BurstPierce.BasePierceLifetimeFalloffPercent = j.BurstRangedAttack.BurstPierce.PierceLifetimeFalloffPercent.ToList();
                j.BurstRangedAttack.BurstRicochet.BaseRicochetCount = j.BurstRangedAttack.BurstRicochet.RicochetCount;
                j.BurstRangedAttack.BurstRicochet.BaseRicochetRange = j.BurstRangedAttack.BurstRicochet.RicochetRange;
                j.BurstRangedAttack.ProjectileIndex = 0;

                foreach(ProjectileData k in j.BurstRangedAttack.AttackProjectiles)
                {
                    k.BaseProjectileDeviation = k.ProjectileDeviation;
                    k.BaseProjectileLifetime = k.ProjectileLifetime;
                    k.BaseProjectileSpeed = k.ProjectileSpeed;
                    k.BaseSpreadDegree = k.SpreadDegree;
                    k.BaseTotalProjectiles = k.TotalProjectiles;
                    k.BaseProjectileRange = k.ProjectileRange;
                    k.ProjectileSizeChange.BaseNewSize = k.ProjectileSizeChange.NewSize;
                    k.ProjectileSizeChange.BaseSizeChangePercentage = k.ProjectileSizeChange.SizeChangePercentage;
                    k.ProjectileSizeChange.BaseTimeTillNewSize = k.ProjectileSizeChange.TimeTillNewSize;
                    k.ProjectileSizeChange.BaseTimeTillSizeChange = k.ProjectileSizeChange.TimeTillSizeChange;
                }
            }
        }
        upgradeGrid.attachedWeapon = this;
    }

    #region Pin Functions
    /// <summary>
    /// changes weapon damage based on the amount of damage buffs given to the weapon on the grid
    /// </summary>
    /// <param name="DamageBuff">Greater than one multiplies the base damage numbers</param>
    public void UpdateWeaponDamage(float DamageBuff)
    {
        foreach(AttackData i in ComboAttacks)
        {
            i.WeaponDamage = Mathf.CeilToInt(i.BaseWeaponDamage * DamageBuff);
        }
    }

    /// <summary>
    /// changes weapon attack speed based on the amount of dpeed buffs given to the weapon on the grid
    /// </summary>
    /// <param name="SpeedBoost">Less than one, multiplies the base attack cooldown</param>
    public void UpdateWeaponSpeed(float SpeedBoost)
    {
        AttackSpeed = AttackSpeed / SpeedBoost;

        foreach(AttackData i in ComboAttacks)
        {
            i.AttackCooldown = i.BaseAttackCooldown * SpeedBoost;
        }
    }

    /// <summary>
    /// changes how much lifesteal the weapon has based on the parameter. Parameter should be a percentage
    /// </summary>
    /// <param name="lifestealPercent"></param>
    public void UpdateLifestealPercent(float lifestealPercent)
    {
        foreach(AttackData i in ComboAttacks)
        {
            foreach(BurstData j in i.BurstAttacks)
            {
                //If adding lifesteal, let the attack know if has lifesteal now
                if(!j.HasLifesteal)
                {
                    j.HasLifesteal = true;
                    j.WeaponLifeSteal.LifestealPercent = lifestealPercent;
                    continue;
                }

                j.WeaponLifeSteal.LifestealPercent = j.WeaponLifeSteal.BaseLifestealPercent * lifestealPercent;
            }
        }
    }

    /// <summary>
    /// changes the amount of damage the abilities 
    /// </summary>
    /// <param name="DamageBuff1"></param>
    /// <param name="DamageBuff2"></param>
    public void UpdateAbilityDamage(float DamageBuff)
    {
        //editableAbilities[0].damage = editableAbilities[0].baseDamage * DamageBuff;
        //editableAbilities[1].damage = editableAbilities[1].baseDamage * DamageBuff;
    }

    /// <summary>
    /// Changes the cooldown of the abilities
    /// </summary>
    /// <param name="DamageBuff1"></param>
    /// <param name="DamageBuff2"></param>
    public void UpdateAbilityCooldown(float CooldownReduction)
    {
        //editableAbilities[0].Cooldown = editableAbilities[0].baseCooldown * CooldownReduction;
        //editableAbilities[1].Cooldown = editableAbilities[1].baseCooldown * CooldownReduction;
    }

    /// <summary>
    /// adds a burst to the attack and adjusts burst damage
    /// </summary>
    /// <param name="percentDamage"></param>
    public void AddBurst(int percentDamage, int burstAttackIndex = 0)
    {
        foreach(AttackData i in ComboAttacks)
        {
            i.BurstAttacks.Add(i.BurstAttacks[burstAttackIndex]);

            foreach(BurstData j in i.BurstAttacks)
            {
                j.BurstDamagePercent += percentDamage;
            }
        }
    }

    /// <summary>
    /// adds speed to the projectiles
    /// </summary>
    /// <param name="speedBoost"></param>
    public void ProjectileSpeedBoost(float speedBoost)
    {
        foreach(AttackData i in ComboAttacks)
        {
            foreach(BurstData j in i.BurstAttacks)
            {
                foreach(ProjectileData k in j.BurstRangedAttack.AttackProjectiles)
                {
                    k.ProjectileSpeed = k.BaseProjectileSpeed * speedBoost;
                }
            }
        }
    }

    /// <summary>
    /// takes and stores the player modifiers
    /// </summary>
    /// <param name="dash"></param>
    /// <param name="DR"></param>
    /// <param name="moveSpeed"></param>
    /// <param name="healing"></param>
    /// <param name="kb"></param>
    /// <param name="regen"></param>
    public void getPlayerModifiers(float dash, float DR, float moveSpeed, float healing, float kb, float regen)
    {
        DashBuff = dash;
        DamageResistance = DR;
        MoveSpeedBuff = moveSpeed;
        HealingPotency = healing;
        knockbackAmount = kb;
        regenAmount = regen;
    }

    /// <summary>
    /// sends the player modifiers to the player
    /// </summary>
    public void sendPlayerModifiers()
    {
        playerController.DashDistance(DashBuff);
        playerController.DamageResistanceChange(DamageResistance);
        playerController.MoveSpeedIncrease(MoveSpeedBuff);
        playerController.healingPotencyChange(HealingPotency);
        playerController.KnockbackResistance(knockbackAmount);
        playerController.RegenBoost(regenAmount);
    }
    #endregion
}

#region Structs

[System.Serializable]
public class AttackData
{
    public int WeaponDamage;
    [HideInInspector] public int BaseWeaponDamage;
    public float AttackCooldown;
    [HideInInspector] public float BaseAttackCooldown;
    [Tooltip("IMPORTANT!!! Make sure burst attacks has at least one entry!")]
    [ReorderableList]
    public List<BurstData> BurstAttacks;
    [Tooltip("How long you have in seconds to attack again to continue the combo. " +
        "If no combo or end of combo, doesn't do anything.")]
    public float ComboTime;
}

[System.Serializable]
public class BurstData
{
    [Tooltip("If you want the bursts to do the same amount of damage, you can leave this at 0.")]
    public int BurstDamagePercent;

    [Tooltip("How much time in seconds elapses before the next attack in the burst happens. Does nothing if the final entry.")]
    public float TimeTillNextBurst;
    [HideInInspector] public float BaseTimeTillNextBurst;

    public bool HasLifesteal;
    [ShowIf(nameof(HasLifesteal)), AllowNesting]
    public LifestealData WeaponLifeSteal;

    public bool HasAoe;
    [ShowIf(nameof(HasAoe)), AllowNesting]
    public AoeData WeaponAoe;

    public bool HasRangeAttack;
    [ShowIf(nameof(HasRangeAttack)), AllowNesting]
    public RangedData BurstRangedAttack;
}

[System.Serializable]
public class LifestealData
{
    public float LifestealPercent;
    [HideInInspector] public float BaseLifestealPercent;
}

[System.Serializable]
public class AoeData
{
    public GameObject AoePrefab;

    public int AoeDamagePercent;
    [HideInInspector] public int BaseAoeDamagePercent;

    [Tooltip("If true, the aoe does less damage the further an enemy is from the center.")]
    public bool HasDamageFalloff;
    [ShowIf(nameof(HasDamageFalloff)), AllowNesting]
    public int MinAoeDamagePercent;

    [HideInInspector] public int BaseMinAoeDamagePercent;
    public float AoeSize;

    [HideInInspector] public float BaseAoeSize;
    public float AoeDelay;

    [HideInInspector] public float BaseAoeDelay;
    public float AoeDuration;
    [HideInInspector] public float BaseAoeDuration
        ;
    [ShowIf(nameof(HasDuration)), AllowNesting]
    public int AoeDamageTicks;
    [HideInInspector] public int BaseAoeDamageTicks;

    /// <summary>
    /// Custom show if bool to see if the aoe has a duration
    /// </summary>
    /// <returns></returns>
    private bool HasDuration()
    {
        return AoeDuration > 0;
    }
}

[System.Serializable]
public class RangedData
{
    [Tooltip("If empty, will be hitscan.")]
    public List<ProjectileData> AttackProjectiles;

    [HideInInspector] public int ProjectileIndex;

    [ShowIf(nameof(MultipleProjectiles)), AllowNesting]
    public bool RandomizeProjectile;

    //[ShowIf(nameof(MultipleProjectiles)), AllowNesting]
    //public bool FireAllProjectiles;

    public bool HasPierce;

    [ShowIf(nameof(HasPierce)), AllowNesting]
    public PierceData BurstPierce;

    public bool HasRicochet;

    [ShowIf(nameof(HasRicochet)), AllowNesting]
    public RicochetData BurstRicochet;

    /// <summary>
    /// Custom show if bool to determine if there are more than one projectile prefab
    /// </summary>
    /// <returns></returns>
    private bool MultipleProjectiles()
    {
        if (AttackProjectiles != null)
        {
            return AttackProjectiles.Count > 1;
        }
        return false;
    }
}

[System.Serializable]
public class ProjectileData
{
    [Tooltip("The type of projectile that gets fired in this part of the burst.")]
    public GameObject ProjectilePrefab;

    public float ProjectileSpeed;
    [HideInInspector] public float BaseProjectileSpeed;

    public bool LifetimeIsRange;

    [ShowIf(nameof(LifetimeIsRange)), AllowNesting]
    public float ProjectileRange;
    [HideInInspector] public float BaseProjectileRange;

    [HideIf(nameof(LifetimeIsRange)), AllowNesting]
    public float ProjectileLifetime;
    [HideInInspector] public float BaseProjectileLifetime;

    public int ProjectileDeviation;
    [HideInInspector] public int BaseProjectileDeviation;

    public int TotalProjectiles;
    [HideInInspector] public int BaseTotalProjectiles;

    [ShowIf(nameof(NeedSpread)), AllowNesting]
    public int SpreadDegree;
    [HideInInspector] public int BaseSpreadDegree;

    public bool HasSizeChange;
    [ShowIf(nameof(HasSizeChange)), AllowNesting]
    public SizeChangeData ProjectileSizeChange;

    /// <summary>
    /// Custom show if bool to determine if the projectiles have spread
    /// </summary>
    /// <returns></returns>
    private bool NeedSpread()
    {
        return TotalProjectiles > 1;
    }
}

[System.Serializable]
public class PierceData
{
    public int PierceAmount;
    [HideInInspector] public int BasePierceAmount;

    public List<int> PierceDamageFalloffPercent;
    [HideInInspector] public List<int> BasePierceDamageFalloffPercent;

    [ShowIf(nameof(HasFalloff)), AllowNesting]
    public int MinPierceDamagePercent;
    [HideInInspector] public int BaseMinPierceDamagePercent;

    public List<int> PierceLifetimeFalloffPercent;
    [HideInInspector] public List<int> BasePierceLifetimeFalloffPercent;

    /// <summary>
    /// Custom bool to determine if there is damage falloff for pierce
    /// </summary>
    /// <returns></returns>
    private bool HasFalloff()
    {
        if(PierceDamageFalloffPercent != null)
        {
            foreach(int i in PierceDamageFalloffPercent)
            {
                if(i > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

[System.Serializable]
public class RicochetData
{
    public int RicochetCount;
    [HideInInspector] public int BaseRicochetCount;

    public float RicochetRange;
    [HideInInspector] public float BaseRicochetRange;
}

[System.Serializable]
public class SizeChangeData
{
    public bool InfinitelyChangeSize;

    [HideIf(nameof(InfinitelyChangeSize)), AllowNesting]
    public float NewSize;
    [HideInInspector] public float BaseNewSize;

    [HideIf(nameof(InfinitelyChangeSize)), AllowNesting]
    public float TimeTillNewSize;
    [HideInInspector] public float BaseTimeTillNewSize;

    [ShowIf(nameof(InfinitelyChangeSize)), AllowNesting]
    public float SizeChangePercentage;
    [HideInInspector] public float BaseSizeChangePercentage;

    [ShowIf(nameof(InfinitelyChangeSize)), AllowNesting]
    public float TimeTillSizeChange;
    [HideInInspector] public float BaseTimeTillSizeChange;

    [Tooltip("Set true if you don't want the x scale to change.")]
    public bool ConstXSize;
    [Tooltip("Set true if you don't want the y scale to change.")]
    public bool ConstYSize;
    [Tooltip("Set true if you don't want the z scale to change.")]
    public bool ConstZSize;

    [ShowIf(EConditionOperator.And, nameof(InfinitelyChangeSize), nameof(SetMinSize)), AllowNesting]
    [Tooltip("If you don't want a min size, leave at 0.")]
    public float MinSize;

    [ShowIf(EConditionOperator.And, nameof(InfinitelyChangeSize), nameof(SetMaxSize)), AllowNesting]
    [Tooltip("If you don't want a max size, leave at 0.")]
    public float MaxSize;

    /// <summary>
    /// Custom show if bool that determines if the size will go infinietely down
    /// </summary>
    /// <returns></returns>
    private bool SetMinSize()
    {
        return SizeChangePercentage < 1;
    }

    /// <summary>
    /// Custom show if bool that determines if the size will go infinietely up
    /// </summary>
    /// <returns></returns>
    private bool SetMaxSize()
    {
        return SizeChangePercentage >= 1;
    }
}

#endregion
