using UnityEngine;

public class Book : MonoBehaviour
{
    [Header("Visual Settings")]
    public float rotationSpeed = 50f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPosition;

    [Header("Audio (Optional)")]
    public AudioClip collectSound;

    [Header("First Book Dialogue")]
    private static bool firstBookCollected = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        //Rotate The Book
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Bob Up and Down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            // Show Dialogue on First Book
            if (!firstBookCollected)
            {
                firstBookCollected = true;
                if (DialogueManager.Instance != null)
                {
                    string[] dialogue = new string[]
                    {
                        "Books? They're scattered everywhere...",
                        "This won't be easy."
                    };
                    DialogueManager.Instance.ShowMultipleLines(dialogue, 1.5f);
                }
            }

            // Find BookManager and collect
            BookManager manager = FindObjectOfType<BookManager>();
            if (manager != null)
            {
                manager.CollectBook();
            }

            // Play sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}