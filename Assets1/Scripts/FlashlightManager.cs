using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;

    void Start()
    {
        // Simply turn on the flashlight - that's it!
        if (flashlight != null)
        {
            flashlight.enabled = true;
        }

        Debug.Log("Flashlight is ON!");
    }
}