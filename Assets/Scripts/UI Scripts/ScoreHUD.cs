using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//this script manages the HUD in general
//it doesnt just handle score it also plays the jump charge
public class ScoreHUD : MonoBehaviour
{
    public Player playerScript;
    public TextMeshProUGUI[] numbers = new TextMeshProUGUI[4];
    public char[] currentString = new char[4];
    public Animator chargeAnim;
    public bool check;
    public int playerScore;

    // Start is called before the first frame update
    void Start()
    {
        
        PlayerPrefs.SetInt("PlayerScore", 0);
        //setting everything to 0 when game starts
        numbers[0].SetText("0");
        numbers[1].SetText("0");
        numbers[2].SetText("0");
        numbers[3].SetText("0");
        Debug.Log(PlayerPrefs.GetInt("PlayerScore"));
    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.Space) && chargeAnim.GetCurrentAnimatorStateInfo(0).IsName("HUDChargeJump"))
        {
            FinishCharge();
        }
    }

    //set the score by breaking score input down to char array
    public void SetScore(int score)
    {
        playerScore += score;
        string temp = "0000" + playerScore.ToString();

        currentString[3] = temp[temp.Length - 1];
        currentString[2] = temp[temp.Length - 2];
        currentString[1] = temp[temp.Length - 3];
        currentString[0] = temp[temp.Length - 4];

        StartCoroutine(blinkHUD());
    }

    public void SetSpend(int score)
    {
        playerScore = PlayerPrefs.GetInt("PlayerScore");
        string temp = "0000" + playerScore.ToString();

        currentString[3] = temp[temp.Length - 1];
        currentString[2] = temp[temp.Length - 2];
        currentString[1] = temp[temp.Length - 3];
        currentString[0] = temp[temp.Length - 4];

        StartCoroutine(blinkHUD());
    }

    IEnumerator blinkHUD()
    {
        numbers[0].SetText("");
        numbers[1].SetText("");
        numbers[2].SetText("");
        numbers[3].SetText("");
        yield return new WaitForSeconds(.1f);
        SetGUI(currentString);
    }

    public void SetGUI(char[] numToDisplay)
    {
        for (int i = 0; i < 4; i++)
        {
            numbers[i].SetText(numToDisplay[i].ToString());
        }
    }

    public void ChargeJump()
    {
        chargeAnim.Play("HUDChargeJump");
        StartCoroutine(AmIStillCharging());
    }

    public IEnumerator AmIStillCharging()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (!Input.GetKey(KeyCode.Space))
            {
                FinishCharge();
                break;
            }
        }
    }

    public void FinishCharge()
    {
        chargeAnim.Play("HUDRetractCharge");
        playerScript.StopCharge();
    }
}