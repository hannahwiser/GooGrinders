using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToLeaderboard : MonoBehaviour
{
    public GameObject gameobject;
    public GameObject lbCamera;
    // Start is called before the first frame update
    void Start()
    {
        lbCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GoToLeaderboard();
        }
    }

    public void GoToLeaderboard()
    {
        gameobject.SetActive(false);
        lbCamera.SetActive(true);
    }
}
