using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheckpoint : MonoBehaviour
{
    public Animator anim;
    public AudioSource flashAudio;
    public CheckpointHandler checkPointScript;
    public Transform myLocation;
    public bool hasThisCheckPointPassed = false;

    // Reference to the CameraFlashAnimation script
    public CameraFlashAnimation cameraFlashAnimation;

    void Start()
    {
        // Find the CameraFlashAnimation script in the scene
        cameraFlashAnimation = FindObjectOfType<CameraFlashAnimation>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player" && !hasThisCheckPointPassed) //if the player passes the checkpoint
        {
            hasThisCheckPointPassed = true;
            checkPointScript.SetCheckPoint(myLocation); //set the spawn point as the correct spawn point
            // Trigger the camera flash animation
            if (cameraFlashAnimation != null)
            {
                cameraFlashAnimation.TriggerFlash(); // Trigger the flash effect
            }
            anim.Play("CameraFlash"); // Flash people
            flashAudio.Play(); // Play music!
        }
    }
}
