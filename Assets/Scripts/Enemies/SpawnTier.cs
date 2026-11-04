/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/01/2026
* Summary: Creates a custom serializable variable/class for SpawnTiers
* To Do:   
*/
using System.Runtime.CompilerServices;
using UnityEngine;
[System.Serializable]
public class SpawnTier
{
    [System.Serializable]
    public class EnemyGroup
    {
        public string groupName;
        public GameObject[] enemies;
    }

    public string tierName;
    public EnemyGroup[] enemyGroups;

    public int minimumPoints;
}

