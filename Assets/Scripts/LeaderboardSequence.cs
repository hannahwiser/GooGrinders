using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardSequence : MonoBehaviour
{
    public tvScreenController screenController;

    public void StartLeaderboardSequence()
    {
        // Set the TV screen to screen 1
        screenController.MoveToLosingScreen(1);

        // Start a coroutine to wait for user input before moving to screen 2
        StartCoroutine(WaitForButtonClick());
    }

    IEnumerator WaitForButtonClick()
    {
        // wait until the button is clicked
        while (!ButtonClicked())
        {
            yield return null;
        }

        // move to screen 2
        screenController.MoveToLosingScreen(2);
    }

    bool ButtonClicked()
    {

        return Input.GetMouseButtonDown(0); 
    }
}
