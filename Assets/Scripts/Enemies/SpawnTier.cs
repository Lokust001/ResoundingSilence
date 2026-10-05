/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/01/2026
* Summary: Creates a custom serializable variable/class for SpawnTiers
* To Do:   
*/
using UnityEngine;
[System.Serializable]
public class SpawnTier
{
    public string tierName;
    public GameObject[] enemyPool;

    public int minimumPoints;
}
