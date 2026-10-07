using System.Collections.Generic;
using UnityEngine;

public class ControllerSupportedClickable : Clickable
{
    /// <summary>
    /// do not return base when overriding.
    /// </summary>
    /// <returns></returns>
    public virtual List<ControllerSupportedClickable> getNeighbors()
    {
        return new();
    }
}
