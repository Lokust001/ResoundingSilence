/*
* Author: Tyler
* Contributors:
* Last Modified: 09/16/2026
* Summary: This is the base scriptable object from which every scriptable object should derive
* To Do:   Add more functions as needed.
*/

using UnityEngine;

public class BaseScriptableObject : ScriptableObject
{
    /// <summary>
    /// Creates a copy of the scriptable object
    /// </summary>
    /// <typeparam name="T"> The type of the scriptable object </typeparam>
    /// <returns> A copy of the current Scriptable Object </returns>
    /// <exception cref="System.Exception"> if you see this error the whole project is fucked </exception>
    public T CreateNonRefCopy<T>() where T : BaseScriptableObject
    {
        T copy = Instantiate((T)this);

        if (copy == null)
        {
            throw new System.Exception("Something went turbo wrong - " +
                "attempted to make a copy of myself but i dont exist?");
        }

        return copy;
    }
}
