using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassCheckpoint : MonoBehaviour
{
    public Animator anim;
    public AudioSource flashAudio;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.Play("CameraFlash");
            flashAudio.Play();
        }
    }
}
