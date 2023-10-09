using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI[] numbers = new TextMeshProUGUI[4];
    public char[] currentString = new char[4];

    // Start is called before the first frame update
    void Start()
    {
        //setting everything to 0 when game starts
        numbers[0].SetText("0");
        numbers[1].SetText("0"); 
        numbers[2].SetText("0");
        numbers[3].SetText("0");
    }

    //set the score by breaking score input down to char array
    public void SetScore(int score)
    {
        string temp = "0" + score.ToString();

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
}