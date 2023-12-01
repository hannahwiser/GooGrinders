/**
 * File: Winzone
 * Programmer: Hannah Wiser
 * Description: This script lets the player win
 * Date: Nov XX, 2023
 */

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
            Destroy(sporet);
        }
    }
}
