/*
* Author: Dalsten Yan
* Contributors:
* Last Modified: 10/02/2026
* Summary: Keeps track of time in the game
* To Do:   
*/
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class GameTimerManager : BaseManager
{

    public static GameTimerManager Instance { get; private set; }

    private float currentGameTime;
    private Coroutine gameTimeCoroutine;

    /// <summary>
    /// Sets up the singleton
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else { Destroy(this); }
        StartOrResumeGameTime();
    }

    /// <summary>
    /// public method to start game time or game timer
    /// </summary>
    public void StartOrResumeGameTime() 
    {
        gameTimeCoroutine = StartCoroutine(ProgressGameTime());
    }

    /// <summary>
    /// public method to stop the game timer
    /// </summary>
    public void PauseOrStopGameTime() 
    {
        StopCoroutine(gameTimeCoroutine);
    }


    /// <summary>
    /// Loop that continually advances game time and can be stopped or started whenever
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProgressGameTime() 
    {
        while (true) 
        {
            yield return null;
            currentGameTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Returns the precise game time for UI display purposes
    /// </summary>
    /// <returns></returns>
    public float GetPreciseGameTime() 
    {
        return currentGameTime;
    }

    /// <summary>
    /// Returns the game time as a non-precise whole number
    /// </summary>
    /// <returns></returns>
    public int GetRoundedGameTime() 
    {
        return Mathf.RoundToInt(currentGameTime);
    }

    [Button("Get Current Time")]
    public void DebugCurrentGameTime() 
    {
        Debug.Log(GetPreciseGameTime());
    }
}
