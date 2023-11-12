/**
 * File: Leaderboard
 * Programmer: Sagar Patel
 * Description: Online Leaderboard in Unity using the following guide: https://youtu.be/-O7zeq7xMLw?si=6A_rzgRErlBdG_is
 * I also implemented custom NSFW word and offensive slur word filters which I got from here: https://github.com/abstraq/chat_filters
 * The two TXT files of bad words will be in Assets > Resources
 * Date: Sept 19, 2023
 * 
 * Note to self: Apparently there's been an update to the package, so make sure to update it based on this vid, and then
 * delete this message once it's been updated according to this video: https://youtu.be/v0aWwSkC-4o?si=o8Gf7jKUQD9ykQrb
 * Example code from the original dev of this leaderboard system is here: https://gist.github.com/danqzq/f464dd89581f2e26cd02dedd4d33d7a2
 */

//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using System.Linq;

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

    // This method sets a new leaderboard entry, uploading the username and score to the leaderboard.
    // It also checks if the username contains profanity or slurs and avoids uploading the entry if it
    // does. Additionally, it processes the username, ensuring it's within a specified length, and
    // fetches the updated leaderboard data.
    public void SetLeaderboardEntry(string username, int score)
    {
        LoadProfanityWords();

        string usernameToUpload = username;

        if (ContainsProfanityOrSlurs(username))
        {
            Debug.LogWarning(
                "SetLeaderboardEntry(): Username contains profanity or slurs. Defaulting the name to Sporet..."
            );
            //return;

            // It's better to set their name to a default name instead of giving an error and letting them try again.
            // The username will be set to "Sporet" in case of blocked words
            usernameToUpload = "Sporet";
        }

        // Process the username
        usernameToUpload = usernameToUpload.Substring(0, Mathf.Min(usernameToUpload.Length, 7));

        LeaderboardCreator.UploadNewEntry(
            publicLeaderboardKey,
            usernameToUpload,
            score,
            (
                (msg) =>
                {
                    GetLeaderBoard();
                }
            )
        );
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
            if (text.Contains(word.ToLower()))
            {
                Debug.LogWarning($"ContainsProfanityOrSlurs() detected an NSFW word: {word}");
                return true;
            }
        }

        foreach (string word in slurWords)
        {
            if (text.Contains(word.ToLower()))
            {
                Debug.LogWarning($"ContainsProfanityOrSlurs() detected a slur: {word}");
                return true;
            }
        }

        return false;
    }
}
