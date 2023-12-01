using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    public CheckpointHandler chkptScript;
    public LeaderboardSequence leaderboardSequence;
    public GameObject sporet;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //leaderboardSequence.SetWin(true);
            chkptScript.LetMeWin();
            sporet.SetActive(false);
        }
    }
}
