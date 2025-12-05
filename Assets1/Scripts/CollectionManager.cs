using UnityEngine;
using TMPro;

public class CollectionManager : MonoBehaviour
{
    [Header("Collection Settings")]
    public int totalKeycards = 30;
    private int collectedKeycards = 0;

    [Header("UI References")]
    public TextMeshProUGUI collectionText;

    [Header("Exit Door")]
    public GameObject exitDoor;
    private ExitDoor exitDoorScript;

    void Start()
    {
        UpdateUI();

        // Get the ExitDoor script component (only if exitDoor is assigned)
        if (exitDoor != null)
        {
            exitDoorScript = exitDoor.GetComponent<ExitDoor>();

            if (exitDoorScript == null)
            {
                Debug.LogWarning("ExitDoor object doesn't have ExitDoor script attached!");
            }
        }
        // Don't show error if no door assigned - Might be Valid for Some levels
    }

    public void CollectKeycard()
    {
        collectedKeycards++;
        UpdateUI();

        Debug.Log("Keycard collected! " + collectedKeycards + "/" + totalKeycards);

        if (collectedKeycards >= totalKeycards)
        {
            UnlockExit();
        }
    }

    void UpdateUI()
    {
        if (collectionText != null)
        {
            collectionText.text = string.Format("{0:D2} / {1:D2} Keycards Collected",
                                                collectedKeycards, totalKeycards);
        }
    }

    void UnlockExit()
    {
        Debug.Log("🎉 All keycards collected! Exit unlocked!");

        // Unlock the door (DON'T destroy it!)
        if (exitDoorScript != null)
        {
            exitDoorScript.UnlockDoor();
            Debug.Log("✅ Door unlocked successfully!");
        }
        else
        {
            Debug.LogError("❌ ExitDoor script is null! Can't unlock door!");
        }
    }

    public bool AreAllKeysCollected()
    {
        return collectedKeycards >= totalKeycards;
    }

    public int GetCollectedCount()
    {
        return collectedKeycards;
    }
}