using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;   
    [SerializeField] private TMP_Text highScoreText; 
    [SerializeField] private TMP_Text runHighScoreText; // <-- новый элемент

    [Header("Настройки")]
    public int currentScore { get; private set; } = 0;
    public int highScore { get; private set; }
    private int runHighScore = 0; // максимальный счёт за текущий забег

    private SaveData saveData;

    void Start()
    {
        saveData = SaveSystem.Load();
        highScore = saveData != null ? saveData.highScore : 0;

        ResetScore();
        UpdateUI();
    }

    public void AddScore(int points)
    {
        if (points == 0) return;

        currentScore += points;

        // обновляем максимум за текущий забег
        if (currentScore > runHighScore)
            runHighScore = currentScore;

        if (currentScore > highScore)
        {
            highScore = currentScore;
            if (saveData == null) saveData = new SaveData();
            saveData.highScore = highScore;
            SaveSystem.Save(saveData);
            UpdateHighScoreUI();
        }

        UpdateScoreUI();
        UpdateRunHighScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        runHighScore = 0;
        UpdateScoreUI();
        UpdateRunHighScoreUI();
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
        UpdateRunHighScoreUI();
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

    private void UpdateRunHighScoreUI()
    {
        if (runHighScoreText != null)
            runHighScoreText.text = runHighScore.ToString();
    }

    // Получить максимум забега для GameOver
    public int GetRunHighScore() => runHighScore;
}
