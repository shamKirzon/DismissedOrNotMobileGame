using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExitDoor2 : MonoBehaviour
{
    [Header("Door Settings")]
    public bool isLocked = true;
    public string nextSceneName = "Level3_ParkingLot";

    [Header("Visual Feedback")]
    public Material lockedMaterial;
    public Material unlockedMaterial;
    public Light doorLight;

    [Header("UI References")]
    public Image fadeImage;
    public GameObject completionPanel;

    [Header("Fade Settings")]
    public float fadeAndWaitDuration = 8f;

    private Renderer doorRenderer;
    private bool isTransitioning = false;

    void Start()
    {
        doorRenderer = GetComponent<Renderer>();

        if (isLocked)
        {
            SetLockedAppearance();
        }
        else
        {
            SetUnlockedAppearance();
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocked && doorLight != null)
        {
            float pulse = Mathf.PingPong(Time.time * 2f, 1f);
            doorLight.intensity = 3f + pulse * 2f;
        }
    }

    public void UnlockDoor()
    {
        if (isLocked)
        {
            isLocked = false;
            SetUnlockedAppearance();
            Debug.Log("🔓 LIBRARY EXIT UNLOCKED! You can now escape!");

            // Show dialogue when door unlocks
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("Finally! Get me out of this damn Place!");
            }
        }
        else
        {
            Debug.Log("Door is already unlocked!");
        }
    }

    void SetLockedAppearance()
    {
        if (doorRenderer != null && lockedMaterial != null)
        {
            doorRenderer.material = lockedMaterial;
        }

        if (doorLight != null)
        {
            doorLight.color = Color.red;
            doorLight.intensity = 1f;
        }
    }

    void SetUnlockedAppearance()
    {
        if (doorRenderer != null && unlockedMaterial != null)
        {
            doorRenderer.material = unlockedMaterial;
        }

        if (doorLight != null)
        {
            doorLight.color = Color.green;
            doorLight.intensity = 5f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            if (isLocked)
            {
                Debug.Log("The door is locked! Collect all books first.");
            }
            else
            {
                Debug.Log("Library complete! Loading parking lot...");
                StartCoroutine(LoadNextLevel(other.gameObject));
            }
        }
    }

    IEnumerator LoadNextLevel(GameObject player)
    {
        isTransitioning = true;

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }

        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeAndWaitDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeAndWaitDuration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
    }
}