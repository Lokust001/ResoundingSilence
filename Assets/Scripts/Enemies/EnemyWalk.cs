/* Author: Dalsten Yan
 * Contributors:
 * Last Modified: 9/28/26
 * Summary: Script that simulates enemy movement, seeking, charing down, and retreating
 * TODO: More as needed
 */
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour, IEntityDataReceiver
{
    private NavMeshAgent m_Agent;
    private GameObject m_GameObject;

    private Coroutine walkingCoroutine;
    
    private BaseEnemyScriptable enemyData;

    private EnemyAttack enemyAttack;
    private EnemyBuff enemyBuff;

    Transform enemyTransform;
    CapsuleCollider capsuleCollider;

    /// <summary>
    /// Grabs necessary components and assigns enemyTransform to transform component
    /// </summary>
    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_GameObject = FindAnyObjectByType<PlayerController>().gameObject;
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        enemyBuff = GetComponentInChildren<EnemyBuff>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        enemyTransform = transform;
    }

    /// <summary>
    /// Start searching for the player by un-constraining the nav mesh agent and moving towards the player
    /// </summary>
    public void StartPlayerSearch() 
    {
        m_Agent.isStopped = false;
        walkingCoroutine = StartCoroutine(MoveTowardsPlayer());
    }

    /// <summary>
    /// Stop searching for the player by constraining the nav mesh agent
    /// </summary>
    public void EndPlayerSearch() 
    {
        m_Agent.isStopped = true;

        if (walkingCoroutine != null) 
        {
            StopCoroutine(walkingCoroutine);
        }
        walkingCoroutine = null;
    }

    /// <summary>
    /// Given a distance, make the enemy charge towards the direction they were facing
    /// </summary>
    /// <param name="chargeDistance"></param>
    /// <returns></returns>
    public IEnumerator ChargeTowardsLocation(float chargeDistance) 
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        //Double the charge distance to account for the offset of the dash indicator
        Vector3 enemyForwardChargeDistance = 2 * chargeDistance * transform.forward;
        
        //Move the enemy towards the destination, then wait until they get there
        Vector3 chargeDestination = transform.position + enemyForwardChargeDistance;

        //Find the distance from the enemy's current position to the destination of the charge
        //using square magnitude for more efficiency
        float distanceToGoal = (chargeDestination - enemyTransform.position).sqrMagnitude;

        //Make the rigidbody on the enemy temporarily kinematic to avoid letting it be interrupted by the player
        rb.isKinematic = true;

        //While there is still a significant gap or distance between the enemy and its charge destination,
        //calculate its current distance from its goal, move the enemy towards the goal by a factor of its chargeSpeedForce,
        //and finally wait a frame then repeat
        while (distanceToGoal > enemyData.chargeDestinationAccuracy) 
        {
            distanceToGoal =  (chargeDestination - enemyTransform.position).sqrMagnitude;
            Vector3 goalOverTime = Vector3.MoveTowards(enemyTransform.position, chargeDestination, enemyData.chargeSpeedForce * Time.deltaTime);
            rb.MovePosition(goalOverTime);
            yield return null;
        }

        //Restore properties and velocity
        rb.isKinematic = false;
        rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        
    }

    /// <summary>
    /// If the enemy runs into the player it is currently running down the charge, make player take damage and knockback
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Debug.Log(Physics.GetIgnoreCollision(collision.collider, capsuleCollider));
            if (enemyAttack.IsEnemyCurrentlyCharging())
            {
                Vector3 pushDirection = player.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection = pushDirection.normalized * enemyData.playerHitKnockbackDistance;

                player.TakeDamage((pushDirection, enemyData.playerHitKnockbackDuration));
            }
        }
    }

    /// <summary>
    /// Move incrementally closer towards the player until this Coroutine is stopped
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveTowardsPlayer() 
    {
        while (true) 
        {
            m_Agent.destination = m_GameObject.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Move away from the player by a specified distance over time
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveAwayFromPlayer() 
    {
        Transform playerTransform = FindAnyObjectByType<PlayerController>().transform;
        Vector3 directionToPlayer = (enemyTransform.position - playerTransform.position).normalized;
        directionToPlayer.y = 0;

        Vector3 retreatPosition = enemyTransform.position + (directionToPlayer * enemyData.retreatDistance);

        float retreatSpeed = enemyData.retreatSpeed;
        float distance = Vector3.Distance(enemyTransform.position, retreatPosition);


        while (distance > 0) 
        {
            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, retreatPosition, retreatSpeed * Time.fixedDeltaTime);
            distance = Vector3.Distance(enemyTransform.position, retreatPosition);
            yield return new WaitForFixedUpdate();
        }
        
    }

    /// <summary>
    /// Casts the scriptable object into a BaseEnemyScriptable and assigns the data of the enemy to this script
    /// </summary>
    /// <param name="baseScriptable"></param>
    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
        Awake();
    }
}
