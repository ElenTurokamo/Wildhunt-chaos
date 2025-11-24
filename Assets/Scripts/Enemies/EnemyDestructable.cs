using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemyDestructable : MonoBehaviour
{
    [Header("Настройки очков")]
    public int baseScore = 10;
    [HideInInspector] public int threatCost = 1;

    [Header("Настройки здоровья")]
    public int health = 3; 

    [Header("Эффект попадания")]
    public float flashDuration = 0.1f; 

    [Header("Эффект взрыва при смерти")]
    public GameObject explosionPrefab; 

    private bool canBeDestroyed = false;
    private ScoreSystem scoreSystem;

    private SpriteRenderer[] childRenderers;

    private EnemySound enemySound;

    void Awake()
    {
        scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
        childRenderers = GetComponentsInChildren<SpriteRenderer>();
        enemySound = GetComponent<EnemySound>();
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
            enemySound.PlayHitAt(transform.position);
            TakeDamage(1);
            Destroy(bullet.gameObject);
        }
    }

    private void TakeDamage(int amount)
    {
        health -= amount;

        StartCoroutine(FlashHit());

        if (health <= 0)
        {
            if (scoreSystem != null)
            {
                int points = baseScore * threatCost;
                scoreSystem.AddScore(points);
            }

            enemySound.PlayDeathAt(transform.position);

            // Эффект взрыва
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(explosion, ps.main.duration + ps.main.startLifetime.constantMax);
                else
                    Destroy(explosion, 1f);
            }
    
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashHit()
    {
        foreach (var sr in childRenderers)
        {
            if(sr != null)
                sr.color = new Color(1f, 0.75f, 0.8f); 
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (var sr in childRenderers)
        {
            if(sr != null)
                sr.color = Color.white;
        }
    }
}
