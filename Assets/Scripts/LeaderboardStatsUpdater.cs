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
        totalTimeGrindedText.text = ConvertToTimeFormat(totalTimeGrinded);
        totalAttemptsText.text = totalAttempts.ToString();
    }

    private string ConvertToTimeFormat(float totalTime)
    {
        int hours = (int)(totalTime / 3600);
        int minutes = (int)((totalTime % 3600) / 60);
        int seconds = (int)(totalTime % 60);

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
