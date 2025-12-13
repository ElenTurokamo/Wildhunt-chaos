using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI Ссылки")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject pauseMenu;

    [Header("Настройки жизней")]
    [SerializeField] private int _maxLives = 3;
    public int maxLives => _maxLives;
    
    public int currentLives { get; private set; }

    [Header("Неуязвимость")]
    [Tooltip("Продолжительность неуязвимости (сек)")]
    [SerializeField] private float invincibilityDuration = 2.0f;
    [Tooltip("Частота мигания")]
    [SerializeField] private float flashInterval = 0.1f;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    public static event Action<int> OnLivesChanged;

    void Start()
    {
        currentLives = _maxLives;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        OnLivesChanged?.Invoke(currentLives);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible) return;

        bool tookDamage = false;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && bullet.isEnemy)
        {
            if (!bullet.isLaser)
            {
                Destroy(bullet.gameObject);
            }
            tookDamage = true;
        }

        EnemyDestructable destructable = collision.GetComponent<EnemyDestructable>();
        if (destructable != null)
        {
            tookDamage = true;
        }

        if (tookDamage)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        currentLives -= damageAmount;

        OnLivesChanged?.Invoke(currentLives);

        if (currentLives <= 0)
        {
            PlayerDie();
        }
        else
        {
            StartCoroutine(InvulnerabilityFrames());
        }
    }

    private IEnumerator InvulnerabilityFrames()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            float timer = 0f;
            while (timer < invincibilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(flashInterval);
                timer += flashInterval;
            }
            spriteRenderer.enabled = true;
        }

        isInvincible = false;
    }

    private void PlayerDie()
    {
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pauseMenu != null) 
        {
            pauseMenu.SetActive(false);
            // Destroy(pauseMenu); // Лучше просто скрыть, чем удалять
        }
        if (gameOverUI != null) gameOverUI.SetActive(true);

        Time.timeScale = 0f;
        Destroy(gameObject);
    }
}