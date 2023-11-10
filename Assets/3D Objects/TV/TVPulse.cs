using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVPulse : MonoBehaviour
{
    public Light tvSpotlight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float pulseSpeed = 1.0f;

    private float currentIntensity;
    private float targetIntensity;
    private bool increasing = true;

    void Start()
    {
        if (tvSpotlight == null)
        {
            tvSpotlight = GetComponent<Light>();
        }

        if (tvSpotlight != null)
        {
            currentIntensity = minIntensity;
            targetIntensity = maxIntensity;
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
        currentIntensity = Mathf.Lerp(
            currentIntensity,
            targetIntensity,
            pulseSpeed * Time.deltaTime
        );
        if (Mathf.Approximately(currentIntensity, targetIntensity))
        {
            increasing = !increasing;
            targetIntensity = increasing ? maxIntensity : minIntensity;
        }
        if (tvSpotlight != null)
        {
            tvSpotlight.intensity = currentIntensity;
        }
    }
}
