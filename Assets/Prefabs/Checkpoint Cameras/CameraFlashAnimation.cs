using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlashAnimation : MonoBehaviour
{
    public Light cameraFlashSpotlight;
    public float flashIntensity = 22.15f;

    // Adjust as needed for the duration of the flash
    public float flashDuration = 0.1f;
    
    // Interval between flashes
    public float flashInterval = 10f;

    // Initial brightness
    public float initialBrightness = 0f; 

    private bool isFlashing = false;
    private float timer = 0f;

    void Start()
    {
        if (cameraFlashSpotlight == null)
        {
            cameraFlashSpotlight = GetComponent<Light>();

            // Set the initial brightness
            cameraFlashSpotlight.intensity = initialBrightness; 
        }
        else
        {
            Debug.LogError("Camera Spotlight not assigned.");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= flashInterval)
        {
            timer = 0f;
            TriggerFlash();
        }

        if (isFlashing)
        {
            // Start the flash effect
            cameraFlashSpotlight.intensity = flashIntensity;

            // After the flash duration, return to the original intensity
            Invoke("EndFlash", flashDuration);
        }
    }

    void EndFlash()
    {
        isFlashing = false;
        cameraFlashSpotlight.intensity = initialBrightness;
    }

    // Method to trigger the flash effect
    public void TriggerFlash()
    {
        isFlashing = true;
    }
}