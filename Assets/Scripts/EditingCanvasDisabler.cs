/*
* Author: Tyler
* Contributors:
* Last Modified: 09/22/2026
* Summary: the only thing this does is turn off whatever its attached to so i dont have to keep turning on
*          and off my canvas in the editing ui scene
* To Do:   N/A
*/

using UnityEngine;

public class EditingCanvasDisabler : MonoBehaviour
{
    /// <summary>
    /// turns off the object its on
    /// </summary>
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
