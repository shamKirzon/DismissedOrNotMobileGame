using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 3;
    private int currentHearts;

    [Header("Hit Settings")]
    public int hitsPerHeart = 3;
    private int currentHits = 0;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float respawnDelay = 2f;

    [Header("Damage Settings")]
    public float invincibilityTime = 2f;
    private float invincibilityTimer = 0f;
    private bool isInvincible = false;

    [Header("UI References")]
    public Image[] heartImages;
    public Image fadeImage;
    public GameObject gameOverPanel;
    public Image damageFlash; // NEW: Red flash when hit

    [Header("References")]
    private CharacterController characterController;
    private bool isGameOver = false;

    void Start()
    {
        currentHearts = maxHearts;
        currentHits = 0;
        isGameOver = false;

        characterController = GetComponent<CharacterController>();

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Make sure damage flash is hidden
        if (damageFlash != null)
        {
            Color c = damageFlash.color;
            c.a = 0f;
            damageFlash.color = c;
            damageFlash.gameObject.SetActive(false);
        }

        UpdateHeartUI();

        if (respawnPoint == null)
        {
            GameObject respawnObj = new GameObject("RespawnPoint");
            respawnPoint = respawnObj.transform;
            respawnPoint.position = transform.position;
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int damage = 1)
    {
        if (isInvincible || isGameOver)
        {
            return;
        }

        currentHits += damage;

        Debug.Log("Player got hit! Hits: " + currentHits + "/" + hitsPerHeart);

        if (currentHits >= hitsPerHeart)
        {
            currentHearts--;
            currentHits = 0;

            Debug.Log("Lost a heart! Hearts remaining: " + currentHearts + "/" + maxHearts);

            UpdateHeartUI();

            if (currentHearts <= 0)
            {
                Die();
            }
            else
            {
                LoseHeart();
            }
        }
        else
        {
            isInvincible = true;
            invincibilityTimer = invincibilityTime;
            StartCoroutine(FlashPlayer());
            StartCoroutine(FlashDamageScreen()); // NEW: Red screen flash
        }
    }

    void LoseHeart()
    {
        Debug.Log("Lost a heart! Respawning with " + currentHearts + " hearts...");

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        StartCoroutine(RespawnSequence());
    }

    void Die()
    {
        Debug.Log("GAME OVER!");
        isGameOver = true;

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(1f);
        ShowGameOver();
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator RespawnSequence()
    {
        yield return StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(0.5f);

        Respawn(false);

        yield return StartCoroutine(FadeFromBlack());

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    void Respawn(bool restoreAllHearts)
    {
        currentHits = 0;

        if (restoreAllHearts)
        {
            currentHearts = maxHearts;
        }

        UpdateHeartUI();

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        isInvincible = true;
        invincibilityTimer = invincibilityTime;
    }

    IEnumerator FadeToBlack()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }
    }

    IEnumerator FadeFromBlack()
    {
        if (fadeImage == null) yield break;

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator FlashPlayer()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            renderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator FlashDamageScreen()
    {
        if (damageFlash == null) yield break;

        damageFlash.gameObject.SetActive(true);

        // Flash in quickly
        float duration = 0.15f;
        float elapsed = 0f;
        Color c = damageFlash.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 0.5f, elapsed / duration);
            damageFlash.color = c;
            yield return null;
        }

        // Flash out
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0.5f, 0f, elapsed / duration);
            damageFlash.color = c;
            yield return null;
        }

        damageFlash.gameObject.SetActive(false);
    }

    void UpdateHeartUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].enabled = (i < currentHearts);
            }
        }
    }

    public int GetCurrentHearts()
    {
        return currentHearts;
    }

    public int GetCurrentHits()
    {
        return currentHits;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}