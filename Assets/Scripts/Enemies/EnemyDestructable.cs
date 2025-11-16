using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemyDestructable : MonoBehaviour
{
    [Header("Настройки очков")]
    public int baseScore = 10;
    [HideInInspector] public int threatCost = 1;

    [Header("Настройки здоровья")]
    public int health = 3; // Количество попаданий, чтобы уничтожить врага

    [Header("Эффект попадания")]
    public float flashDuration = 0.1f; // Длительность мигания

    private bool canBeDestroyed = false;
    private ScoreSystem scoreSystem;

    private SpriteRenderer[] childRenderers;

    void Awake()
    {
        scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
        // Берём все SpriteRenderer дочерних объектов
        childRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (transform.position.y < 5.0f && !canBeDestroyed)
        {
            canBeDestroyed = true;
            Gun[] guns = transform.GetComponentsInChildren<Gun>();
            foreach (Gun gun in guns)
            {
                gun.isActive = true;
            }
        }

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeDestroyed) return;

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && !bullet.isEnemy)
        {
            TakeDamage(1); // Каждый выстрел = 1 урон
            Destroy(bullet.gameObject);
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;

        // Запускаем мигание дочерних спрайтов
        StartCoroutine(FlashHit());

        if (health <= 0)
        {
            if (scoreSystem != null)
            {
                int points = baseScore * threatCost;
                scoreSystem.AddScore(points);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashHit()
    {
        // Меняем цвет всех спрайтов на красный
        foreach (var sr in childRenderers)
        {
            if(sr != null)
                sr.color = Color.lightPink;
        }

        yield return new WaitForSeconds(flashDuration);

        // Возвращаем исходный цвет
        foreach (var sr in childRenderers)
        {
            if(sr != null)
                sr.color = Color.white;
        }
    }
}
