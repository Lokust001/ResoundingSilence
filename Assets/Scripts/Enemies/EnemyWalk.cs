/* Author: Dalsten Yan
 * Contributors:
 * Last Modified: 9/28/26
 * Summary: Script that simulates enemy movement, seeking, charing down, and retreating
 * TODO: More as needed
 */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour, IStatBasedEnemies, ICustomEnabler, ICustomDisabler
{
    private NavMeshAgent navMeshAgent;
    private GameObject playerReference;

    private Coroutine walkingCoroutine;
    
    private BaseEnemyScriptable enemyData;

    private EnemyAttack enemyAttack;

    Transform enemyTransform;
    CapsuleCollider capsuleCollider;

    private MeshRenderer meshRenderer;
    private Rigidbody rigidbody;

    /// <summary>
    /// Grabs necessary components and assigns enemyTransform to transform component
    /// </summary>
    private void InitializeVariables() 
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerReference = FindAnyObjectByType<PlayerController>().gameObject;
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        meshRenderer = GetComponent<MeshRenderer>();
        rigidbody = GetComponent<Rigidbody>();
        enemyTransform = transform;
    }

    /// <summary>
    /// Start a coroutine for ranged enemies to strafe towards the enemies
    /// </summary>
    public void StrafeTowardsPlayer() 
    {
        StopApproachingPlayer();
        walkingCoroutine = StartCoroutine(StrafeWalking());
    }

    /// <summary>
    /// Start a coroutine for melee enemies to directly follow the player
    /// </summary>
    public void FollowPlayer() 
    {
        StopApproachingPlayer();
        walkingCoroutine = StartCoroutine(FollowWalking());
    }

    /// <summary>
    /// Stop searching for the player by constraining the nav mesh agent
    /// </summary>
    public void StopApproachingPlayer() 
    {
        navMeshAgent.isStopped = true;
        if (walkingCoroutine != null) 
        {
            StopCoroutine(walkingCoroutine);
            walkingCoroutine = null;
        }
    }

    /// <summary>
    /// Given a distance, make the enemy charge towards the direction they were facing
    /// </summary>
    /// <param name="chargeDistance"></param>
    /// <returns></returns>
    public IEnumerator ChargeTowardsLocation(float chargeDistance) 
    {

        //Double the charge distance to account for the offset of the dash indicator
        Vector3 enemyForwardChargeDistance = 2 * chargeDistance * transform.forward;
        
        //Move the enemy towards the destination, then wait until they get there
        Vector3 chargeDestination = transform.position + enemyForwardChargeDistance;

        //Find the distance from the enemy's current position to the destination of the charge
        //using square magnitude for more efficiency
        float distanceToGoal = (chargeDestination - enemyTransform.position).sqrMagnitude;

        //Make the rigidbody on the enemy temporarily kinematic to avoid letting it be interrupted by the player
        //rigidbody.isKinematic = true;

        //While there is still a significant gap or distance between the enemy and its charge destination,
        //calculate its current distance from its goal, move the enemy towards the goal by a factor of its chargeSpeedForce,
        //and finally wait a frame then repeat
        while (distanceToGoal > enemyData.chargeDestinationAccuracy) 
        {
            distanceToGoal =  (chargeDestination - enemyTransform.position).sqrMagnitude;
            Vector3 goalOverTime = Vector3.MoveTowards(enemyTransform.position, chargeDestination, enemyData.chargeSpeedForce * Time.deltaTime);
            rigidbody.MovePosition(goalOverTime);
            yield return null;
        }

        //Restore properties and velocity
        //rigidbody.isKinematic = false;
        rigidbody.linearVelocity = rigidbody.angularVelocity = Vector3.zero;
        
    }

    /// <summary>
    /// If the enemy runs into the player it is currently running down the charge, make player take damage and knockback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (enemyAttack.IsEnemyCurrentlyCharging())
            {
                Vector3 pushDirection = player.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection = pushDirection.normalized * enemyData.playerHitKnockbackDistance;

                player.TakeDamage(enemyData.enemyATKDmg, (pushDirection, enemyData.playerHitKnockbackDuration));
            }
        }
    }

    /// <summary>
    /// Move closer towards the player one cycle at a time until the player is reached or a new strafe is started
    /// </summary>
    /// <returns></returns>
    private IEnumerator StrafeWalking() 
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = CalculateMovementVariability();

        while (navMeshAgent.hasPath) 
        {
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Follow the player completely until the player is within range
    /// </summary>
    /// <returns></returns>
    private IEnumerator FollowWalking() 
    {
        navMeshAgent.isStopped = false;
        Transform playerTransform = playerReference.transform;


        while (true)
        {
            navMeshAgent.destination = playerTransform.position;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Helper method that uses trignometry to repeatedly calculate a random position to walk towards or retreat towards until one is found
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector3 CalculateMovementVariability(int offset = 0) 
    {
        //Splits the approach or retreat ariability degrees into two so that it encompasses as much the left of the circle as the right from the front
        int halfPieSlice = offset == 0 ? enemyData.approachDestinationVariabilityDegrees / 2 : enemyData.retreatDestinationVariabilityDegrees / 2;
        //Scales the rotated player offset for a minimum retreat distance
        float playerOffsetScaler = offset == 0 ? 1 : enemyData.retreatDistance;

        //Base case for while loop
        bool validLocation = false;
        //Return value
        Vector3 finalGoal = transform.position;

        //Fail-safe
        int attempts = 0;

        
        while (!validLocation) 
        {
            //Generates a random approach angle to rotate the destination by
            int currentApproachAngleOffset = Random.Range(-halfPieSlice, halfPieSlice + 1);
            currentApproachAngleOffset += offset;

            Vector3 offsetFromPlayer = playerReference.transform.position - transform.position;
            Vector3 rotatedOffset = Quaternion.Euler(0, currentApproachAngleOffset, 0) * offsetFromPlayer * playerOffsetScaler;
            finalGoal = transform.position + rotatedOffset;

            //[Uncomment for debug]
            //Debug.Log("Location Calculation attempt #" + (attempts + 1)+ ": Rotated movement destination by " + currentApproachAngleOffset + " degrees");

            //Checks the destination to see if it is a valid navmesh location
            validLocation = NavMesh.SamplePosition(finalGoal, out _, capsuleCollider.height, LayerMask.NameToLayer("Ground"));
            attempts++;
            if (attempts >= enemyData.maxVariabilityDestinationAttempts) 
            {
                Debug.LogWarning("Warning! " + gameObject.name + " failed to calculate a valid movement location");
                finalGoal = transform.position;
                break;
            }
        }

        return finalGoal;
    }

    /// <summary>
    /// Move away from the player towards a valid location
    /// </summary>
    /// <returns></returns>
    public IEnumerator RetreatWalking() 
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = CalculateMovementVariability(180);

        while (navMeshAgent.hasPath)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// [Deprecated] Move away from the player by a specified distance over time 
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveAwayFromPlayer() 
    {
        Transform playerTransform = playerReference.transform;
        Vector3 directionToPlayer = (enemyTransform.position - playerTransform.position).normalized;
        directionToPlayer.y = 0;

        Vector3 retreatPosition = enemyTransform.position + (directionToPlayer * enemyData.retreatDistance);
        navMeshAgent.destination = retreatPosition;
        navMeshAgent.isStopped = false;

        float distance = Vector3.Distance(enemyTransform.position, retreatPosition);
        float accuracy = enemyData.retreatAccuracyThreshold;
        Vector3 prevPos;

        while (distance > accuracy) 
        {
            prevPos = enemyTransform.position;
            distance = Vector3.Distance(enemyTransform.position, retreatPosition);
            yield return new WaitForFixedUpdate();
            if (prevPos == enemyTransform.position) 
            {
                Debug.Log("Stuck");
                navMeshAgent.ResetPath();
                navMeshAgent.SetDestination(retreatPosition);
            }
        }

        enemyTransform.position = retreatPosition;
        navMeshAgent.isStopped = true;
        Debug.Log("Finished Retreat");
    }

    /// <summary>
    /// Casts the scriptable object into a BaseEnemyScriptable and assigns the data of the enemy to this script
    /// </summary>
    /// <param name="baseScriptable"></param>
    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
        InitializeVariables();
    }

    /// <summary>
    /// Specifies which components on this enemy to enable when it is called to be enabled
    /// </summary>
    public void EnableEntity()
    {
        meshRenderer.enabled = true;
        capsuleCollider.enabled = true;
    }

    /// <summary>
    /// Specifies which components & methods to discable when this enemy needs to be disabled
    /// </summary>
    public void DisableEntity()
    {
        DeAggroEntity();
        meshRenderer.enabled = false;
        capsuleCollider.enabled = false;
    }

    /// <summary>
    /// De-aggro's the enemy's movement behavior by stopping all movement coroutines
    /// </summary>
    public void DeAggroEntity() 
    {
        StopApproachingPlayer();
        StopAllCoroutines();
    }
}
