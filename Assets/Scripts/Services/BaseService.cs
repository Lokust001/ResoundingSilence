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
}
