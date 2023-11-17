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
        float distanceThreshold = 0.01f;
        while (Vector3.Distance(tvRecorderCamera.transform.position, targetScreen.position) > distanceThreshold)
        {
            float step = movementSpeed * Time.deltaTime;

            //tvRecorderCamera.transform.position = Vector3.Lerp(tvRecorderCamera.transform.position, targetScreen.position, Time.deltaTime * movementSpeed);

            // move the camera towards the target position
            tvRecorderCamera.transform.position = Vector3.MoveTowards(tvRecorderCamera.transform.position, targetScreen.position, step);

            yield return null;
        }
        tvRecorderCamera.transform.position = targetScreen.position;
    }
}