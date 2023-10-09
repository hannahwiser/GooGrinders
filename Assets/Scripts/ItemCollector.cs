/**
 * File: ItemCollector
 * Programmer: Sagar Patel
 * Description: Simple item collection script made using this video guide: https://www.youtube.com/watch?v=YQEq6Lkd69c
 * Date: Sept 19, 2023
 * 
 * Hi, I am Hannah and I'm gonna modify this script to give more points
 * for every Googorger. Each goo gorger will have a different amount of points.
 * Right now that'll be randomly assigned but we can do merit-based assignments
 * in the future based on how difficult any given googorger is to get.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int playerScore = 0;
    public int gooScore;

    public ScoreHUD scoreHUDScript;

    // when the game starts, load the player's score from PlayerPrefs
    private void Start()
    {
        gooScore = Random.Range(100, 500);
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GooGorger"))
        {
            int amountToAdd = other.GetComponent<ItemCollector>().gooScore;
            Destroy(other.gameObject);
            playerScore+= amountToAdd;
            Debug.Log("Score: " + playerScore);

            // update the score in PlayerPrefs
            PlayerPrefs.SetInt("PlayerScore", playerScore);
            PlayerPrefs.Save();
            scoreHUDScript.SetScore(playerScore);
        }
    }



    private void OnApplicationQuit()
    {
        // when the game is closed, set the score to 0
        PlayerPrefs.SetInt("PlayerScore", 0);
        PlayerPrefs.Save();
    }
}