using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    private NavMeshAgent m_Agent;
    [SerializeField] private GameObject m_GameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }



    // Update is called once per frame
    void Update()
    {

        m_Agent.destination = m_GameObject.transform.position;
    }
}
