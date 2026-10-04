using UnityEngine;

public interface ICustomEnabler
{
    /// <summary>
    /// A custom OnEnable method that relies on methods other than OnEnable()
    /// </summary>
    void EnableEntity();
}

public interface ICustomDisabler 
{
    /// <summary>
    /// A custom OnEnable method that relies on methods other than OnDisable()
    /// </summary>
    void DisableEntity();
}
