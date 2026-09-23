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

    [SerializeField]
    float playerHealth;

    [SerializeField]
    float invincibilityDuration;

    [SerializeField]
    float knockbackDestinationAccuracyThreshold;

    #region Private Variables
    Rigidbody rigidbody;
    Vector3 playerVelocity;

    Coroutine dmgCoroutine;
    Coroutine knockbackCoroutine;

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
        StopCoroutine(playerMovementCoroutine);
        PlayerInputEnded();
    }

    /// <summary>
    /// Public method to start moving the player once again
    /// </summary>
    public void RestartPlayerMovementAndInput() 
    {
        playerMovementCoroutine = StartCoroutine(MovePlayerCoroutine());
    }

    /// <summary>
    /// Lets the player take damage, with optional/nullable knockbackInfo
    /// </summary>
    /// <param name="knockbackInfo"></param>
    public void TakeDamage((Vector3 knockbackDistance, float knockbackSpeed)? knockbackInfo = null)
    {
        //If the player is already damaged/invicible at the moment
        if (dmgCoroutine != null)
            return;

        //Start a damage/invincibility couroutine
        dmgCoroutine ??= StartCoroutine(PlayerInvincibilityFrames());

        //If there is knockback data and there is no knockback coroutine active 
        if(knockbackInfo.HasValue)
            knockbackCoroutine ??= StartCoroutine(TakePlayerKnockback(knockbackInfo.Value));
    }

    private IEnumerator PlayerInvincibilityFrames() 
    {
        Renderer playerRenderer = GetComponent<Renderer>();
        Color original = playerRenderer.material.color;
        playerRenderer.material.color = Color.red;
        float timer = 0;
        while (timer < invincibilityDuration) 
        {
            timer += Time.deltaTime;
            
            yield return null;
        }

        playerRenderer.material.color = original;
        dmgCoroutine = null;
    }

    public IEnumerator TakePlayerKnockback((Vector3 knockbackDistance, float knockbackSpeed) knockbackInfo) 
    {
        //Stop player input
        StopPlayerMovementAndInput();

        //Disable gravity and the collider at once
        rigidbody.useGravity = GetComponent<Collider>().enabled = false;

        //Retrieve a local copy of the player's transform
        Transform playerTransform = transform;

        //Calculate the knockback destination, which is the player's current position plus its calculated offset after knockback
        Vector3 knockbackDestination = playerTransform.position + knockbackInfo.knockbackDistance;
        
        //calculate the distance and how far the distance is bridged between frames
        float distanceDeltaMultiplier = knockbackInfo.knockbackSpeed;
        float distance = (knockbackDestination - playerTransform.position).sqrMagnitude;

        Debug.Log("Starting Knockback");

        //While the distance has more than the accuracy threshold,
        //move the player towards the knockback position and then calculate the new distance
        while (distance > knockbackDestinationAccuracyThreshold) 
        {
            Vector3 knockbackDelta = Vector3.MoveTowards(playerTransform.position, knockbackDestination, distanceDeltaMultiplier * Time.deltaTime);
            rigidbody.MovePosition(knockbackDelta);
            distance = (knockbackDelta - playerTransform.position).sqrMagnitude;
            yield return null;
        }

        //Re-enable gravity and the collider
        rigidbody.useGravity = GetComponent<Collider>().enabled = true;
        RestartPlayerMovementAndInput();
        knockbackCoroutine = null;
    }
}
