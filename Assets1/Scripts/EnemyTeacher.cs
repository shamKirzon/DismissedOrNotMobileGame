using UnityEngine;
using UnityEngine.AI;

public class EnemyTeacher : MonoBehaviour
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
    private NavMeshAgent agent;
    private int currentWaypointIndex = 0;
    private bool isChasing = false;
    private bool hasDetectedPlayer = false; // NEW: Track if player was ever detected
    private float attackCooldown = 0f;
    public float attackCooldownTime = 1f;
    private SafeRoom[] safeRooms;
    private bool hasShownDialogue = false;

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

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
        if (waypoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
    }

    void Update()
    {
        if (player == null) return;

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
                        "Crap! What are you chasing me for!",
                        "Is this some kind of prank?!"
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

        // Switch back to patrol mode
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }

        // Check if reached current waypoint
        if (!agent.pathPending && agent.remainingDistance <= 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;

        // Set destination to current waypoint
        agent.SetDestination(waypoints[currentWaypointIndex].position);

        // Move to next waypoint (loop back to start)
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    void ChasePlayer()
    {
        // Switch to chase mode
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            Debug.Log("Teacher started chasing!");
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

        Debug.Log("Teacher attacked the player!");

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