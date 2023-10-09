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
        string temp = score.ToString();
        
        for (int i = 0; i < 4; i++)
        {
            if (temp.Length < 4 && i==0) 
            { 
                currentString[i] = '0';
                break;
            }
            currentString[i] = temp[i];
        }
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
