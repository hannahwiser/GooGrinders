/**
 * File: CameraFlashAnimation
 * Programmer: Sagar Patel
 * Description: Script for handling the camera flash animation. Based on this video of a camera flash: https://www.youtube.com/watch?v=SNCASMUKpBY
 * Date: Nov 16, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlashAnimation : MonoBehaviour
{
    public Light cameraFlashSpotlight;
    public float flashIntensity = 3366151f;
    public float flashDuration = 0.05f;
    public float flashInterval = 5f;
    public float initialBrightness = 0f;
    public float fadeOutDuration = 0.2f; // Duration for exponential fade-out

    public bool flashDebugging = true; // Toggle to control flash interval

    private bool isFlashing = false;
    private float timer = 0f;
    private float flashTimer = 0f;
    private float originalIntensity;

    void Start()
    {
        if (cameraFlashSpotlight == null)
        {
            cameraFlashSpotlight = GetComponent<Light>();
        }

        if (cameraFlashSpotlight != null)
        {
            originalIntensity = cameraFlashSpotlight.intensity;
            cameraFlashSpotlight.intensity = initialBrightness;
        }
        else
        {
            Debug.LogError("Camera Spotlight not assigned.");
        }
    }

    void Update()
    {
        if (flashDebugging)
        {
            timer += Time.deltaTime;

            if (timer >= flashInterval)
            {
                timer = 0f;
                TriggerFlash();
            }
        }

        if (isFlashing)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer <= flashDuration)
            {
                // Calculate exponential flash effect
                float t = flashTimer / flashDuration;
                float exponentialIntensity = Mathf.Lerp(initialBrightness, flashIntensity, Mathf.Pow(t, 4)); // Exponential curve (power can be adjusted)

                cameraFlashSpotlight.intensity = exponentialIntensity;
            }
            else if (flashTimer <= flashDuration + fadeOutDuration)
            {
                // Calculate exponential fade-out effect
                float t = (flashTimer - flashDuration) / fadeOutDuration;
                float exponentialIntensity = Mathf.Lerp(flashIntensity, initialBrightness, Mathf.Pow(t, 2)); // Slightly slower fade-out

                cameraFlashSpotlight.intensity = exponentialIntensity;
            }
            else
            {
                isFlashing = false;
                flashTimer = 0f;
                cameraFlashSpotlight.intensity = initialBrightness;
            }
        }
    }

    // Method to trigger the flash effect
    public void TriggerFlash()
    {
        isFlashing = true;
    }
}