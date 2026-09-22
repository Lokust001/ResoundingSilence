/*
* Author: Tyler
* Contributors:
* Last Modified: 09/21/2026
* Summary: The scriptable object for the glyphs
* To Do:   N/A
*/

using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGlyph", menuName = "Scriptables/New Glyph")]
public class GlyphScriptable : BaseScriptableObject
{

    [Header("Gameplay Variables")]
    [HorizontalLine(4, EColor.Indigo)]
    [Tooltip("The type of pin that has to be placed on this for this glyph to work.")]
    public PinScriptable.PinType PinTypeToModify;

    [Tooltip("how to modify the glyph above")]
    public float HowToModifyPin;

    [HorizontalLine(4, EColor.Blue)]
    [Header("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    [Tooltip("TEMPORARY - REPLACE WITH SPRITE WHEN I GET THEM")]
    public Color glyphColor;
}
