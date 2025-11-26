using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class GameUIEntryEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float startOffsetX = 50f; // Расстояние смещения по горизонтали
    
    [Tooltip("Если включено, элемент вылетает СПРАВА. Если выключено, СЛЕВА.")]
    [SerializeField] private bool comeFromRight = false; // Логика для fromLeft/fromRight
    
    [SerializeField] private float duration = 1.0f;    // Длительность анимации
    [SerializeField] private float delay = 0f;         // Задержка перед стартом

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _finalPosition;
    private Vector2 _startPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _finalPosition = _rectTransform.anchoredPosition;

        float offset = comeFromRight ? startOffsetX : -startOffsetX;
        
        _startPosition = _finalPosition + new Vector2(offset, 0);

        _canvasGroup.alpha = 0f;
        _rectTransform.anchoredPosition = _startPosition;
    }

    private IEnumerator Start()
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = Mathf.Clamp01(elapsedTime / duration);
            
            // Используем SmoothStep для более плавного движения
            float curve = Mathf.SmoothStep(0f, 1f, percentage);

            _canvasGroup.alpha = curve;
            _rectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _finalPosition, curve);

            yield return null;
        }

        // Убеждаемся, что конечные значения установлены точно
        _canvasGroup.alpha = 1f;
        _rectTransform.anchoredPosition = _finalPosition;
    }
}