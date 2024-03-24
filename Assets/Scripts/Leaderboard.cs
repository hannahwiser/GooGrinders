/**
 * File: Leaderboard
 * Programmer: Sagar Patel
 * Description: Online Leaderboard in Unity using the following guide: https://youtu.be/-O7zeq7xMLw?si=6A_rzgRErlBdG_is
 * I also implemented custom NSFW word and offensive slur word filters which I got from here: https://github.com/abstraq/chat_filters
 * The two TXT files of bad words will be in Assets > Resources
 * Date: Sept 19, 2023
 */

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using System.Linq;
using System;
using System.Collections;
using Dan.Models;
using System.Security.Cryptography.X509Certificates;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;

    [SerializeField]
    private List<TextMeshProUGUI> scores;

    // Arrays to store the profanity and slur words
    private string[] nsfwWords;
    private string[] slurWords;

    string usernameToUpload;
    int scoreToUpload;

    public TMP_FontAsset normalFont;
    public TMP_FontAsset winnerFont;

    // Initializes the leaderboard script by loading profanity words and updating the UI with local leaderboard data
    private void Start()
    {
        LoadProfanityWords();
        UpdateLocalLeaderboardUI();
    }

    // Method to update the local leaderboard UI
    private void UpdateLocalLeaderboardUI()
    {
        string username = PlayerPrefs.GetString("PlayerUsername", "Sporet");
        int score = PlayerPrefs.GetInt("PlayerScore", 0);

        // Display the player's username and score on the UI
        names[0].text = username;
        scores[0].text = score.ToString();
    }

    // Method to set a new leaderboard entry locally
    public void SetLeaderboardEntry(string username, int score)
    {
        if (ContainsProfanityOrSlurs(username))
        {
            Debug.LogWarning(
                "SetLeaderboardEntry(): Username contains profanity or slurs. Defaulting the name to Sporet..."
            );
            username = "Sporet"; // Default username for blocked words
        }

        PlayerPrefs.SetString("PlayerUsername", username);
        PlayerPrefs.SetInt("PlayerScore", score);

        UpdateLocalLeaderboardUI();

    }

    // Method to load profanity and slur words from resources
    private void LoadProfanityWords()
    {
        TextAsset nsfwText = Resources.Load<TextAsset>("blacklist_nsfw_words");
        TextAsset slurText = Resources.Load<TextAsset>("blacklist_slur_words");

        nsfwWords = nsfwText.text.Split(
            new char[] { ',' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
        slurWords = slurText.text.Split(
            new char[] { ',' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
    }

    // Method to check if a given text contains profanity or slurs
    private bool ContainsProfanityOrSlurs(string text)
    {
        foreach (string word in nsfwWords)
        {
            if (text.ToLower().Contains(word.ToLower()))
            {
                Debug.LogWarning($"ContainsProfanityOrSlurs() detected an NSFW word: {word}");
                return true;
            }
        }

        foreach (string word in slurWords)
        {
            if (text.ToLower().Contains(word.ToLower()))
            {
                Debug.LogWarning($"ContainsProfanityOrSlurs() detected a slur: {word}");
                return true;
            }
        }

        return false;
    }

    //hannah's figure out if we're on da leadaboard stuff here
    public void FlashNameIfBased()
    {
        for (int i = 0; i < 10; i++)
        {
            if (usernameToUpload == names[i].text && scoreToUpload.ToString() == scores[i].text)
            {
                Debug.Log("Made it big time");
                ChangeToWinner(i);
                break;
            }
        }
    }
    // changes the font of the username at the specified index in the leaderboard UI to indicate that the player is a winner
    public void ChangeToWinner(int placeInArray)
    {
        names[placeInArray].font = winnerFont;
    }

}
