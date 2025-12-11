using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;   
    [SerializeField] private TMP_Text highScoreText; 
    [SerializeField] private TMP_Text runHighScoreText;

    [Header("Настройки")]
    public int currentScore { get; private set; } = 0;
    public int highScore { get; private set; }
    private int runHighScore = 0; 

    private SaveData saveData;

    private Color defaultScoreColor;

    [Header("Эффекты (Pulse)")]
    [SerializeField] private float pulseScale = 1.2f; 
    [SerializeField] private float pulseDuration = 1.0f;
    [SerializeField] private Color pulseColor = Color.white;

    public enum FloatingTextPosition { Center, RightEdge, LeftEdge } 

[Header("Эффекты (Floating Text)")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private FloatingTextPosition spawnPosition = FloatingTextPosition.RightEdge; 
    [SerializeField] private float verticalOffset = 50f; 
    [SerializeField] private float horizontalPadding = 20f; 
    [SerializeField] private float floatDuration = 1.0f;
    [SerializeField] private float floatSpeed = 100f;

    void Start()
    {
        if (scoreText != null)
        {
            defaultScoreColor = scoreText.color;
        }
        saveData = SaveSystem.Load();
        highScore = saveData != null ? saveData.highScore : 0;

        ResetScore();
        UpdateUI();
    }

    public void AddScore(int points, Vector3 enemyPosition = default)
    {
        if (points == 0) return;

        currentScore += points;

        if (currentScore > runHighScore) runHighScore = currentScore;

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
        
        StartPulse(scoreText); 
        if (currentScore >= runHighScore) StartPulse(runHighScoreText);

        SpawnFloatingText(points, enemyPosition);
    }

    private void SpawnFloatingText(int points, Vector3 worldPos)
    {
        if (floatingTextPrefab == null || scoreText == null) return;

        GameObject instance = Instantiate(floatingTextPrefab, scoreText.transform.parent);
        TMP_Text tmp = instance.GetComponent<TMP_Text>();
        if (tmp == null) { Destroy(instance); return; }

        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        if (rectTransform == null) { Destroy(instance); return; }
        
        tmp.text = "+" + points;
        
        rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, 0);
        rectTransform.localScale = Vector3.one;

        if (worldPos != default)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            Camera uiCamera = scoreText.canvas.worldCamera;
            
            if (uiCamera == null) uiCamera = Camera.main;

            RectTransform parentRect = scoreText.transform.parent as RectTransform;
            Vector2 localPoint;
            
            bool isConverted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect, 
                screenPos, 
                uiCamera, 
                out localPoint
            );
            if (isConverted)
            {
                localPoint.x += horizontalPadding; 
                localPoint.y += verticalOffset;
                rectTransform.localPosition = localPoint;
            }
        }
        else
        {
            Vector3 spawnPoint = scoreText.transform.localPosition;
            spawnPoint = scoreText.transform.position;
            float scoreTextWidth = scoreText.textBounds.size.x * scoreText.rectTransform.lossyScale.x;
            tmp.ForceMeshUpdate(); 
            float floatingTextWidth = tmp.textBounds.size.x * tmp.rectTransform.lossyScale.x;
            
            switch (spawnPosition)
            {
                case FloatingTextPosition.RightEdge:
                    spawnPoint.x += (scoreTextWidth / 2f) + (floatingTextWidth / 2f) + horizontalPadding;
                    break;
                case FloatingTextPosition.LeftEdge:
                    spawnPoint.x -= (scoreTextWidth / 2f) + (floatingTextWidth / 2f) + horizontalPadding;
                    break;
                case FloatingTextPosition.Center:
                    spawnPoint.x += horizontalPadding; 
                    break;
            }
        }

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

            target.transform.position = startPos + (Vector3.up * floatSpeed * timer);

            target.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, progress));

            yield return null;
        }

        Destroy(target.gameObject);
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

    public int GetRunHighScore() => runHighScore;

    private void StartPulse(TMP_Text targetText)
    {
        StopCoroutine(PulseEffect(targetText)); 
        StartCoroutine(PulseEffect(targetText));
    }

    private IEnumerator PulseEffect(TMP_Text targetText)
    {
        if (targetText == null) yield break;

        Color startColor = targetText == scoreText ? defaultScoreColor : targetText.color;
        
        targetText.transform.localScale = Vector3.one * pulseScale;
        targetText.color = pulseColor;

        float timer = 0f;
        Vector3 startScale = targetText.transform.localScale;
        Vector3 endScale = Vector3.one;

        while (timer < pulseDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / pulseDuration; 
            
            targetText.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            
            targetText.color = Color.Lerp(pulseColor, startColor, progress);
            
            yield return null; 
        }
        
        targetText.transform.localScale = endScale;
        targetText.color = startColor;
    }

    // private IEnumerator PulseScale(Transform targetTransform)
    // {
    //     targetTransform.localScale = Vector3.one * pulseScale;

    //     float timer = 0f;
    //     Vector3 startScale = targetTransform.localScale;
    //     Vector3 endScale = Vector3.one;

    //     while (timer < pulseDuration)
    //     {
    //         timer += Time.deltaTime;
    //         float progress = timer / pulseDuration; 
    //         targetTransform.localScale = Vector3.Lerp(startScale, endScale, progress);
    //         yield return null; 
    //     }
    //     targetTransform.localScale = endScale;
    // }
}