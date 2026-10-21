using UnityEngine;

[CreateAssetMenu(fileName = "NewTarotScriptable", menuName = "Scriptables/Tarot")]
public class TarotScriptable : BaseScriptableObject
{
    public string LockedTooltip = $"Hand Not Dealt";

    public bool isUnlocked = false;

    public Color tempColor;
}
