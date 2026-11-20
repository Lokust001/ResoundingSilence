/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/01/2026
* Summary: Creates a custom serializable variable/class for SpawnTiers
* To Do:   
*/
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpawnTier
{

    public string tierName;
    public EnemyGroup[] enemyGroups;

    public int minimumPoints;
}

[System.Serializable]
public class EnemyGroup
{
    public string groupName;
    public List<GameObject> enemies;
}