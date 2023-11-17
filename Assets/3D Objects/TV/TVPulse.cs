using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVPulse : MonoBehaviour
{
    public Light tvSpotlight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float pulseSpeed = 1.0f;

    void Start()
    {
        if (tvSpotlight == null)
        {
            tvSpotlight = GetComponent<Light>();
        }

        if (tvSpotlight != null)
        {
            // Set the initial intensity
            tvSpotlight.intensity = minIntensity;
        }
        else
        {
            Debug.LogError("TV Spotlight not assigned.");
        }
    }

    void Update()
    {
        UpdateIntensity();
    }

    void UpdateIntensity()
    {
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
        float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        if (tvSpotlight != null)
        {
            tvSpotlight.intensity = targetIntensity;
        }
    }
}
