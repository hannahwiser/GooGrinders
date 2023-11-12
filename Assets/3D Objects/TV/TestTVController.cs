using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTVController : MonoBehaviour
{
    public tvScreenController screenController;
    public Button leftArrowButton;
    public Button rightArrowButton;

    private void Start()
    {
        if (screenController == null)
        {
            Debug.LogError("TV Screen Controller not assigned!");
            enabled = false;
        }

        leftArrowButton.onClick.AddListener(MoveToPreviousScreen);
        rightArrowButton.onClick.AddListener(MoveToNextScreen);
    }

    private void OnDestroy()
    {
        leftArrowButton.onClick.RemoveAllListeners();
        rightArrowButton.onClick.RemoveAllListeners();
    }

    private void MoveToPreviousScreen()
    {
        screenController.MoveToPreviousScreen();
    }

    private void MoveToNextScreen()
    {
        screenController.MoveToNextScreen();
    }
}