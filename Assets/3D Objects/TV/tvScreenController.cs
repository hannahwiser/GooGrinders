using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tvScreenController : MonoBehaviour
{
    public Transform[] winningScreens;
    public Transform[] losingScreens;
    public Camera tvRecorderCamera;

    public float movementSpeed = 5.0f;
    public float rotationSpeed = 2.0f;

    private int currentWinningScreenIndex = 0;
    private int currentLosingScreenIndex = 0;

    public void MoveToNextScreen()
    {
        if (currentWinningScreenIndex < winningScreens.Length - 1)
        {
            currentWinningScreenIndex++;
            MoveToWinningScreen(currentWinningScreenIndex);
        }
        else if (currentLosingScreenIndex < losingScreens.Length - 1)
        {
            currentLosingScreenIndex++;
            MoveToLosingScreen(currentLosingScreenIndex);
        }
    }

    public void MoveToPreviousScreen()
    {
        if (currentWinningScreenIndex > 0)
        {
            currentWinningScreenIndex--;
            MoveToWinningScreen(currentWinningScreenIndex);
        }
        else if (currentLosingScreenIndex > 0)
        {
            currentLosingScreenIndex--;
            MoveToLosingScreen(currentLosingScreenIndex);
        }
    }

    private void Start()
    {
        MoveToScreen(winningScreens[0]);
    }

    public void MoveToWinningScreen(int screenIndex)
    {
        if (screenIndex >= 0 && screenIndex < winningScreens.Length)
        {
            MoveToScreen(winningScreens[screenIndex]);
        }
        else
        {
            Debug.LogError("Invalid winning screen index");
        }
    }

    public void MoveToLosingScreen(int screenIndex)
    {
        if (screenIndex >= 0 && screenIndex < losingScreens.Length)
        {
            MoveToScreen(losingScreens[screenIndex]);
        }
        else
        {
            Debug.LogError("Invalid losing screen index");
        }
    }

    private void MoveToScreen(Transform targetScreen)
    {
        StartCoroutine(MoveToScreenCoroutine(targetScreen));
    }

    private IEnumerator MoveToScreenCoroutine(Transform targetScreen)
    {
        while (Vector3.Distance(tvRecorderCamera.transform.position, targetScreen.position) > 0.01f)
        {
            tvRecorderCamera.transform.position = Vector3.Lerp(tvRecorderCamera.transform.position, targetScreen.position, Time.deltaTime * movementSpeed);
            yield return null;
        }
    }
}