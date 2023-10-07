/**
 * File: ItemCollector
 * Programmer: Sagar Patel
 * Description: Simple item collection script made using this video guide: https://www.youtube.com/watch?v=YQEq6Lkd69c
 * Date: Sept 19, 2023
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int playerScore = 0;

    // when the game starts, load the player's score from PlayerPrefs
    private void Start()
    {
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GooGorger"))
        {
            Destroy(other.gameObject);
            playerScore++;
            Debug.Log("Score: " + playerScore);

            // update the score in PlayerPrefs
            PlayerPrefs.SetInt("PlayerScore", playerScore);
            PlayerPrefs.Save();
        }
    }

    private void OnApplicationQuit()
    {
        // when the game is closed, set the score to 0
        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.Save();
    }
}