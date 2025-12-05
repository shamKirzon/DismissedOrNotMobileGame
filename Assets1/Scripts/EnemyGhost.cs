using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGhost : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform[] waypoints; // Patrol points
    public float patrolSpeed = 3f;
    public float chaseSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;

    [Header("References")]
    public Transform player;

    // Private variables
    private UnityEngine.AI.NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private bool hasDetectedPlayer = false; // NEW: Track if player was ever detected
    private float attackCooldown = 0f;
    public float attackCooldownTime = 1f;
    private SafeRoom[] safeRooms;
    private bool hasShownDialogue = false;

    void Start()
    {
        // Get the navMeshAgent Component
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Find the player automatically if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Find all safe rooms in the scene
        safeRooms = FindObjectsOfType<SafeRoom>();

        // Start patrolling
        if (waypoints.Length > 0 && agent != null)
        {
            agent.speed = patrolSpeed;
            // Wait for NavMesh to be ready before setting destination
            if (agent.isOnNavMesh)
            {
                GoToNextWaypoint();
            }
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        // IMPORTANT: Check if agent is on NavMesh before doing anything
        if (!agent.isOnNavMesh || !agent.enabled)
        {
            Debug.LogWarning("Agent is not on NavMesh!");
            return;
        }

        // Update attack cooldown
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        // Check if player is in a safe room
        bool playerIsSafe = IsPlayerInSafeRoom();

        // Check distance to player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Detect player for the first time
        if (distanceToPlayer <= detectionRange && !hasDetectedPlayer)
        {
            hasDetectedPlayer = true; // Mark as detected

            // Show dialogue when first spotted
            if (!hasShownDialogue)
            {
                hasShownDialogue = true;
                if (DialogueManager.Instance != null)
                {
                    string[] dialogue = new string[]
                    {
                        "Why are Ghosts Chasing Me now ?!",
                        "It's just one thing after another in this place"
                    };
                    DialogueManager.Instance.ShowMultipleLines(dialogue, 1f);
                }
            }
        }

        // CHASE MODE: Once detected, chase forever UNLESS player is in safe room
        if (hasDetectedPlayer && !playerIsSafe)
        {
            ChasePlayer();
        }
        // PATROL MODE: Only patrol if never detected OR player is in safe room
        else if (!hasDetectedPlayer || playerIsSafe)
        {
            Patrol();

            // Reset detection if player enters safe room (optional - remove if you want permanent detection)
            if (playerIsSafe)
            {
                hasDetectedPlayer = false;
            }
        }

        // Check if close enough to attack (only if player is NOT safe)
        if (distanceToPlayer <= attackRange && attackCooldown <= 0 && !playerIsSafe)
        {
            AttackPlayer();
        }
    }

    bool IsPlayerInSafeRoom()
    {
        // Check all safe rooms
        foreach (SafeRoom room in safeRooms)
        {
            if (room != null && room.IsPositionInside(player.position))
            {
                return true;
            }
        }
        return false;
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // IMPORTANT: Check if agent is ready before accessing remainingDistance
        if (!agent.isOnNavMesh || !agent.enabled)
            return;

        // Switch back to patrol mode
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }

        // Check if reached current waypoint
        // FIXED: Added pathPending check before remainingDistance
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0 || agent == null) return;

        // IMPORTANT: Only set destination if agent is on NavMesh
        if (!agent.isOnNavMesh || !agent.enabled)
            return;

        // Set destination to current waypoint
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        // Move to next waypoint (loop back to start)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    void ChasePlayer()
    {
        if (agent == null || player == null) return;

        // IMPORTANT: Only chase if agent is on NavMesh
        if (!agent.isOnNavMesh || !agent.enabled)
            return;

        // Switch to chase mode
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("GHOST started chasing!");
        }

        // ALWAYS follow the player (never give up!)
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        // Can't attack if player is in safe room
        if (IsPlayerInSafeRoom())
        {
            return;
        }

        // Start cooldown
        attackCooldown = attackCooldownTime;

        Debug.Log("GHOST attacked the player!");

        // Deal damage to player
        HealthSystem playerHealth = player.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); // Deal 1 damage
        }

        // You can add a simple feedback here
        // Like playing a sound or showing an effect
    }

    // Draw detection range in editor (helpful for debugging)
    void OnDrawGizmosSelected()
    {
        // Detection range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}