/*
* Author: Dalsten Yan
* Contributors: Brad Dixon
* Last Modified: 10/02/2026
* Summary: Player input, stats, and damage are handled here
* To Do:   Add more variables as needed.
*/
using System.Collections;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float playerHealth;

    [SerializeField]
    float playerDamage;

    [SerializeField]
    string[] invincibilityLayerNamesToIgnore;

    [SerializeField]
    float invincibilityDuration;

    [SerializeField] float dashRange;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] int totalDashes;
    [SerializeField] float chainDashWindow;

    #region Private Variables
    Rigidbody rigidbody;
    CapsuleCollider playerModelCollider;
    Vector3 playerVelocity;

    Coroutine dmgCoroutine;
    Coroutine knockbackCoroutine;

    Coroutine playerMovementCoroutine;

    Vector3 dashDir;
    bool canDash;
    bool chainDashing;
    int currentDash;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Allows the script to listen for player input
    /// </summary>
    private void OnEnable()
    {
        InputPublicEvents.MovePressed += PlayerInputStarted;
        InputPublicEvents.MoveReleased += PlayerInputEnded;
        InputPublicEvents.DashPressed += PlayerDashPressed;
    }

    /// <summary>
    /// Stops listening when the object is destroyed. 
    /// </summary>
    private void OnDisable()
    {
        InputPublicEvents.MovePressed -= PlayerInputStarted;
        InputPublicEvents.MoveReleased -= PlayerInputEnded;
        InputPublicEvents.DashPressed -= PlayerDashPressed;
    }

    /// <summary>
    /// Sets components
    /// </summary>
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerModelCollider = GetComponent<CapsuleCollider>();

        //Defaults to right in case player dashes before ever moving
        dashDir = Vector3.right;
        currentDash = 0;
        canDash = true;

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
        playerVelocity.x = moveDirection.x;
        playerVelocity.z = moveDirection.y;

        dashDir = new Vector3(moveDirection.x, 0, moveDirection.y);
    }

    /// <summary>
    /// Helper method to completely stop velocity when no more input is captured.
    /// First it stops the player's active velocity, then recorded velocity PlayerVelocity
    /// </summary>
    void PlayerInputEnded() 
    {
        playerVelocity.x = 0;
        playerVelocity.z = 0;
    }

    /// <summary>
    /// Checks that the player is able to dash
    /// </summary>
    private void PlayerDashPressed()
    {
        //Makes sure you can't dash while in the air
        if (rigidbody.linearVelocity.y == 0 && (canDash || chainDashing))
        {
            StartCoroutine(PlayerDash());
        }
    }

    /// <summary>
    /// Dashes the player in the last direction they were moving
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDash()
    {
        canDash = false;

        ++currentDash;
        Vector3 dashPoint = transform.position + (dashDir.normalized * dashRange);

        StopCoroutine(playerMovementCoroutine);

        for (float t = 0; t < dashDuration; t += Time.deltaTime)
        {
            transform.position = (Vector3.MoveTowards(transform.position, dashPoint, (t / dashDuration)));

            if (transform.position == dashPoint)
            {
                break;
            }
            yield return null;
        }

        canDash = true;
        RestartPlayerMovementAndInput();

        yield return null;
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
            rigidbody.linearVelocity = new Vector3(playerVelocity.x * moveSpeed, rigidbody.linearVelocity.y, playerVelocity.z * moveSpeed);

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
    public void TakeDamage((Vector3 knockbackDistance, float kbDuration)? knockbackInfo = null)
    {
        //If the player is already damaged/invicible at the moment
        if (dmgCoroutine != null)
            return;

        //Start a damage/invincibility couroutine
        dmgCoroutine ??= StartCoroutine(PlayerInvincibilityFrames());

        //If there is knockback data and there is no knockback coroutine active 
        if (knockbackInfo.HasValue)
            knockbackCoroutine ??= StartCoroutine(TakePlayerKnockback(knockbackInfo.Value));
    }

    /// <summary>
    /// Simulate the player not being able to take any damage visually
    /// or collide with any enemies for a short time
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerInvincibilityFrames() 
    {
        //Make the player intangible to collisions from specified layers
        SetInvincibilityIntangible(true);

        Renderer playerRenderer = GetComponent<Renderer>();
        Color original = playerRenderer.material.color;
        playerRenderer.material.color = Color.red;
        float timer = 0;

        while (timer < invincibilityDuration) 
        {
            timer += Time.deltaTime;
            
            yield return null;
        }
        SetInvincibilityIntangible(false);

        playerRenderer.material.color = original;
        dmgCoroutine = null;
    }

    /// <summary>
    /// Helper method that takes the layer name specified in invincibilityLayerNamesToIgnore 
    /// and either toggles the collision on or off for those layers all at once
    /// </summary>
    /// <param name="value"></param>
    private void SetInvincibilityIntangible(bool value) 
    {
        foreach (string layerName in invincibilityLayerNamesToIgnore) 
        {
            Physics.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer(layerName), value);
        }
    }

    /// <summary>
    /// Physics-based Coroutine to find the distance 
    /// </summary>
    /// <param name="knockbackInfo"></param>
    /// <returns></returns>
    public IEnumerator TakePlayerKnockback((Vector3 knockbackDistance, float knockbackDuration) knockbackInfo)
    {
        //Stop player input
        StopPlayerMovementAndInput();

        //Retrieve a local copy of the player's transform
        Transform playerTransform = transform;

        //Calculate the knockback destination, which is the player's current position plus its calculated offset after knockback
        Vector3 knockbackDestination = playerTransform.position + knockbackInfo.knockbackDistance;

        //Total distance between the player and the destination of the knockback
        float maxDistance = Vector3.Distance(playerTransform.position, knockbackDestination);

        //Tracker for remaining distance between player and knockback position
        float distance = maxDistance;

        //Count-up timer
        float timer = 0;
        //Timer goal and while loop break condition
        float timeLimit = knockbackInfo.knockbackDuration;

        /*                  - Physics Note -
         * Speed is calculated through Distance divided by Time
         *      We already possess Distance (maxDistance)
         *      We also possess Time (timeLimit)
         *      
         *  As a result, we can calculate both the linearUniformKnockbackSpeed
         *  & also the dynamicKnockbackSpeed
         */

        
        //The constant speed that would allow the player to move to knockbackDestination linearly
        float linearUniformKnockbackSpeed = maxDistance / timeLimit;

        //Cached fixedUpdate interval
        float fixedUpdateTick = Time.fixedDeltaTime;

        //The minimum speed to LERP to over time T
        float minimumKnockbackSpeed = linearUniformKnockbackSpeed * fixedUpdateTick;

        //The maximum speed the will initially be applied to the player
        float maximumKnockbackSpeed = 2 * linearUniformKnockbackSpeed;

        //For the duration of knockback,
        //Incrementally move the player towards the destination distance while dynamically calculating its speed on every FixedUpdate tick
        //The dynamic speed allows for the player to be initially fast, then slow down
        while (timer < timeLimit) 
        {

            //Tick FixedUpdate while we haven't reached duration yet
            //On the initial start of this loop, the time would be 0, so the player wouldn't move anywhere
            //To maintain as much precision as possible, wait first, and then move the player
            yield return new WaitForFixedUpdate();

            timer += fixedUpdateTick;
            //0 to 1
            float timerT = timer / timeLimit;
            
            //Calculate new distance and time-dependent knockback force
            distance = Vector3.Distance(playerTransform.position, knockbackDestination);

            //Its just this line that needs work
            float dynamicDeltaSpeed = Mathf.Lerp(maximumKnockbackSpeed, minimumKnockbackSpeed, timerT);

            //Calculate and move towards the knockback position
            Vector3 knockbackDelta = Vector3.MoveTowards(playerTransform.position, knockbackDestination, dynamicDeltaSpeed * fixedUpdateTick);
            rigidbody.MovePosition(knockbackDelta);


            Debug.Log("\tDistance: " + distance + " \tSpeed: " + dynamicDeltaSpeed + " \tTime: " + timer);

        }
        
        //The final move for 100% accuracy
        rigidbody.MovePosition(knockbackDestination);

        //Let player move again
        RestartPlayerMovementAndInput();
        knockbackCoroutine = null;
    }
}
