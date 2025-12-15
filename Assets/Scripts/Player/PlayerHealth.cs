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
    [Tooltip("Продолжительность действия щита (сек)")]
    [SerializeField] private float invincibilityDuration = 2.0f;
    
    [Header("Щит")]
    [Tooltip("Объект щита (дочерний), который будет включаться")]
    [SerializeField] private GameObject shieldObject;

    private bool isInvincible = false;

    public static event Action<int> OnLivesChanged;

    void Start()
    {
        currentLives = _maxLives;
        
        if (shieldObject != null) 
            shieldObject.SetActive(false);

        OnLivesChanged?.Invoke(currentLives);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible) return;

        bool tookDamage = false;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && bullet.isEnemy)
        {
            if (!bullet.isLaser) Destroy(bullet.gameObject);
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
            StartCoroutine(ActivateShieldRoutine());
        }
    }

    private IEnumerator ActivateShieldRoutine()
    {
        isInvincible = true;

        if (shieldObject != null)
            shieldObject.SetActive(true);

        yield return new WaitForSeconds(invincibilityDuration);

        if (shieldObject != null)
            shieldObject.SetActive(false);

        isInvincible = false;
    }

    private void PlayerDie()
    {
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (gameOverUI != null) gameOverUI.SetActive(true);

        Time.timeScale = 0f;
        Destroy(gameObject);
    }
}