using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTVController : MonoBehaviour
{
    public tvScreenController screenController;

    private void Start()
    {
        if (screenController == null)
        {
            Debug.LogError("TV Screen Controller not assigned!");
            enabled = false;
        }

        StartCoroutine(CycleScreens());
    }

    private IEnumerator CycleScreens()
    {
        while (true)
        {
            for (int i = 0; i < screenController.winningScreens.Length; i++)
            {
                screenController.MoveToWinningScreen(i);
                yield return new WaitForSeconds(3.0f);
            }

            for (int i = 0; i < screenController.losingScreens.Length; i++)
            {
                screenController.MoveToLosingScreen(i);
                yield return new WaitForSeconds(3.0f);
            }
        }
    }
}