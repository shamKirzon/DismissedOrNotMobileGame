using UnityEngine;

public class RandomFlicker : MonoBehaviour
{
    public Light myLight;             // Drag your Light dito
    public float minIntensity = 0f;   // pinaka-dilaw kapag flicker
    public float maxIntensity = 1f;   // pinaka-liwanag
    public float minTime = 0.05f;     // pinakamabilis na flicker interval
    public float maxTime = 0.3f;      // pinakamabagal na flicker interval

    private float timer;

    void Start()
    {
        if (myLight == null)
            myLight = GetComponent<Light>();

        timer = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            myLight.intensity = Random.Range(minIntensity, maxIntensity);

            timer = Random.Range(minTime, maxTime);
        }
    }
}
