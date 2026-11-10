/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 11/5/2026
* Summary: Serializable Reward object
*/
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class Reward
{
    public enum RewardType
    {
        Gold,
        XP,
        Weapon,
        Pin
    }
    /// <summary>
    /// If the RewardType is one that needs a GameObject instantiated
    /// </summary>
    /// <returns></returns>
    public bool DispensesGameObject() => type == RewardType.Weapon || type == RewardType.Pin;

    [Header("(Optional) for organization purposes")]
    public string rewardName = "Reward Entry";
    [Space(8), Header("(Required) Reward Type")]
    public RewardType type;

    [AllowNesting, ShowIf(nameof(DispensesGameObject))]
    public GameObject prefab;

    [Space(2), Header("% Drop & Amounts")]
    [Range(0, 100)]
    public int dropChance = 1;
    public int minDropAmt = 1;
    public int maxDropAmt = 1;
}
