using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundAudio;

    private void Start()
    {
        Time.timeScale = 0.0f;
    }
    public void PlayAudio()
    {
        backgroundAudio.Play();
        Time.timeScale = 1.0f;
    }
}
