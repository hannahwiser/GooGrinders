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

    private const int MAX_ENTRIES = 10; // Maximum number of entries on the leaderboard
    private List<string> usernames = new List<string>();
    private List<int> userScores = new List<int>();

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
        LoadLeaderboard();
        UpdateLeaderboardUI();
        // Reset PlayerPrefs
        //PlayerPrefs.DeleteAll();
    }

    private void LoadLeaderboard()
    {
        usernames.Clear();
        userScores.Clear();

        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            usernames.Add("");
            userScores.Add(0);
        }

        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            string username = PlayerPrefs.GetString($"Username_{i}");
            int score = PlayerPrefs.GetInt($"Score_{i}");

            if (!string.IsNullOrEmpty(username))
            {
                usernames[i] = username;
                userScores[i] = score;
            }
        }
    }

    private void UpdateLeaderboardUI()
    {
        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            names[i].text = usernames[i];
            scores[i].text = userScores[i].ToString();
        }
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
        // Check if the score qualifies for the leaderboard
        if (IsHighScore(score))
        {
            int insertIndex = GetInsertIndex(score);

            ShiftEntriesDown(insertIndex);

            usernames[insertIndex] = username;
            userScores[insertIndex] = score;

            SaveLeaderboard();

            UpdateLeaderboardUI();
        }
    }

    private int GetInsertIndex(int score)
    {
        int index = 0;
        while (index < userScores.Count && score <= userScores[index])
        {
            index++;
        }
        return index;
    }

    private void SaveLeaderboard()
    {
        for (int i = 0; i < MAX_ENTRIES; i++)
        {
            PlayerPrefs.SetString($"Username_{i}", usernames[i]);
            PlayerPrefs.SetInt($"Score_{i}", userScores[i]);
        }
        PlayerPrefs.Save();
    }

    private void ShiftEntriesDown(int startIndex)
    {
        for (int i = MAX_ENTRIES - 2; i >= startIndex; i--)
        {
            usernames[i + 1] = usernames[i];
            userScores[i + 1] = userScores[i];
        }
    }

    private bool IsHighScore(int score)
    {
        if (userScores.Count < MAX_ENTRIES || score > userScores[userScores.Count - 1])
        {
            return true;
        }
        return false;
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
