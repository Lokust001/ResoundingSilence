/*
* Author: Brenden
* Contributors:
* Last Modified: 09/21/2026
* Summary: Holds and stores the data of each island
* To Do:   N/A
*/
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class IslandData : MonoBehaviour
{
    public List<GameObject> POISpawns;
    public List<GameObject> FastTravelPoints;
    public GameObject realFastTravel;

    public void ChooseFastTravel()
    {
        realFastTravel = FastTravelPoints[Random.Range(0, FastTravelPoints.Count)];
        realFastTravel.SetActive(true);
    }
}
