/*
* Author: Tyler
* Contributors:
* Last Modified: 09/18/2026
* Summary: Controls the camera that outputs to the minimap render texture 
* To Do:   
*/

using System.Collections;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private PlayerController player;
    private bool shouldBeFollowingPlayer;

    private Vector3 offset;

    private Coroutine followingPlayerCoroutine;

    /// <summary>
    /// initializes the camera
    /// </summary>
    private void InitMinimapCamera()
    {
        offset = transform.position;
        player = FindAnyObjectByType<PlayerController>();
        shouldBeFollowingPlayer = true;
        followingPlayerCoroutine = StartCoroutine(FollowPlayer());
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
                if (player.transform.position + offset != transform.position)
                {
                    transform.position = player.transform.position + offset;
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
