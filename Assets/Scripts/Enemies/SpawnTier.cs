using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnTier
{
    public string tierName;
    public GameObject[] enemyPool;

    public int minimumPoints;
}
