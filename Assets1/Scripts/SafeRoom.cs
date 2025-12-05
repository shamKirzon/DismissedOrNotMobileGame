using UnityEngine;

public class SafeRoom : MonoBehaviour
{
    [Header("Safe Room Settings")]
    public Color safeColor = Color.green;
    public Color normalColor = Color.white;

    private bool playerInside = false;
    private bool hasShownDialogue = false;

    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("🛡️ Player entered SAFE ROOM - You're protected!");

            // Show dialogue first time entering any safe room
            if (!hasShownDialogue)
            {
                hasShownDialogue = true;
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("I think I'm safe in this closet... let's hide here for now.");
                }
            }

            // Optional: Change player color to show they're safe
            Renderer playerRenderer = other.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                playerRenderer.material.color = safeColor;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if player left
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            Debug.Log("⚠️ Player left safe room - DANGER!");

            // Reset player color
            Renderer playerRenderer = other.GetComponent<Renderer>();
            if (playerRenderer != null)
            {
                playerRenderer.material.color = normalColor;
            }
        }
    }

    // Public method so enemies can check if player is safe
    public bool IsPlayerInside()
    {
        return playerInside;
    }

    // Check if a specific position is inside this safe room
    public bool IsPositionInside(Vector3 position)
    {
        Collider col = GetComponent<Collider>();
        return col.bounds.Contains(position);
    }
}