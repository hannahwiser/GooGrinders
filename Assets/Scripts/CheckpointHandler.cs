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
    public AudioSource spendPointsAudio;
    public ScoreHUD scoreHUDscript;
    public Button spendPointsButton;
    public GameObject cam1, cam2;
    private void Awake()
    {
        deathCanvas.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
        currentText.SetText(FunnyTexts[Random.Range(0, FunnyTexts.Length-1)]);
        balanceText.SetText("current balance: " + PlayerPrefs.GetInt("PlayerScore") + " points");

        //making sure the player cant even press the button if they don't have enough $$
        if (PlayerPrefs.GetInt("PlayerScore") <= 0)
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
        Time.timeScale = 1;
        deathCanvas.GetComponent<Animator>().Play("DeathPopIn");
        int temp = PlayerPrefs.GetInt("PlayerScore");
        PlayerPrefs.SetInt("PlayerScore", temp - gorgerCost); //subtracta da pointsa
        scoreHUDscript.SetSpend(PlayerPrefs.GetInt("PlayerScore"));
        spendPointsAudio.Play(); //play the audio
        playerScript.Respawn(lastCheckpoint); //do the respawn legwork from another script i didnt write <3
    }

    public void LetMeDie()
    {
        HUDCanvas.SetActive(false);
        Time.timeScale = 1;
        cam1.SetActive(false);
        cam2.SetActive(true);
        deathCanvas.GetComponent<Animator>().Play("DeathPopIn");
    }

}