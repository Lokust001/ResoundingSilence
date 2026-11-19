

/*
* Author: Tyler
* Contributors:
* Last Modified: 11/19/2026
* Summary: Base class for all services to inherit from
* To Do:   N/A
*/

using System.Threading.Tasks;
using UnityEngine;

public class BaseService : MonoBehaviour
{
    /// <summary>
    /// Initializes the service and all that initialization requires.
    /// Is an empty virtual
    /// </summary>
    /// <returns></returns>
    public async virtual Awaitable InitService()
    {
        //if cancelled, stop here
        if (destroyCancellationToken.IsCancellationRequested)
        {
            return;
        }

        //done with func
        await Task.CompletedTask;
    }

    /// <summary>
    /// deconstructs this service
    /// </summary>
    /// <returns></returns>
    public async virtual Awaitable DeconstructService()
    {
        //if cancelled, stop here
        if (destroyCancellationToken.IsCancellationRequested)
        {
            return;
        }
        await Task.CompletedTask;
    }
}


