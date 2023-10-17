/**
 * File: PlayerLife
 * Programmer: Sagar Patel
 * Description: Player death script made using this video guide: https://www.youtube.com/watch?v=YQEq6Lkd69c
 * Date: Sept 19, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerLife : MonoBehaviour
{
    bool dead = false;
    public float deathLevelY = -7.5f;

    //hannah added this
    public Transform spawnPoint;

    // reference the Player script
    private Player playerScript;

    // reference to ClampFollowTargetX script
    private ClampFollowTargetX clampFollowTargetX;

    // reference the Cinemachine Cam (so we can teleport it back to spawn)
    public CinemachineVirtualCamera virtualCamera;
    private Vector3 initialCameraPosition;

    // reference to GoonamiController script
    public GoonamiController goonamiController;
    public AudioSource goonamiDeathSound;
    public float goonamiDeadzoneOffset = 5.0f;

    void Start()
    {
        // find the GoonamiController script
        goonamiController = FindObjectOfType<GoonamiController>();

        // get a reference to the Player script on the player GameObject
        playerScript = GetComponent<Player>();
        clampFollowTargetX = GetComponentInChildren<ClampFollowTargetX>();

        // get the initial camera position
        initialCameraPosition = virtualCamera.transform.position;
    }

    void Update()
    {
        if (transform.position.y < deathLevelY && !dead)
        {
            Die();
        }
        if (Input.GetKey(KeyCode.G))
        {
            SceneManager.LoadScene(0);
        }

        // check if the Goonami fog's X position > the player's X position
        if (
            goonamiController != null
            && transform.position.x < goonamiController.transform.position.x + goonamiDeadzoneOffset
            && !dead
        )
        {
            // play the Goonami death sound
            if (goonamiDeathSound != null)
            {
                goonamiDeathSound.Play();
            }

            Debug.Log("Player was caught by the goo-nami.");
            Die();
        }
    }

    void Die()
    {
        // disable player control
        playerScript.SetPlayerControlEnabled(false);

        float decelerationForce = 4.0f;

        // apply the deceleration force in the opposite direction of the player's current velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(-rb.velocity.normalized * decelerationForce, ForceMode.Acceleration);

        // respawn after a delay
        Invoke(nameof(Respawn), 1.0f);
        //Invoke(nameof(ReloadLevel), 1.3f);

        dead = true;
        Debug.Log("Die() method was called");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Impact_Death") && !dead)
        {
            Debug.Log("Player died on impact.");
            Die();
        }
        if (collision.gameObject.CompareTag("Spike_Death") && !dead)
        {
            Debug.Log("Player was impaled to death.");
            Die();
        }
    }

    void Respawn()
    {
        // Disable Player.cs
        playerScript.enabled = false;

        // reset ClampFollowTargetX
        if (clampFollowTargetX != null)
        {
            clampFollowTargetX.ResetPosition();
        }

        // reset the Goo-nami's position
        if (goonamiController != null)
        {
            goonamiController.ResetPositionToStart();
        }

        // set the player's position to the spawn point
        transform.position = spawnPoint.position;

        // reset the player's velocity
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;

        virtualCamera.transform.position = initialCameraPosition; // set initialCameraPosition to the original camera position

        // re-enable Player.cs
        playerScript.enabled = true;

        // re-enable player control
        playerScript.SetPlayerControlEnabled(true);

        playerScript.SetPlayeOnRail(true);

        dead = false;
        Debug.Log("Player respawned.");
    }
}
