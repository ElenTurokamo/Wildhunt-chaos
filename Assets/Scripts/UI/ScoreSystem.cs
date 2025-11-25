using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreSystem : MonoBehaviour
{
    // ... (Ваши существующие поля UI и Настройки)
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;   
    [SerializeField] private TMP_Text highScoreText; 
    [SerializeField] private TMP_Text runHighScoreText;

    [Header("Настройки")]
    public int currentScore { get; private set; } = 0;
    public int highScore { get; private set; }
    private int runHighScore = 0; 

    private SaveData saveData;

    [Header("Эффекты (Pulse)")]
    [SerializeField] private float pulseScale = 1.2f; 
    [SerializeField] private float pulseDuration = 1.0f;

    public enum FloatingTextPosition { Center, RightEdge, LeftEdge } // Новое перечисление

[Header("Эффекты (Floating Text)")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private FloatingTextPosition spawnPosition = FloatingTextPosition.RightEdge; 
    [SerializeField] private float verticalOffset = 50f; 
    [SerializeField] private float horizontalPadding = 20f; // <-- Новое поле для смещения в пикселях
    [SerializeField] private float floatDuration = 1.0f;
    [SerializeField] private float floatSpeed = 100f;

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
        
        StartPulse(scoreText.transform);
        if (currentScore >= runHighScore)
        {
             StartPulse(runHighScoreText.transform);
        }

        SpawnFloatingText(points);
    }

private void SpawnFloatingText(int points)
    {
        if (floatingTextPrefab == null || scoreText == null) return;

        GameObject instance = Instantiate(floatingTextPrefab, scoreText.transform.parent);
        TMP_Text tmp = instance.GetComponent<TMP_Text>();
        if (tmp == null)
        {
            Destroy(instance);
            return;
        }

        tmp.text = "+" + points;
        
        // --- Логика определения позиции ---
        Vector3 spawnPoint = scoreText.transform.position;
        float scoreTextWidth = scoreText.textBounds.size.x * scoreText.rectTransform.lossyScale.x;
        
        // Вычисляем ширину созданного всплывающего текста
        tmp.ForceMeshUpdate(); 
        float floatingTextWidth = tmp.textBounds.size.x * tmp.rectTransform.lossyScale.x;
        
        // Учет ширины текущего счетчика для смещения
        switch (spawnPosition)
        {
            case FloatingTextPosition.RightEdge:
                // Сдвиг к правому краю счета + половина ширины всплывающего текста + ручной отступ
                spawnPoint.x += (scoreTextWidth / 2f) + (floatingTextWidth / 2f) + horizontalPadding;
                break;
            case FloatingTextPosition.LeftEdge:
                // Сдвиг к левому краю счета - половина ширины всплывающего текста - ручной отступ
                spawnPoint.x -= (scoreTextWidth / 2f) + (floatingTextWidth / 2f) + horizontalPadding;
                break;
            case FloatingTextPosition.Center:
                // Добавляем только ручной отступ по X, если нужно
                spawnPoint.x += horizontalPadding; 
                break;
        }

        spawnPoint.y += verticalOffset;

        instance.transform.position = spawnPoint;
        // ----------------------------------

        StartCoroutine(AnimateFloatingText(tmp));
    }

    private IEnumerator AnimateFloatingText(TMP_Text target)
    {
        float timer = 0f;
        Vector3 startPos = target.transform.position;
        Color startColor = target.color;

        while (timer < floatDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / floatDuration;

            // Движение вверх
            target.transform.position = startPos + (Vector3.up * floatSpeed * timer);

            // Исчезновение (Fade Out)
            target.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, progress));

            yield return null;
        }

        Destroy(target.gameObject);
    }

    // --- Остальной код без изменений ---

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

    public int GetRunHighScore() => runHighScore;

    private void StartPulse(Transform targetTransform)
    {
        StopCoroutine(PulseScale(targetTransform)); 
        StartCoroutine(PulseScale(targetTransform));
    }

    private IEnumerator PulseScale(Transform targetTransform)
    {
        targetTransform.localScale = Vector3.one * pulseScale;

        float timer = 0f;
        Vector3 startScale = targetTransform.localScale;
        Vector3 endScale = Vector3.one;

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / pulseDuration; 
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null; 
        }
        targetTransform.localScale = endScale;
    }
}