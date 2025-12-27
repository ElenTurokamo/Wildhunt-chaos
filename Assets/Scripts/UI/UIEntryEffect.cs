using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIEntryEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float startOffsetY = 10f;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private bool replayOnEnable = false;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector2 _finalPosition;
    private Vector2 _startPosition;
    private bool _hasPlayed;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _finalPosition = _rectTransform.anchoredPosition;
        _startPosition = _finalPosition + new Vector2(0, startOffsetY);
    }

    private void OnEnable()
    {
        if (replayOnEnable || !_hasPlayed)
        {
            StartCoroutine(AnimateRoutine());
        }
        else
        {
            _canvasGroup.alpha = 1f;
            _rectTransform.anchoredPosition = _finalPosition;
        }
    }

    private IEnumerator AnimateRoutine()
    {
        _hasPlayed = true;
        
        _canvasGroup.alpha = 0f;
        _rectTransform.anchoredPosition = _startPosition;

        if (delay > 0)
            yield return new WaitForSecondsRealtime(delay);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float percentage = Mathf.Clamp01(elapsedTime / duration);
            float curve = Mathf.SmoothStep(0f, 1f, percentage);

            _canvasGroup.alpha = curve;
            _rectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _finalPosition, curve);

            yield return null;
        }

        _canvasGroup.alpha = 1f;
        _rectTransform.anchoredPosition = _finalPosition;
    }
}