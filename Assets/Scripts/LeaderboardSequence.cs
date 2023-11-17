using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LeaderboardSequence : MonoBehaviour
{
    public tvScreenController screenController;
    public BoxCollider TV;
    public Leaderboard leaderboard;

    public TMP_InputField myField;
    public TextMeshProUGUI inputScore;

    // Set this to true if the player won the game
    public bool playerWon = false;

    private int currentScreen = 0;
    private bool waitingForClick = false;

    private void OnEnable()
    {
        TV.enabled = true;
    }

    private void OnDisable()
    {
        TV.enabled = false;
    }

    public void StartLeaderboardSequence()
    {
        // set the initial screen based on playerWon
        if (playerWon)
            screenController.MoveToWinningScreen(0);
        else
            screenController.MoveToLosingScreen(0);

        currentScreen = 0;
        waitingForClick = true;

        if (currentScreen == 2 && playerWon)
        {
            myField.ActivateInputField();
        }
    }

    void OnMouseDown()
    {
        if (waitingForClick)
        {
            if (currentScreen == 2 && playerWon)
            {
                SubmitScore();
            }
            else
            {
                waitingForClick = false;
                MoveToNextScreen();
            }
        }
    }

    public void MoveToNextScreen()
    {
        int maxScreens = playerWon ? 5 : 4;

        if (currentScreen < maxScreens)
        {
            currentScreen++;
            if (playerWon)
                screenController.MoveToWinningScreen(currentScreen);
            else
                screenController.MoveToLosingScreen(currentScreen);
            waitingForClick = true;

            if (currentScreen == 2 && playerWon)
            {
                myField.ActivateInputField();
            }
        }
    }

    public void MoveToPreviousScreen()
    {
        if (currentScreen > 0)
        {
            currentScreen--;
            if (playerWon)
                screenController.MoveToWinningScreen(currentScreen);
            else
                screenController.MoveToLosingScreen(currentScreen);
            waitingForClick = true;
        }
    }

    public void SubmitScore()
    {
        int parsedScore;
        if (int.TryParse(inputScore.text, out parsedScore))
        {
            leaderboard.SetLeaderboardEntry(myField.text, parsedScore);
        }
        else
        {
            myField.text = "Sporet";
            leaderboard.SetLeaderboardEntry(myField.text, parsedScore); 
        }
    }

    public void SetWin(bool value)
    {
        playerWon = value;
    }
}