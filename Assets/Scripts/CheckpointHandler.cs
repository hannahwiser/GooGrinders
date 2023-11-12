using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointHandler : MonoBehaviour
{
    public string[] FunnyTexts;
    public TextMeshProUGUI currentText, balanceText, promptText, confirmSpendText;
    public Animator anim;
    public GameObject deathCanvas;
    public int gorgerCost;

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

    public void DeathPopup()
    {
        Time.timeScale = 0;

        promptText.SetText("Would you like to spend " + gorgerCost + " googorger points to continue?");
        currentText.SetText(FunnyTexts[Random.Range(0, FunnyTexts.Length-1)]);
        balanceText.SetText("current balance: " + PlayerPrefs.GetInt("PlayerScore") + " points");

        if (PlayerPrefs.GetInt("PlayerScore") < gorgerCost)
        {
            confirmSpendText.SetText("Not Enough Points");
        }
        else
        {
            confirmSpendText.SetText("Spend Points");
        }

        deathCanvas.SetActive(true);
    }

}
