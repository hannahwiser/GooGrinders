using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Animator anim;
    public bool currentlyPaused;

    public AudioSource bgAudio;
    public Slider volumeSlider;


    // Start is called before the first frame update
    void Start()
    {
        bgAudio = GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>();
        volumeSlider.value = bgAudio.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!currentlyPaused)
            {
                Time.timeScale = 0;
                currentlyPaused = true;
                anim.Play("PauseFadeIn");
            }
            else
            {
                Time.timeScale = 1;
                currentlyPaused = false;
                anim.Play("PauseFadeOut");
            }
        }
    }

    public void BackToGame()
    {
        Time.timeScale = 1;
        currentlyPaused = false;
        anim.Play("PauseFadeOut");
    }

    public void ToScene(int scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void ChangeVolume()
    {
        bgAudio.volume = volumeSlider.value;
    }

}
