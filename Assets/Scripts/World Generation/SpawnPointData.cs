/*
* Author: Brenden
* Contributors:
* Last Modified: 09/18/2026
* Summary: Just to hold the data of what can spawn at each location
* To Do:   N/A
*/
using NUnit.Framework;
using UnityEngine;

public class SpawnPointData : MonoBehaviour
{
    [Tooltip("Can this location spawn a small environment space")]
    public bool SmallEnvironment;
    [Tooltip("Can this location spawn a medium environment space")]
    public bool MediumEnvironment;
    [Tooltip("Can this location spawn a large environment space")]
    public bool LargeEnvironment;
    [Tooltip("Can this location spawn a Buff Structure")]
    public bool BuffStructure;
    [Tooltip("Can this location spawn a POI")]
    public bool POI;
}
