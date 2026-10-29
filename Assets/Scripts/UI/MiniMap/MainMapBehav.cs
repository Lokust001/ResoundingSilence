using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using NaughtyAttributes;

public class MainMapBehav : MonoBehaviour
{
    [SerializeField]
    private Transform visualBounds;

    [SerializeField]
    private Canvas mapCanvas;

    [Button]
    public void ResizeMap()
    {
        Vector3 size = visualBounds.GetComponent<Renderer>().bounds.size;

        mapCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        mapCanvas.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.z);
    }
}
