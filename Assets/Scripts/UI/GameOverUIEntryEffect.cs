using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class GameOverUIEntryEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject parentTarget; // Ссылка на главный объект GameOverUI
    [SerializeField] private float startOffsetY = 20f; 
    [SerializeField] private float duration = 1.0f; 
    [SerializeField] private float delay = 0f; 

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _finalPosition;
    private Vector2 _startPosition;
    private Coroutine _animationCoroutine;
    private bool _hasAnimated = false; // Флаг для однократного запуска

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _finalPosition = _rectTransform.anchoredPosition;
        _startPosition = _finalPosition + new Vector2(0, startOffsetY);

        // Устанавливаем начальное состояние.
        _canvasGroup.alpha = 0f;
        _rectTransform.anchoredPosition = _startPosition;
    }

    private void Update()
    {
        // Проверяем, активен ли целевой родительский элемент
        if (parentTarget != null && parentTarget.activeInHierarchy)
        {
            // Если он активен, и анимация еще не проигрывалась, запускаем
            if (!_hasAnimated)
            {
                // Сбрасываем позицию и альфу на случай, если они были изменены
                _canvasGroup.alpha = 0f;
                _rectTransform.anchoredPosition = _startPosition;
                
                // Останавливаем старую корутину, если была
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }
                
                _animationCoroutine = StartCoroutine(AnimateEntry());
                _hasAnimated = true; // Устанавливаем флаг, чтобы не запускать снова
            }
        }
        else
        {
            // Если родитель не активен, сбрасываем флаг, 
            // чтобы анимация могла запуститься при следующей активации
            if (_hasAnimated)
            {
                _hasAnimated = false;
            }
        }
    }

    private IEnumerator AnimateEntry()
    {
        if (delay > 0)
            yield return new WaitForSecondsRealtime(delay); // Используем Realtime для экрана паузы/смерти

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Используем Time.unscaledDeltaTime, так как Time.timeScale = 0
            elapsedTime += Time.unscaledDeltaTime; 
            float percentage = Mathf.Clamp01(elapsedTime / duration);
            
            float curve = Mathf.SmoothStep(0f, 1f, percentage);

            _canvasGroup.alpha = curve;
            _rectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _finalPosition, curve);

            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _rectTransform.anchoredPosition = _finalPosition;
        
        _animationCoroutine = null;
    }
}