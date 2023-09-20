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

    private void Update()
    {
        if (transform.position.y < -7.5f && !dead)
        {
            Die();
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
        Invoke(nameof(ReloadLevel), 1.3f);
        dead = true;
        Debug.Log("The player has died.");
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
