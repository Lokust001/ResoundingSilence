using UnityEngine;

public class EditingCanvasDisabler : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
