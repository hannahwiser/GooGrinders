using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CheckpointHandler : MonoBehaviour
{
    public string[] FunnyTexts;
    public TextMeshProUGUI currentText, balanceText, promptText, confirmSpendText;
    public Animator anim;
    public GameObject deathCanvas;
    public GameObject HUDCanvas;
    public int gorgerCost;
    public Transform lastCheckpoint;
    public PlayerLife playerScript;
    public AudioSource spendPointsAudio, confirmDeathAudio;
    public ScoreHUD scoreHUDscript;
    public Button spendPointsButton;
    public GameObject cam1, cam2;

    public LeaderboardStatsUpdater leaderboardUpdater;
    public ItemCollector itemCollector;
    public Player player;
    int totalAttempts = 1;
    public LeaderboardSequence leaderboardSequence;
    public TextMeshProUGUI explorersNote;
    public int arrayPoint;

    private void Awake()
    {
        deathCanvas.SetActive(false);
    }

    public void SetCheckPoint(Transform theLocation)
    {
        lastCheckpoint = theLocation;
    }

    public void DeathPopup()
    {
        Time.timeScale = 0;

        gorgerCost = 100;

        promptText.SetText("Would you like to spend " + gorgerCost + " googorger points to respawn at the most recent checkpoint?");
        arrayPoint = Random.Range(0, FunnyTexts.Length - 1);
        currentText.SetText(FunnyTexts[arrayPoint]);
        balanceText.SetText("current balance: " + PlayerPrefs.GetInt("PlayerScore") + " points");

        //making sure the player cant even press the button if they don't have enough $$
        if (PlayerPrefs.GetInt("PlayerScore") < 100)
        {
            confirmSpendText.SetText("Outta Points");
            spendPointsButton.enabled = false;
        }
        else
        {
            spendPointsButton.enabled = true;
            confirmSpendText.SetText("Spend Points");
        }

        deathCanvas.SetActive(true);
    }

    public void SpendPoints()
    {
        totalAttempts++;
        Time.timeScale = 1;
        deathCanvas.GetComponent<Animator>().Play("DeathPopIn");
        int temp = PlayerPrefs.GetInt("PlayerScore");
        PlayerPrefs.SetInt("PlayerScore", temp - gorgerCost); //subtracta da pointsa
        scoreHUDscript.SetSpend(PlayerPrefs.GetInt("PlayerScore"));
        spendPointsAudio.Play(); //play the audio
        playerScript.Respawn(lastCheckpoint); //do the respawn legwork from another script i didnt write <3
        //StartCoroutine(WaitToRespawn());
    }

    public IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(.5f);
        playerScript.Respawn(lastCheckpoint); //do the respawn legwork from another script i didnt write <3
    }

    public void LetMeWin()
    {
        explorersNote.SetText("Wow. We're surprised to see you here.");
        int googorgersPopped = 0;
        float totalTimeGrinded = 0;

        if (itemCollector != null)
        {
            googorgersPopped = itemCollector.googorgersPopped;
        }
        if (playerScript != null)
        {
            totalTimeGrinded = player.totalTimeOnRail;
        }
        leaderboardUpdater.UpdateLeaderboardStats(PlayerPrefs.GetInt("PlayerScore"), googorgersPopped, totalTimeGrinded, totalAttempts);
        cam1.SetActive(false);
        cam2.SetActive(true);

        leaderboardSequence.StartLeaderboardSequence();
    }

    public void LetMeDie()
    {
        explorersNote.SetText(FunnyTexts[arrayPoint]);
        confirmDeathAudio.Play();
        // get the stats for the leaderboard 
        int googorgersPopped = 0;
        float totalTimeGrinded = 0;

        if (itemCollector != null)
        {
            googorgersPopped = itemCollector.googorgersPopped;
        }
        if (playerScript != null)
        {
            totalTimeGrinded = player.totalTimeOnRail;
        }

        // update the leaderboard stats
        leaderboardUpdater.UpdateLeaderboardStats(PlayerPrefs.GetInt("PlayerScore"), googorgersPopped, totalTimeGrinded, totalAttempts);

        HUDCanvas.SetActive(false);
        Time.timeScale = 1;
        cam1.SetActive(false);
        cam2.SetActive(true);
        deathCanvas.GetComponent<Animator>().Play("DeathPopIn");

        leaderboardSequence.StartLeaderboardSequence();
    }

}
