using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehavior : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool isZoomed = false;
    public Button startButton;

    void Start()
    {
        startButton.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isZoomed) //makes sure we are not already zoomed in when player presses esc
        {
            anim.Play("FlaskDown");
            StartCoroutine(WaitForAnim(false));
        }
    }

    void OnMouseDown() // flask is clicked
    {
        if (!isZoomed)
        {
            //start async loading the game scene here for ease of transfer
            anim.Play("FlaskUp");
            StartCoroutine(WaitForAnim(true));
        }
    }

    IEnumerator WaitForAnim(bool target) //coroutine to make sure player cannot double input.. not necessary but hunter requested lol
    {
        yield return new WaitForSeconds(1.5f);
        isZoomed = target;
        startButton.enabled = target;
    }

    public void NextLevel()
    {
        anim.Play("MainMenuFadeOut");
        //this is where we will go to the next level
    }
}
