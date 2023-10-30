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
    public bool dead = false;
    public float deathLevelY = -7.5f;
    public PlayerAnimController animController;

    //hannah added this
    public Transform spawnPoint;

    // reference the Player script
    private Player playerScript;

    // reference to ClampFollowTargetX script
    private ClampFollowTargetX clampFollowTargetX;

    // reference the Cinemachine Cam (so we can teleport it back to spawn)
    public CinemachineVirtualCamera virtualCamera;
    private Vector3 initialCameraPosition;
    private Vector3 originalDamping;

    // reference to GoonamiController script
    public GoonamiController goonamiController;
    public AudioSource goonamiDeathSound;
    public float goonamiDeadzoneOffset = 7.5f; // determines where the Goonami's deadzone is. It's sortof a barbaric quick and dirty way of doing this
    private bool goonamiCanKill = true; // Prevent the Goonami from killing the player at a bad time, like immediately after respawning

    // checkbox for if the level starts with a cutscene
    public bool levelStartsWithCutscene = true;

    // store drag value of the player's Rigidbody
    private float originalDrag;

    // The sound effect for falling down the hole
    public GameObject cutsceneAudioSource;

    void Start()
    {
        Cinemachine3rdPersonFollow thirdPersonFollow =
            virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (thirdPersonFollow != null)
        {
            originalDamping = thirdPersonFollow.Damping;
        }

        // find the GoonamiController script
        goonamiController = FindObjectOfType<GoonamiController>();
        if (!animController)
            animController = GetComponent<PlayerAnimController>();
        // get a reference to the Player script on the player GameObject
        playerScript = GetComponent<Player>();
        clampFollowTargetX = GetComponentInChildren<ClampFollowTargetX>();

        // get the initial camera position
        initialCameraPosition = virtualCamera.transform.position;

        if (levelStartsWithCutscene)
        {
            // disable player.cs so the player can't move
            playerScript.SetPlayerControlEnabled(false);
            // save the original drag value and temporarily set it to 0.1 so we don't fall too fast, otherwise you can see outside the map
            originalDrag = playerScript.GetComponent<Rigidbody>().drag;
            playerScript.GetComponent<Rigidbody>().drag = 0.13f;
            cutsceneAudioSource.GetComponent<AudioSource>().Play();

            // enable controls after X seconds NOTE: If the game is paused or if the computer lags, the player will potentially be able to get out of the map, so fix this later
            StartCoroutine(EnablePlayerControlsAfterDelay(7.0f));
        }
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

        // check if it makes sense for the Goonami to kill the player
        if (goonamiCanKill)
        {
            // check if the Goonami fog's X position > the player's X position
            if (
                goonamiController != null
                && transform.position.x
                    < goonamiController.transform.position.x + goonamiDeadzoneOffset
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
        Invoke(nameof(Respawn), 0.0f);
        //Invoke(nameof(ReloadLevel), 1.3f);

        dead = true;
        /* Debug.Log("Die() method was called"); */
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
        // prevent the Goonami from killing the player while we respawn them
        goonamiCanKill = false;

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

        // IMPORTANT: Because we save the initialCameraPosition based on where the camera was when this script was first loaded in, the camera will be sent there. But spawns change, so there's an issue
        virtualCamera.transform.position = initialCameraPosition; // set initialCameraPosition to the original camera position
        animController.animator.speed = 1;

        // re-enable player control
        playerScript.SetPlayerControlEnabled(true);

        playerScript.OnRail = true;
        playerScript.startAttached = true;

        // set the damping values to 0 when you first respawn
        Cinemachine3rdPersonFollow thirdPersonFollow =
            virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (thirdPersonFollow != null)
        {
            thirdPersonFollow.Damping = Vector3.zero;
        }

        // restore original damping values after 0.5 seconds
        StartCoroutine(RestoreDampingValues(thirdPersonFollow, originalDamping, 0.5f));

        dead = false;
        Debug.Log("Player respawned.");

        // set goonamiCanKill to true after 4 seconds
        StartCoroutine(EnableGoonamiKillAfterDelay(4.0f));
    }

    // enable player controls after a delay
    IEnumerator EnablePlayerControlsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerScript.SetPlayerControlEnabled(true);
        playerScript.GetComponent<Rigidbody>().drag = originalDrag;

        // set levelStartsWithCutscene to false after the first time
        levelStartsWithCutscene = false;
    }

    IEnumerator EnableGoonamiKillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        goonamiCanKill = true;
    }

    IEnumerator RestoreDampingValues(
        Cinemachine3rdPersonFollow follow,
        Vector3 original,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);
        if (follow != null)
        {
            follow.Damping = original;
        }
    }
}
