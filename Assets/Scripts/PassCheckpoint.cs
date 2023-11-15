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
        if (collision.gameObject.tag == "Player") //if the player passes the checkpoint
        {
            checkPointScript.SetCheckPoint(myLocation);//set the spawn point as the correct spawn point
            anim.Play("CameraFlash");//flash people
            flashAudio.Play();//music !
        }
    }
}
