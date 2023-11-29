using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LeaderboardSequence : MonoBehaviour
{
    public tvScreenController screenController;
    public BoxCollider TV;
    public Leaderboard leaderboard;

    public TMP_InputField myField;
    public TextMeshProUGUI inputScore;

    // set playerWon to true if the player won the game
    public bool playerWon = false;

    private int currentScreen = 0;
    private bool waitingForClick = false;

    private bool scoreSubmitted = false;

    public GameObject clickText;

    public bool nameEntered;

    private void Update()
    {
        if ((!playerWon && currentScreen == 4) || (playerWon && currentScreen == 5))
        {
            if (Input.GetKey(KeyCode.R))
            {
                SceneManager.LoadScene(1); // Restart level
            }
            if (Input.GetKey(KeyCode.Q))
            {
                SceneManager.LoadScene(0); // Quit to menu
            }
        }
        if (currentScreen == 2 && Input.GetKeyDown(KeyCode.Return))
        {
            nameEntered = true;
        }
    }

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
        StartCoroutine(PlaySequence());
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

    /*void OnMouseDown()
    {
        if (waitingForClick)
        {
            if (currentScreen == 2 && playerWon)
            {
                if (!scoreSubmitted)
                {
                    SubmitScore();
                    // set the scoreSubmitted to true after submitting the score so the user can't spam the leaderboard
                    scoreSubmitted = true;
                    // move to next screen after a delay
                    StartCoroutine(MoveToNextScreenWithDelay(0.5f)); 
                }
            }
            else
            {
                waitingForClick = false;
                MoveToNextScreen();
                //StartCoroutine(MoveToNextScreenWithDelay(0.05f)); // move to next screen after a delay
            }
        }
    }*/

    public IEnumerator PlaySequence() //setting this up to play the end of game sequence
    {
        yield return new WaitForSeconds(2f); 
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.2f);
        MoveToNextScreen(); //moves to player info
        yield return new WaitForSeconds(1.8f);
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.2f);
        MoveToNextScreen(); //moves to name input
        myField.ActivateInputField(); //activate the input text for the name
        bool isEnterPressed = false;

        while (!nameEntered)//wait for player to press enter
        {
            yield return new WaitForEndOfFrame();
        }

        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.2f);
        MoveToNextScreen(); //moves to score screen 1
        yield return new WaitForSeconds(1.3f);
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.2f);
        MoveToNextScreen(); //moves to score screen 2
        yield return new WaitForSeconds(1.3f);
        this.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(.2f);
        MoveToNextScreen(); //moves to restart screen
    }

    void OnMouseDown()
    {
        myField.ActivateInputField();
        if (currentScreen == 2 && myField.text.Length > 0) //make sure that its not someone clicking to input their name
        {
            nameEntered = true;
        }
    }


    public void MoveToNextScreen()
    {
        //int maxScreens = playerWon ? 5 : 4;
        //if (currentScreen < maxScreens)
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

    IEnumerator MoveToNextScreenWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MoveToNextScreen();
    }
}