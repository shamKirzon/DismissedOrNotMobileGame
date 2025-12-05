using UnityEngine;
using UnityEngine.AI;

public class PatrolBot : MonoBehaviour
{
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (patrolPoints.Length > 0)
        {
            GoToNextPoint();
        }
    }

    void Update()
    {
        // Check if agent has reached current destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        // Set next destination
        agent.destination = patrolPoints[currentPointIndex].position;

        // Update index for next waypoint (loops back to 0)
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }
}
