using UnityEngine;

public class Keycard : MonoBehaviour
{
    [Header("Visual Settings")]
    public float rotationSpeed = 100f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    private Vector3 startPosition;

    [Header("Audio (Optional)")]
    public AudioClip collectSound;

    [Header("First Keycard Dialogue")]
    private static bool firstKeycardCollected = false;

    void Start()
    {
        // Store the starting position for bobbing effects.
        startPosition = transform.position;
    }

    void Update()
    {
        // Rotate the keycard continuously
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with this keycard
        // Works with characterController which uses the root GameObject
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            // Show dialogue on first keycard
            if (!firstKeycardCollected)
            {
                firstKeycardCollected = true;
                if (DialogueManager.Instance != null)
                {
                    string[] dialogue = new string[]
                    {
                        "What the...",
                        "Collect keycards... does that mean I have to collect all these to get out?"
                    };
                    DialogueManager.Instance.ShowMultipleLines(dialogue, 1.5f);
                }
            }

            // Find the collectionManager and add the keycard
            CollectionManager manager = FindObjectOfType<CollectionManager>();
            if (manager != null)
            {
                manager.CollectKeycard();
            }

            // Play collection sound if assigned
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Destroy the keycard
            Destroy(gameObject);
        }
    }
}