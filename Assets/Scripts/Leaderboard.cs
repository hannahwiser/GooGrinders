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

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;

    [SerializeField]
    private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey =
        "186b749c493dcea3110a114dafdbaa3584564d23f641110a0bcfa615be2d0b04";

    // Arrays to store the profanity and slur words
    private string[] nsfwWords;
    private string[] slurWords;

    string usernameToUpload;
    int scoreToUpload;

    private void Start()
    {
        GetLeaderBoard();
    }

    // Retrieves the leaderboard data from an external source, likely an online service or server, and
    // updates the UI elements in your game to display the leaderboard entries.
    public void GetLeaderBoard()
    {
        LeaderboardCreator.GetLeaderboard(
            publicLeaderboardKey,
            (
                (msg) =>
                {
                    int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;

                    for (int i = 0; i < loopLength; i++)
                    {
                        names[i].text = msg[i].Username;
                        scores[i].text = msg[i].Score.ToString();
                    }
                }
            )
        );
    }

    //hannah's figure out if we're on da leadaboard stuff here
    public void FlashNameIfBased()
    {
        for (int i = 0; i < 10; i++)
        {
            if (usernameToUpload == names[i].ToString() && scoreToUpload.ToString() ==scores[i].ToString())
            {
                Debug.Log("Made it big time");
                StartCoroutine(FlashName(i));
                break;
            }
        }
    }

    public IEnumerator FlashName(int placeInArray)
    {
        for (int i = 0; i<5; i++)
        {
            Debug.Log("Flashing");
            names[placeInArray].fontStyle = FontStyles.Normal;
            yield return new WaitForSeconds(.25f);
            names[placeInArray].fontStyle = FontStyles.Bold;
            yield return new WaitForSeconds(.25f);
        }
    }


    // This method sets a new leaderboard entry, uploading the username and score to the leaderboard.
    // It also checks if the username contains profanity or slurs and avoids uploading the entry if it
    // does. Additionally, it processes the username, ensuring it's within a specified length, and
    // fetches the updated leaderboard data.
    public void SetLeaderboardEntry(string username, int score)
    {
        LoadProfanityWords();

        //hannah added this so i can access outside this method
        usernameToUpload = username;
        scoreToUpload = score;

        if (ContainsProfanityOrSlurs(username))
        {
            Debug.LogWarning("SetLeaderboardEntry(): Username contains profanity or slurs. Defaulting the name to Sporet...");
            usernameToUpload = "Sporet"; // Default username for blocked words
        }

        // Process the username
        usernameToUpload = usernameToUpload.Substring(0, Mathf.Min(usernameToUpload.Length, 7));

        LeaderboardCreator.UploadNewEntry(
            publicLeaderboardKey,
            usernameToUpload,
            score,
            (isSuccessful) =>
            {
                if (isSuccessful)
                {
                // Reset the player after uploading the entry to enable multiple entries for the same username
                LeaderboardCreator.ResetPlayer(() =>
                    {
                        GetLeaderBoard();
                    });
                }
                else
                {
                    Debug.LogError("Failed to upload the new entry to the leaderboard.");
                }
            }
        );
        //FlashNameIfBased();
    }

    // This method loads the profanity and slur words from the "blacklist_nsfw_words.txt" and
    // "blacklist_slur_words.txt" files into respective arrays for use in filtering.
    private void LoadProfanityWords()
    {
        TextAsset nsfwText = Resources.Load<TextAsset>("blacklist_nsfw_words");
        TextAsset slurText = Resources.Load<TextAsset>("blacklist_slur_words");

        nsfwWords = nsfwText.text
            .Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim())
            .ToArray();
        slurWords = slurText.text
            .Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Trim())
            .ToArray();
    }

    // This method checks if a given text (e.g., a username) contains any of the loaded profanity
    // or slur words, returning true if it does and false otherwise.
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
}
