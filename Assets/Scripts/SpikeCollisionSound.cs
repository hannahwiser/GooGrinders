/**
 * File: SpikeCollisionSound
 * Programmer: Sagar Patel
 * Description: Play the stab sound when the player hits a spike, but also have a cooldown to prevent multiple sounds in quick succession
 * Date: Oct 11, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCollisionSound : MonoBehaviour
{
    private AudioSource audioSource;
    private bool canPlayAudio = true;
    private float cooldownTime = 5.0f;
    private float lastAudioPlayTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canPlayAudio && collision.gameObject.CompareTag("Player"))
        {
            PlayStabbingSound();
        }
    }

    private void Update()
    {
        // check if the cooldown time has passed
        if (!canPlayAudio && Time.time - lastAudioPlayTime >= cooldownTime)
        {
            canPlayAudio = true;
        }
    }

    private void PlayStabbingSound()
    {
        if (audioSource != null)
        {
            audioSource.Play();

            // set the cooldown and remember the last play time
            canPlayAudio = false;
            lastAudioPlayTime = Time.time;
        }
    }
}
