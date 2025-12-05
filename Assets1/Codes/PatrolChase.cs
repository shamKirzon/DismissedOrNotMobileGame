using UnityEngine;
using UnityEngine.AI;

public class PatrolChase : MonoBehaviour
{
    public Transform[] patrolPoints;

    public Transform player;

    public float detectionRange = 5f;

    private NavMeshAgent agent;

    private int currentPointIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Start walking to the first patrol point
        GoToNextPoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --------------------------
        // CHASE MODE
        // If the player is inside the detection range, follow the player
        // --------------------------
        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
            return; // stop patrol code because we are chasing
        }

        // PATROL MODE
        // If the NPC reached the current patrol point, go to the next one
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            GoToNextPoint();
        }
    }

    // Moves the NPC to the next patrol point
    void GoToNextPoint()
    {
        // Don't do anything if there are no patrol points assigned
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        // Choose the patrol point based on the index
        Transform targetPoint = patrolPoints[currentPointIndex];

        // Tell the NavMeshAgent to walk to the target point
        agent.SetDestination(targetPoint.position);

        // Move to the next index
        // If we reach the end, go back to index 0
        currentPointIndex = currentPointIndex + 1;

        if (currentPointIndex >= patrolPoints.Length)
        {
            currentPointIndex = 0; // loop back to start
        }
    }

    private void OnDrawGizmosSelected()
    {
        // This draws a red circle in the editor so you can see the detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
