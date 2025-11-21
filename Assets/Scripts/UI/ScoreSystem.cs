using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;   
    [SerializeField] private TMP_Text highScoreText; 

    [Header("Настройки")]
    public int currentScore { get; private set; } = 0;
    public int highScore { get; private set; }

    private SaveData saveData;

    void Start()
    {
        saveData = SaveSystem.Load();
        highScore = saveData != null ? saveData.highScore : 0;

        currentScore = 0;

        UpdateUI();
    }

    public void AddScore(int points)
    {
        if (points == 0) return;

        currentScore += points;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            if (saveData == null) saveData = new SaveData();
            saveData.highScore = highScore;
            SaveSystem.Save(saveData);

            UpdateHighScoreUI();
        }

        UpdateScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    public void RefreshHighScoreFromDisk()
    {
        saveData = SaveSystem.Load();
        highScore = saveData != null ? saveData.highScore : 0;
        UpdateHighScoreUI();
    }

    private void UpdateUI()
    {
        UpdateScoreUI();
        UpdateHighScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = highScore.ToString();
    }
}
