using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardStatsUpdater : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI googorgersPoppedText;
    public TextMeshProUGUI totalTimeGrindedText;
    public TextMeshProUGUI totalAttemptsText;

    // Method to update leaderboard stats with the final stats when the player dies for good
    public void UpdateLeaderboardStats(int finalScore, int googorgersPopped, float totalTimeGrinded, int totalAttempts)
    {
        finalScoreText.text = finalScore.ToString();
        googorgersPoppedText.text = googorgersPopped.ToString();
        totalTimeGrindedText.text = totalTimeGrinded.ToString("F2") + "s";
        totalAttemptsText.text = totalAttempts.ToString();
    }
}
