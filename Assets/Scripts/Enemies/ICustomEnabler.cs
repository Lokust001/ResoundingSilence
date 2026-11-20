/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/05/2026
* Summary: Hosts the interfaces for ICustomEnabler and ICustomDisabler, which specifies custom enable and disable behavior
* To Do:   
*/
/// <summary>
/// Specifies a custom method to enable the gameobject without setting gameobject.SetActive(true)
/// </summary>
public interface ICustomEnabler
{
    /// <summary>
    /// A custom OnEnable method that relies on methods other than OnEnable()
    /// </summary>
    void EnableEntity();
}
/// <summary>
/// Specifies a custom method to disable the gameobject without setting gameobject.SetActive(false)
/// </summary>
public interface ICustomDisabler 
{
    /// <summary>
    /// A custom OnEnable method that relies on methods other than OnDisable()
    /// </summary>
    void DisableEntity();
}
