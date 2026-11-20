/*
* Author: Tyler
* Contributors:
* Last Modified: 09/18/2026
* Summary: Controls the camera that outputs to the minimap render texture 
* To Do:   
*/

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private PlayerController player;
    private bool shouldBeFollowingPlayer;

    [SerializeField]
    private Vector3 mainMapOffset;

    [SerializeField]
    private Vector3 miniMapOffset;

    [SerializeField]
    private Vector3 currentOffset;

    private Coroutine followingPlayerCoroutine;
    private Coroutine camPosSwapCoroutine;

    /// <summary>
    /// makes the camera zoom out or in
    /// </summary>
    /// <param name="timeToSwap"></param>
    [Button]
    public void SwapCameraPositions(float timeToSwap = 0.5f)
    {
        if (camPosSwapCoroutine == null)
        {
            Debug.Log("triggered");
            camPosSwapCoroutine = StartCoroutine(PerformCamPosSwap
                (currentOffset == miniMapOffset ? mainMapOffset : miniMapOffset, timeToSwap));
        }
    }

    /// <summary>
    /// lerps the camera positions between the current position and the target position over a given time
    /// </summary>
    /// <param name="targetOffset"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator PerformCamPosSwap(Vector3 targetOffset, float time = 0.5f)
    {
        Vector3 vec1 = currentOffset;

        float currentTimer = 0.0f;
        while (currentTimer <= time)
        {
            currentOffset = Vector3.Lerp(vec1, targetOffset, (currentTimer / time));


            currentTimer += Time.deltaTime;
            yield return null;
        }
        currentOffset = targetOffset;
        camPosSwapCoroutine = null;
    }

    /// <summary>
    /// initializes the camera
    /// </summary>
    public void InitMinimapCamera()
    {
        player = FindAnyObjectByType<PlayerController>();
        shouldBeFollowingPlayer = true;
        followingPlayerCoroutine = StartCoroutine(FollowPlayer());
        currentOffset = miniMapOffset;
    }

    /// <summary>
    /// this is the coroutine that has the camera actually follow the player
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowPlayer()
    {
        while (shouldBeFollowingPlayer)
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerController>();
            }
            else
            {
                if (player.transform.position + currentOffset != transform.position)
                {
                    transform.position = player.transform.position + currentOffset;
                }
            }

            

            yield return null;
        }
    }

    /// <summary>
    /// ensures the camera doesnt move while its disabled
    /// </summary>
    private void OnDisable()
    {
        shouldBeFollowingPlayer = false;
        if (followingPlayerCoroutine != null)
        {
            StopCoroutine(followingPlayerCoroutine);
            
        }

        if (camPosSwapCoroutine != null)
        {
            StopCoroutine(camPosSwapCoroutine);
            currentOffset = miniMapOffset;
        } 
        
    }

    /// <summary>
    /// moves the camera - callable by other scripts
    /// </summary>
    public void StartFollowingPlayerWrapper()
    {
        shouldBeFollowingPlayer = true;
        if (followingPlayerCoroutine == null)
        {
            followingPlayerCoroutine = StartCoroutine(FollowPlayer());
        }
        
    }
}
