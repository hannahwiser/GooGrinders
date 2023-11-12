using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheckpoint : MonoBehaviour
{
    public Animator anim;
    public AudioSource flashAudio;
    public CheckpointHandler checkPointScript;
    public int myNumber = 1;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            checkPointScript.SetCheckPoint(myNumber);
            anim.Play("CameraFlash");
            flashAudio.Play();
        }
    }
}
