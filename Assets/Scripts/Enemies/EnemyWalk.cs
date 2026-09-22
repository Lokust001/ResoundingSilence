using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    public NavMeshAgent m_Agent;
    private GameObject m_GameObject;

    private Coroutine walkingCoroutine;

    

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
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

    public IEnumerator ChargeTowardsLocation(float chargingSpeed, float chargingAccelerationSpeed, float chargeDistance) 
    {
        //Store original NavMeshAgent and all of its properties
        NavMeshAgent originalAgent = m_Agent;

        //Continue along a path
        m_Agent.isStopped = false;
        //Do NOT stop early
        m_Agent.stoppingDistance = 0;
        //Do NOT break early either, otherwise the force will be interrupted
        m_Agent.autoBraking = false;
        //Turn up the speed dial to the the dashing speed specified in enemyData
        m_Agent.speed = chargingSpeed;
        //Turn up the acceleration dial to the accelerating speed specified in enemyData
        m_Agent.acceleration = chargingAccelerationSpeed;

        //Double the charge distance to account for the offset of the dash indicator
        Vector3 chargeDestination = 2 * chargeDistance * transform.forward;
        
        //Move the enemy towards the destination
        m_Agent.destination = transform.position + chargeDestination;
        while (m_Agent.remainingDistance > m_Agent.stoppingDistance) 
        {
            yield return null;
        }
        //Restore properties
        //GetComponent<NavMeshAgent>(). = m_Agent = originalAgent;
    }


    private IEnumerator MoveTowardsPlayer() 
    {
        while (true) 
        {
            m_Agent.destination = m_GameObject.transform.position;
            yield return new WaitForFixedUpdate();
        }
    }

}
