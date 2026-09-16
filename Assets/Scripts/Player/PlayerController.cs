/* Author: Dalsten Yan
 * Creation Date 9/15/2026
 * 
 * PlayerController.cs enables the player character to move based on captured PlayerInput 
 */

using UnityEngine;
using Unity.Scripting;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    #region Private Variables
    Rigidbody rigidbody;
    Vector3 playerVelocity;

    Coroutine playerMovementCoroutine;
    #endregion

    #region Unity Methods

    private void OnEnable()
    {
        InputPublicEvents.MovePressed += PlayerInputStarted;
        InputPublicEvents.MoveReleased += PlayerInputEnded;
    }

    private void OnDisable()
    {
        InputPublicEvents.MovePressed -= PlayerInputStarted;
        InputPublicEvents.MoveReleased -= PlayerInputEnded;
    }
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        //Original movement speed is zero + start move coroutine
        PlayerInputEnded();
        RestartPlayerMovementAndInput();
    }
    #endregion

    /// <summary>
    /// Helper method to update PlayerVelocity based on user input
    /// </summary>
    /// <param name="moveDirection"></param>
    void PlayerInputStarted(Vector2 moveDirection) 
    {
        playerVelocity.x = moveDirection.x * moveSpeed;
        playerVelocity.z = moveDirection.y * moveSpeed;
    }

    /// <summary>
    /// Helper method to completely stop velocity when no more input is captured.
    /// First it stops the player's active velocity, then recorded velocity PlayerVelocity
    /// </summary>
    void PlayerInputEnded() 
    {
        rigidbody.linearVelocity = Vector3.zero;
        playerVelocity = Vector3.zero;
    }

    /// <summary>
    /// Coroutine that continuously runs and moves the player unless disabled by external factors.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePlayerCoroutine() 
    {
        while (true) 
        {
            //Propagate calculated velocity to rigidbody & make player move
            rigidbody.linearVelocity = playerVelocity;

            //Tick Coroutine as if it were in FixedUpdate
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Public method to halt all player movement
    /// </summary>
    public void StopPlayerMovementAndInput() 
    {
        StopCoroutine(MovePlayerCoroutine());
        PlayerInputEnded();
    }

    /// <summary>
    /// Public method to start moving the player once again
    /// </summary>
    public void RestartPlayerMovementAndInput() 
    {
        playerMovementCoroutine = StartCoroutine(MovePlayerCoroutine());
    }
}
