using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSequence : MonoBehaviour
{
    public tvScreenController screenController;

    // Set this to true if the player won the game
    public bool playerWon = false;

    private int currentScreen = 1;

    public void StartLeaderboardSequence()
    {
        // set the initial screen based on playerWon
        if (playerWon)
            screenController.MoveToWinningScreen(1);
        else
            screenController.MoveToLosingScreen(1);

        currentScreen = 1;
    }

    public void MoveToNextScreen()
    {
        if (currentScreen < 5)
        {
            currentScreen++;
            if (playerWon)
                screenController.MoveToWinningScreen(currentScreen);
            else
                screenController.MoveToLosingScreen(currentScreen);
        }
    }

    public void MoveToPreviousScreen()
    {
        if (currentScreen > 1)
        {
            currentScreen--;
            if (playerWon)
                screenController.MoveToWinningScreen(currentScreen);
            else
                screenController.MoveToLosingScreen(currentScreen);
        }
    }

    internal void setWin(bool v)
    {
        playerWon = v;
    }
}