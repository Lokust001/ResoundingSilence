/*
* Author: Tyler
* Contributors: Brenden, Brad Dixon
* Last Modified: 11/19/2026
* Summary: stores the data for the tarot card.
* To Do:   N/A
*/


using UnityEngine;

[CreateAssetMenu(fileName = "NewTarotScriptable", menuName = "Scriptables/Tarot")]
public class TarotScriptable : BaseScriptableObject
{
    public string LockedTooltip = $"Hand Not Dealt";

    public bool isUnlocked = false;

    public Sprite sprite;

    public TarotCardType heldTarot;

}
