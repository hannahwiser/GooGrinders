using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//hannah's script
public class BlockRaycasts : MonoBehaviour
{
    public GameObject[] rayObjects;
    public Button startButton;
    public AudioSource glug, alarm, squish, thud;

    void Start()
    {
        rayObjects = GameObject.FindGameObjectsWithTag("Raycast Target");
        BlockRaycast();
    }

    public void BlockRaycast()
    {
        for (int i = 0; i < rayObjects.Length; i++)
        {
            rayObjects[i].GetComponent<Collider>().enabled = false;
        }
    }

    public void PlayGlug()
    {
        glug.Play();
    }

    public void PlayAlarm()
    {
        alarm.time = 1;
        alarm.Play();
    }

    public void PlaySquish()
    {
        squish.Play();
    }
    public void PlayThud()
    {
        thud.time = .5f;
        thud.Play();
    }

    public void EnableButton()
    {
        startButton.interactable = true;
    }

    public void DisableButton()
    {
        startButton.interactable = false;
    }

    public void UnBlockRaycast()
    {
        for (int i = 0; i < rayObjects.Length; i++)
        {
            rayObjects[i].GetComponent<Collider>().enabled = true;
        }
    }
}
