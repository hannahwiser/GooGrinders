using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheckpoint : MonoBehaviour
{
    public Animator anim;
    public AudioSource flashAudio;
    public CheckpointHandler checkPointScript;
    public Transform myLocation;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            checkPointScript.SetCheckPoint(myLocation);
            anim.Play("CameraFlash");
            flashAudio.Play();
        }
    }
}
