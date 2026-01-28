using UnityEngine;

public static class SaveSystem
{
    private const string HIGH_SCORE_KEY = "HighScore";

    public static void SaveScore(int score)
    {
        int currentHighScore = GetHighScore();
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
        }
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
    
    public static void ClearProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
