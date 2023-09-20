/**
 * File: ScoreManager
 * Programmer: Sagar Patel
 * Description: Script for handling and submitting player scores to the leaderboard using the following guide: https://youtu.be/-O7zeq7xMLw?si=6A_rzgRErlBdG_is
 * This script acts as a bridge between the UI elements (inputScore and inputName) and other parts of the game 
 * that handle score submission. When the player interacts with the UI to submit their score, the SubmitScore 
 * method is called, which, in turn, triggers the submitScoreEvent. Other components in the game can subscribe 
 * to this event to perform actions like updating a high score leaderboard or saving the player's score.
 * Date: Sept 19, 2023
 */

using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inputScore;
    [SerializeField] private TMP_InputField inputName;

    public UnityEvent<string, int> submitScoreEvent; 

    public void SubmitScore()
    {
        submitScoreEvent.Invoke(inputName.text, int.Parse(inputScore.text));
    }
}