using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Hannah's script
//This is attached to two objects in the Main Menu scene
//The objects are 'Lid' and 'Flask Export v2'
public class MainMenuBehavior : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private bool isZoomed = false;
    public Button startButton;
    public string zoomInTitle;
    public string zoomOutTitle;
    public GameObject textObject;
    public AudioSource backgroundAudio;

    public GameObject currentVial;


    void Start()
    {
        startButton.enabled = false;
        textObject.SetActive(false);
    }

    public void SetCurrentVial(GameObject vial)
    {
        currentVial = vial;
    }

    public void NullifyVial()
    {
        currentVial = null;
    }

    public GameObject CheckIfNull()
    {
        return currentVial;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isZoomed) //makes sure we are not already zoomed in when player presses esc
        {
            textObject.GetComponent<TextMeshProUGUI>().text = "click to interact";
            anim.Play(zoomOutTitle);
            StartCoroutine(WaitForAnim(false));
        }

        if (Input.GetKeyDown(KeyCode.G)) //makes sure we are not already zoomed in when player presses esc
        {
            SceneManager.LoadScene(0);
        }
    }

    void OnMouseOver()
    {
        textObject.SetActive(true);
    }

    void OnMouseExit()
    {
        textObject.SetActive(false);
    }

    void OnMouseDown() // flask is clicked
    {
        if (!isZoomed) //if not already zoomed only
        {
            textObject.GetComponent<TextMeshProUGUI>().text = "";
            anim.Play(zoomInTitle);
            StartCoroutine(WaitForAnim(true));
            if (gameObject.GetComponent<AudioSource>() != null) gameObject.GetComponent<AudioSource>().Play();
        }
    }

    IEnumerator WaitForAnim(bool target) //coroutine to make sure player cannot double input.. not necessary but hunter requested lol
    {
        yield return new WaitForSeconds(.02f);
        isZoomed = target;
        startButton.enabled = target;
    }

    public void NextLevel()
    {
        anim.Play("MainMenuFadeOut");
        
        StartCoroutine(WaitForAnimation());
    }

    IEnumerator WaitForAnimation()
    {
        float timeCrunch = backgroundAudio.volume;
        while (timeCrunch > 0)
        {
            Debug.Log("RUN");
            backgroundAudio.volume = timeCrunch;
            timeCrunch -= .01f;
            yield return new WaitForSeconds(.01f);
        }
        SceneManager.LoadScene(1);
    }
}

