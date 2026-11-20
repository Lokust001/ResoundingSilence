/*
* Author: Brenden
* Contributors:
* Last Modified: 11/20/2026
* Summary: Makes making more buff objects easier for the designers
* To Do:   N/A
*/
using UnityEngine;



[CreateAssetMenu(fileName = "New Buff Object", menuName = "Scriptables/New Buff Object")]
public class BuffObjectScriptable : BaseScriptableObject
{
    public BuffObject.PermStatBuff StatBuffed;
    public int Value;
}
