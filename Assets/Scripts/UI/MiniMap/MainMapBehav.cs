/*
* Author: Tyler
* Contributors:
* Last Modified: 09/29/2026
* Summary: resizes the image this has with the bounds of object 2. May move to utility functions later
* To Do:   N/A
*/

using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MainMapBehav : MonoBehaviour
{
    [SerializeField]
    private Transform visualBounds;

    [SerializeField]
    private Canvas mapCanvas;

    /// <summary>
    /// resizes the mapcanvas image using the visualbounds object
    /// </summary>
    [Button]
    public void ResizeMap()
    {
        Vector3 size = visualBounds.GetComponent<Renderer>().bounds.size;

        mapCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        mapCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.z);
    }
}
