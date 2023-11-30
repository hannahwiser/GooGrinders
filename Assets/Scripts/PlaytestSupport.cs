using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaytestSupport : MonoBehaviour
{
    public PlayerLife playerScript;
    public ScoreHUD scoreHUD;

    public void AddMoney()
    {
        scoreHUD.AddDebugPoints();
    }

    public void RespawnAt(Transform respawnPoint)
    {
        playerScript.Respawn(respawnPoint);
    }
}
