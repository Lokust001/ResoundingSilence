using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour, IEntityDataReceiver
{
    private NavMeshAgent m_Agent;
    private GameObject m_GameObject;

    private Coroutine walkingCoroutine;
    
    private BaseEnemyScriptable enemyData;

    Transform enemyTransform;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        enemyTransform = transform;
        m_GameObject = FindAnyObjectByType<PlayerController>().gameObject;
        StartPlayerSearch();
    }

    public void StartPlayerSearch() 
    {
        m_Agent.isStopped = false;
        walkingCoroutine = StartCoroutine(MoveTowardsPlayer());
    }

    public void EndPlayerSearch() 
    {
        m_Agent.isStopped = true;

        if (walkingCoroutine != null) 
        {
            StopCoroutine(walkingCoroutine);
        }
        walkingCoroutine = null;
    }

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
        while (distanceToGoal > enemyData.chargeDestinationAccuracyThreshold) 
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

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            Vector3 pushDirection = player.transform.position - transform.position;
            pushDirection.y = 0;
            pushDirection = pushDirection.normalized * enemyData.chargePlayerKnockbackDistanceMultiplier;

            player.TakeDamage((pushDirection, enemyData.chargePlayerKnockbackForce));
        }
    }

    private IEnumerator MoveTowardsPlayer() 
    {
        while (true) 
        {
            m_Agent.destination = m_GameObject.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetEntityData(BaseScriptableObject baseScriptable)
    {
        enemyData = (BaseEnemyScriptable)baseScriptable;
    }
}
