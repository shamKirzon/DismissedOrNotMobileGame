using UnityEngine;
using TMPro;

public class BookManager : MonoBehaviour
{
    [Header("Collection Settings")]
    public int totalBooks = 30;
    private int collectedBooks = 0;

    [Header("UI References")]
    public TextMeshProUGUI collectionText;

    [Header("Exit Door")]
    public GameObject exitDoor;
    private ExitDoor exitDoorScript;

    void Start()
    {
        UpdateUI();

        if (exitDoor != null)
        {
            exitDoorScript = exitDoor.GetComponent<ExitDoor>();
        }
    }

    public void CollectBook()
    {
        collectedBooks++;
        UpdateUI();

        Debug.Log("Book collected! " + collectedBooks + "/" + totalBooks);

        if (collectedBooks >= totalBooks)
        {
            UnlockExit();
        }
    }

    void UpdateUI()
    {
        if (collectionText != null)
        {
            collectionText.text = string.Format("{0:D2} / {1:D2} Books Collected",
                                                collectedBooks, totalBooks);
        }
    }

    void UnlockExit()
    {
        Debug.Log("📚 All books collected! Exit unlocked!");

        if (exitDoorScript != null)
        {
            exitDoorScript.UnlockDoor();
            Debug.Log("✅ Door unlocked successfully!");
        }
    }

    public bool AreAllBooksCollected()
    {
        return collectedBooks >= totalBooks;
    }

    public int GetCollectedCount()
    {
        return collectedBooks;
    }
}