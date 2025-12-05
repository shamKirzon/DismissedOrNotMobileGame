using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OpeningSequence : MonoBehaviour
{
    [Header("UI References")]
    public Image blackScreen;
    public TextMeshProUGUI dialogueText;

    [Header("Player Reference")]
    public GameObject player;

    [Header("Blink Settings")]
    public int blinkCount = 3;
    public float blinkSpeed = 1.5f;

    [Header("Dialogue Settings")]
    public float typingSpeed = 0.05f;
    public string[] dialogueLines = new string[]
    {
        "W... Where am I...",
        "Why is it... so dark?",
        "I need to get out of here..."
    };
    public bool skipOpeningBlink = false; // Set TRUE for Level 2+

    [Header("Timing")]
    public float pauseBetweenLines = 1.5f;
    public float finalPauseBeforeGameplay = 2f;

    private PlayerController playerController;

    void Start()
    {
        // Disable player controls at start
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        // Lock cursor during sequence
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start the opening sequence
        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        // Start with black screen
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 1f;
            blackScreen.color = c;
        }

        // Skip blink for Level 2+
        if (!skipOpeningBlink)
        {
            // Initial pause (still asleep)
            yield return new WaitForSeconds(1f);

            // Slow blink animation (eyes opening and closing)
            for (int i = 0; i < blinkCount; i++)
            {
                // Open eyes (fade to transparent)
                yield return StartCoroutine(FadeScreen(1f, 0.3f, blinkSpeed));

                // Brief moment of vision
                yield return new WaitForSeconds(0.3f);

                // Close eyes (fade to black)
                yield return StartCoroutine(FadeScreen(0.3f, 1f, blinkSpeed * 0.8f));

                // Pause between blinks
                yield return new WaitForSeconds(0.5f);
            }
        }

        // Final eye open (stay open)
        yield return StartCoroutine(FadeScreen(1f, 0f, skipOpeningBlink ? 1f : blinkSpeed * 1.5f));

        // Wait a moment before dialogue
        yield return new WaitForSeconds(0.5f);

        // Show dialogue lines with typing effect
        foreach (string line in dialogueLines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(pauseBetweenLines);

            // Clear text before next line
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
        }

        // Final pause before gameplay
        yield return new WaitForSeconds(finalPauseBeforeGameplay);

        // Fade out black screen completely
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }

        // Hide dialogue text
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
        }

        // Enable player controls
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        Debug.Log("Opening sequence complete! Game started.");
    }

    IEnumerator FadeScreen(float startAlpha, float endAlpha, float duration)
    {
        if (blackScreen == null) yield break;

        float elapsed = 0f;
        Color c = blackScreen.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            blackScreen.color = c;
            yield return null;
        }

        // Ensure final alpha is set
        c.a = endAlpha;
        blackScreen.color = c;
    }

    IEnumerator TypeText(string text)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;

            // Play typing sound here if you have one
            // AudioSource.PlayClipAtPoint(typingSound, Camera.main.transform.position, 0.5f);

            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Optional: Allow player to skip sequence
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            // Skip to gameplay
            StopAllCoroutines();

            if (blackScreen != null)
            {
                blackScreen.gameObject.SetActive(false);
            }

            if (dialogueText != null)
            {
                dialogueText.gameObject.SetActive(false);
            }

            if (playerController != null)
            {
                playerController.enabled = true;
            }

            Debug.Log("Opening sequence skipped!");
        }
    }
}