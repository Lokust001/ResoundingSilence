using System.Collections;
using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    private PlayerController player;
    private bool shouldBeFollowingPlayer;

    private Vector3 offset;

    private Coroutine followingPlayerCoroutine;

    private void InitMinimapCamera()
    {
        offset = transform.position;
        player = FindAnyObjectByType<PlayerController>();

        followingPlayerCoroutine = StartCoroutine(FollowPlayer());
    }

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

    private void OnDisable()
    {
        StopCoroutine(followingPlayerCoroutine);
    }

    public void StartFollowingPlayerWrapper()
    {
        if (followingPlayerCoroutine == null)
        {
            followingPlayerCoroutine = StartCoroutine(FollowPlayer());
        }
        
    }
}
