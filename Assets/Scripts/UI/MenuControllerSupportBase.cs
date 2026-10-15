using UnityEngine;

public class MenuControllerSupportBase : MenuBase

{
    [HideInInspector]
    public ControllerSupportedClickable carriedItem;

    public virtual void MoveSelectedItem(ControllerSupportedClickable target, int direction)
    {

    }

    public virtual void SelectDefault()
    {

    }
}
