/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/20/2026
* Summary: Makes the entity drop loot when it is destroyed
*/

using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class LootDrop : MonoBehaviour
{
    private enum RollTypes 
    {
        IndependentRolls,
        WeightedRolls
    }

    [SerializeField]
    private RollTypes rollingMethod;

    [ShowIf(nameof(rollingMethod), RollTypes.WeightedRolls)]
    [Header("Weighted Roll Option")]
    [SerializeField]
    private int noDropsGivenWeight;

    [SerializeField]
    private List<Loot> possibleDrops;

    /// <summary>
    /// Disables this script if there are no drops
    /// </summary>
    private void Awake()
    {
        if(possibleDrops.Count == 0)
            this.enabled = false;
    }
    /// <summary>
    /// Selects a method to roll for loot based on user specification, assuming that drop chances and weights are properly defined
    /// </summary>
    private void RollForLoot() 
    {
        //TODO: Make a conditional if statement for pin drops for special logic
        switch (rollingMethod)
        {
            case RollTypes.IndependentRolls:
                RollIndependentLootTables();
                break;
            case RollTypes.WeightedRolls:
                RollWeightedLootDrops();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Loops through the loot table and rolls for each loot item independently. 
    /// Loot is acquired if the rolled value is within the range of the random number
    /// </summary>
    private void RollIndependentLootTables() 
    {
        List<GameObject> acquiredLoot = new();

        foreach(var loot in possibleDrops) 
        {
            float independentRoll = Random.Range(0, 101) * 0.01f;
            if (independentRoll <= loot.dropChance)
                acquiredLoot.Add(loot.lootPrefab);
        }

        DropLootOnGround(acquiredLoot);
    }

    /// <summary>
    /// Rolls one weight number which is calculate from adding the chances of all loot items.
    /// Loot is acquired if it falls into range of the weight
    /// </summary>
    private void RollWeightedLootDrops() 
    {
        List<GameObject> acquiredLoot = new();

        //Add the "no-drop" loot
        possibleDrops.Add(
            new Loot() 
            { weightedDropChance = noDropsGivenWeight }
        );

        //Weight of all items and "no-drop" loot
        int allDropsWeight = 0;

        foreach (var loot in possibleDrops) { allDropsWeight += loot.weightedDropChance; }

        //Roll the weighted loot as many times as there are unique loot
        for (int i = 0; i < possibleDrops.Count - 1; i++) 
        {
            int rolledWeight = Random.Range(0, allDropsWeight);

            foreach (var loot in possibleDrops)
            {
                if (rolledWeight <= loot.weightedDropChance)
                {
                    acquiredLoot.Add(loot.lootPrefab);
                    break;
                }
                
                rolledWeight -= loot.weightedDropChance; 
            }
        }

        //Remove "no-drop" loot
        acquiredLoot.RemoveAll(loot => loot == null);

        DropLootOnGround(acquiredLoot);
    }

    /// <summary>
    /// Drops/spawns loot at the position where the enemy dies
    /// </summary>
    /// <param name="objectsToInstantiate"></param>
    private void DropLootOnGround(List<GameObject> objectsToInstantiate) 
    {
        foreach (var prefab in objectsToInstantiate)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);

            if (prefab.name == "Pin")
                DroppedPin();
        }
    }

    /// <summary>
    /// TODO for Brenden: Add Pin logic
    /// </summary>
    private void DroppedPin() 
    {
        //TODO
    }

    /// <summary>
    /// Calls the loot dropping code when this enemy dies and starts to be destroyed
    /// </summary>
    private void OnDestroy()
    {
        RollForLoot();
    }
}

[System.Serializable]
public class Loot
{
    public string name;
    public GameObject lootPrefab;

    [Range(0.01f, 100f)]
    public float dropChance = 100f;
    public int weightedDropChance = 10;
}
