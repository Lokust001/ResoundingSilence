using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    private NavMeshAgent m_Agent;
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
        StopCoroutine(walkingCoroutine);
        walkingCoroutine = null;
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
