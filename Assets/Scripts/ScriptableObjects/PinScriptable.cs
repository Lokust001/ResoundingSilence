/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: Scriptable object for the pins. May need to make children later on depending on how complex the codebase gets
* To Do:   N/A
*/

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
[CreateAssetMenu(fileName = "NewPin", menuName = "Scriptables/New Pin")]
public class PinScriptable : BaseScriptableObject
{
    public enum PinType
    {
        none,
        Power,
        Vitality,
        Speed,
        Status
    }

    public enum WeaponStatToChange
    {
        none,
        BulletDamage,
        AttackSpeed,
        addDebuff,
        Lifesteal
    }

    [HorizontalLine(4, EColor.Red)]
    [Tooltip("The name of the pin")]
    public string PinName;

    [HorizontalLine(4, EColor.Indigo)]
    [Header("Pin Gameplay Values")]
    [Tooltip("The type of pin this is")]
    public List<PinType> Types;

    [Tooltip("this is the types that can give an adjacent bonus")]
    public List<PinType> AutoAdjacencyPinTypes;

    [Tooltip("This should add up to at least 8")]
    public List<int> autoAdjacencyAmounts;

    public List<PinType> NorthAdjacency;
    public List<PinType> NorthEastAdjacency;
    public List<PinType> EastAdjacency;
    public List<PinType> SouthEastAdjacency;
    public List<PinType> SouthAdjacency;
    public List<PinType> SouthWestAdjacency;
    public List<PinType> WestAdjacency;
    public List<PinType> NorthWestAdjacency;
    public bool ManualAdjacency;

    [Tooltip("This is numerical value for how much to modify in %")]
    public float ModifierNumber;

    [Tooltip("How you want to change the weapon")]
    public WeaponStatToChange StatToChange;

    [Tooltip("How much (in %) this gains from having adjacent tiles be the same type")]
    public int AdjacentTileScaling;

    [Tooltip("If true the scaling number will be added per adjacent tile that matches the type, if false it will multiply the base modifier by 1 + the percent in adjacent scaling")]
    public bool AdditiveScaling = true;

    [HorizontalLine(4, EColor.Blue)]
    [Header("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    [Tooltip("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    public Color pinColor;

    /// <summary>
    /// Will make the adjacency with how many of each type specified,needs to be called when the player picks up the pin
    /// </summary>
    public void AutoMakeAdjacency()
    {
        if (ManualAdjacency)
            return;
        int amountOfAdjacencyAmounts = 0;
        foreach(int amount in autoAdjacencyAmounts)
        {
            amountOfAdjacencyAmounts += amount;
        }

        int startNumber = Random.Range(0, 8);
        int loopedTimes = 0;
        for(int i = startNumber; i < startNumber + 8; i++)
        {
            PinType thisType = PinType.none;
            int existance = loopedTimes;
            for(int j = 0; j < autoAdjacencyAmounts.Count; j++)
            {
                if(existance <= autoAdjacencyAmounts[j])
                {
                    thisType = AutoAdjacencyPinTypes[j];
                    break;
                }
                existance -= autoAdjacencyAmounts[j];
            }
            switch(startNumber % 8)
            {
                case 0:
                    NorthAdjacency.Add(thisType);
                    continue;
                case 1:
                    NorthEastAdjacency.Add(thisType);
                    continue;
                case 2:
                    EastAdjacency.Add(thisType);
                    continue;
                case 3:
                    SouthEastAdjacency.Add(thisType);
                    continue;
                case 4:
                    SouthAdjacency.Add(thisType);
                    continue;
                case 5:
                    SouthWestAdjacency.Add(thisType);
                    continue;
                case 6:
                    WestAdjacency.Add(thisType);
                    continue;
                case 7:
                    NorthWestAdjacency.Add(thisType);
                    continue;
            }
        }
    }
}
