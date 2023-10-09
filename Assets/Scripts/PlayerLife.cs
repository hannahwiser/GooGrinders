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

public class PlayerLife : MonoBehaviour
{
    bool dead = false;
    public float deathLevelY = -7.5f;

    //hannah added this
    public Transform spawnPoint;
    public GameObject sporetParent;

    // reference the ragdoll script attached to the player
    //public RagdollController ragdollController;
    // reference the Player script
    private Player playerScript;

    // reference to ClampFollowTargetX script
    private ClampFollowTargetX clampFollowTargetX;

    void Start()
    {
        // get a reference to the Player script on the player GameObject
        playerScript = GetComponent<Player>();
        clampFollowTargetX = GetComponentInChildren<ClampFollowTargetX>();
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
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy Body"))
    //    {
    //        GetComponent<MeshRenderer>().enabled = false;
    //        GetComponent<Rigidbody>().isKinematic = true;
    //        GetComponent<PlayerMovement>().enabled = false;
    //        Die();
    //    }
    //}

    void Die()
    {
        // enable ragdoll physics
        //ragdollController.EnableRagdoll();

        // disable player control
        playerScript.SetPlayerControlEnabled(false);

        // respawn after a delay
        Invoke(nameof(Respawn), 1.0f);
        //Invoke(nameof(ReloadLevel), 1.3f);

        dead = true;
        Debug.Log("The player has died.");
    }

    void Respawn()
    {
        // reset ClampFollowTargetX
        if (clampFollowTargetX != null)
        {
            clampFollowTargetX.ResetPosition();
        }

        // teleport the player to the spawn point
        transform.position = spawnPoint.position;

        // disable ragdoll mode
        //ragdollController.DisableRagdoll();

        // Re-enable player control
        playerScript.SetPlayerControlEnabled(true);

        dead = false;
        Debug.Log("Player respawned.");
    }
}
