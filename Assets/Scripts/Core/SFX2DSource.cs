using UnityEngine;
using UnityEngine.EventSystems;

public class SFX2DSource : MonoBehaviour, IPointerClickHandler
{
    public enum PlayCondition
    {
        Manual,            
        OnStart,           
        OnEnable,          
        OnDisable,          
        OnDestroy,    
        OnTriggerEnter,   
        OnCollisionEnter,   
        OnUIClick          
    }

    [HideInInspector] 
    public string soundName;

    [Header("Условия запуска")]
    public PlayCondition condition = PlayCondition.Manual;

    [Header("Фильтры (для физики)")]
    [Tooltip("Если пусто - реагирует на всё. Если задан тег - только на него.")]
    public string targetTag = "";

    [Header("Настройки")]
    [Tooltip("Уничтожить этот объект после запуска звука?")]
    public bool destroyAfterPlay = false;
    public float destroyDelay = 0f;

    private bool hasPlayed = false;


    void Start()
    {
        if (condition == PlayCondition.OnStart) Play();
    }

    void OnEnable()
    {
        hasPlayed = false; 
        if (condition == PlayCondition.OnEnable) Play();
    }

    void OnDisable()
    {
        if (condition == PlayCondition.OnDisable) Play();
    }

    void OnDestroy()
    {
        if (condition == PlayCondition.OnDestroy) Play();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (condition == PlayCondition.OnTriggerEnter)
        {
            if (string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag))
                Play();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (condition == PlayCondition.OnCollisionEnter)
        {
            if (string.IsNullOrEmpty(targetTag) || other.gameObject.CompareTag(targetTag))
                Play();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (condition == PlayCondition.OnUIClick) Play();
    }

    public void Play()
    {
        if (string.IsNullOrEmpty(soundName)) return;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFXOneShot(soundName);
        }
        else
        {
            Debug.LogWarning("AudioManager не найден на сцене!");
        }

        HandleDestruction();
    }

    private void HandleDestruction()
    {
        if (destroyAfterPlay && !hasPlayed)
        {
            hasPlayed = true;
            Destroy(gameObject, destroyDelay);
        }
    }
}