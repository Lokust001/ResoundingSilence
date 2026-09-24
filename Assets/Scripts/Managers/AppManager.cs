/*
* Author: Tyler
* Contributors:
* Last Modified: 09/17/2026
* Summary: Starts the app and ensures all managers are initialized properly.
* To Do:   N/A
*/


using NaughtyAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    private List<BaseManager> managers = new List<BaseManager>();
    private List<BaseManager> initializedManagers = new List<BaseManager>();

    public static AppManager Instance;

    /// <summary>
    /// try to spawn everything in
    /// </summary>
    /// <exception cref="System.Exception"></exception>
    private async void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        try
        {
            //spawns in all managers
            await SpawnManagers();
        }
        catch
        {
            throw new System.Exception("Failed to initialize");
        }

        //lets everything know that everything is spawned in
        GenericPublicEvents.AllManagersInitialized?.Invoke();
    }

    /// <summary>
    /// spawns in all of the managers
    /// </summary>
    /// <returns> if the task is completed or not </returns>
    private async Awaitable SpawnManagers()
    {
        foreach (BaseManager man in managers)
        {
            //if cancelled, stop here
            if (destroyCancellationToken.IsCancellationRequested)
            {
                return;
            }

            //spawns in the manager
            BaseManager b = Instantiate(man, transform);
            initializedManagers.Add(b);

            //initializes the manager
            await b.InitManager();
        }
    }

}
