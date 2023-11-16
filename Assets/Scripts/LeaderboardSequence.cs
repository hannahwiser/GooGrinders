using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSequence : MonoBehaviour
{
    public tvScreenController screenController;

    private int currentScreen = 1;

    public void StartLeaderboardSequence()
    {
        // Set the TV screen to screen 1
        screenController.MoveToLosingScreen(1);
        currentScreen = 1;
    }

    public void MoveToNextScreen()
    {
        if (currentScreen < 5)
        {
            currentScreen++;
            screenController.MoveToLosingScreen(currentScreen);
        }
    }

    public void MoveToPreviousScreen()
    {
        if (currentScreen > 1)
        {
            currentScreen--;
            screenController.MoveToLosingScreen(currentScreen);
        }
    }
}