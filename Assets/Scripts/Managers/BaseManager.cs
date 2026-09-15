using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [SerializeField]
    private List<BaseService> services = new List<BaseService>();

    private List<BaseService> initializedServices = new List<BaseService>();

    /// <summary>
    /// initializes the manager and all of the services that rely on this manager
    /// </summary>
    /// <returns> if the task is completed </returns>
    public async virtual Awaitable InitManager()
    {
        //if cancelled, stop here
        if (destroyCancellationToken.IsCancellationRequested)
        {
            return;
        }

        //if theres no services, dont spawn anything
        if (services.Count <= 0)
        {
            return;
        }

        //set up the container for the child services
        GameObject serviceContainer = new GameObject("ServicesContainer");
        serviceContainer.transform.position = Vector3.zero;
        serviceContainer.transform.parent = transform;

        foreach (BaseService service in services)
        {
            //if cancelled, stop here
            if (destroyCancellationToken.IsCancellationRequested)
            {
                return;
            }

            //make service gameobject
            BaseService servScript = Instantiate(service, serviceContainer.transform);
            initializedServices.Add(servScript);

            //init service
            await servScript.InitService();
        }
    }
}
