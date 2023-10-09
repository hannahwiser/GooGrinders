using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Animator anim;
    public bool currentlyPaused;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!currentlyPaused)
            {
                currentlyPaused = true;
                anim.Play("PauseFadeIn");
            }
            else
            {
                currentlyPaused = false;
                anim.Play("PauseFadeOut");
            }
        }
    }

    
}
