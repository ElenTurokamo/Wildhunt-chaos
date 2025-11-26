using UnityEngine;
using System.Collections;

public class CharacterEntryEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 finalPosition = new Vector3(0f, -3.05f, 0f); 
    [SerializeField] private float duration = 2.0f;    
    [SerializeField] private float delay = 0.5f;       

    private Vector3 _startPosition;
    private PlayerController _playerController; // Ссылка на контроллер

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        if (_playerController != null)
        {
            // Отключаем управление и ограничение сразу при старте сцены
            _playerController.isControlsActive = false; 
        }

        _startPosition = transform.position;
    }

    private IEnumerator Start()
    {
        if (delay > 0)
            yield return new WaitForSecondsRealtime(delay); 

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ... (Код анимации остается прежним) ...
            
            elapsedTime += Time.unscaledDeltaTime; 
            float percentage = Mathf.Clamp01(elapsedTime / duration);
            float curve = Mathf.SmoothStep(0f, 1f, percentage);

            transform.position = Vector3.Lerp(_startPosition, finalPosition, curve);

            yield return null;
        }

        transform.position = finalPosition;
        
        // !!! ГЛАВНОЕ ИЗМЕНЕНИЕ: Включаем управление и ограничение !!!
        if (_playerController != null)
        {
            _playerController.isControlsActive = true;
        }
        
        // Если вы остановили время в LoadingManager, вам нужно его включить здесь:
        Time.timeScale = 1f;
    }
}