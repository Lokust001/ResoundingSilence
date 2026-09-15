/*
* Author: Tyler
* Contributors:
* Last Modified: 09/15/2026
* Summary: Starts the app and ensures all managers are initialized properly.
* To Do:   N/A
*/


using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        //can be removed once input testing is not needed.
        GenericPublicEvents.AllManagersInitialized += EnableInputTesting;

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

            //lets everything know that everything is spawned in
            GenericPublicEvents.AllManagersInitialized?.Invoke();
        }
        catch
        {
            throw new System.Exception("Failed to initialize");
        }
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

    #region TEMPORARY

    /// <summary>
    /// WILL BE REMOVED
    /// 
    /// only temporary to test the input system.
    /// </summary>
    private void EnableInputTesting()
    {
        InputPublicEvents.MovePressed += TestMovePressed;
        InputPublicEvents.MoveReleased += TestMoveCancelled;
        InputPublicEvents.ShootPressed += TestShootPressed;
        InputPublicEvents.ShootReleased += TestShootCancelled;
        InputPublicEvents.InteractPressed += TestInteractPressed;
        InputPublicEvents.InteractReleased += TestInteractCancelled;
    }

    /// <summary>
    /// WILL BE REMOVED
    /// 
    /// only temporary to test the input system.
    /// </summary>
    private void OnDestroy()
    {
        InputPublicEvents.MovePressed -= TestMovePressed;
        InputPublicEvents.MoveReleased -= TestMoveCancelled;
        InputPublicEvents.ShootPressed -= TestShootPressed;
        InputPublicEvents.ShootReleased -= TestShootCancelled;
        InputPublicEvents.InteractPressed -= TestInteractPressed;
        InputPublicEvents.InteractReleased -= TestInteractCancelled;
    }

    /// <summary>
    /// logs when move is pressed
    /// </summary>
    /// <param name="dir"></param>
    private void TestMovePressed(Vector2 dir)
    {
        Debug.Log(dir);
    }

    /// <summary>
    /// logs when move is cancelled
    /// </summary>
    private void TestMoveCancelled()
    {
        Debug.Log("Move Cancelled");
    }

    /// <summary>
    /// logs when shoot is cancelled
    /// </summary>
    private void TestShootCancelled()
    {
        Debug.Log("Shoot Cancelled");
    }

    /// <summary>
    /// logs when shoot is pressed
    /// </summary>
    private void TestShootPressed()
    {
        Debug.Log("Shoot Pressed");
    }

    /// <summary>
    /// logs when interact is cancelled
    /// </summary>
    private void TestInteractCancelled()
    {
        Debug.Log("Interact Cancelled");
    }

    /// <summary>
    /// logs when interact is pressed
    /// </summary>
    private void TestInteractPressed()
    {
        Debug.Log("Interact Pressed");
    }

    #endregion
}
