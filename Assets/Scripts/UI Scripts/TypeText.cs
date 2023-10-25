using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeText : MonoBehaviour
{
    public float timeBetweenLetters;
    public TextMeshProUGUI thisText;
    public string targetText;
    public string currentText;

    IEnumerator TypeThisText()
    {
        Debug.Log("Method Start");
        targetText = thisText.text;
        Debug.Log(targetText);
        thisText.SetText("");

        foreach (char c in targetText)
        {
            currentText += c;
            thisText.SetText(currentText);
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }
}
