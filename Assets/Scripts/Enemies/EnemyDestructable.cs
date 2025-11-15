using Unity.VisualScripting;
using UnityEngine;

public class EnemyDestructable : MonoBehaviour
{
    [Header("Настройки очков")]
    public int baseScore = 10;
    [HideInInspector] public int threatCost = 1;

    bool canBeDestroyed = false;
    private ScoreSystem scoreSystem;

    void Awake()
    {
        scoreSystem = Object.FindFirstObjectByType<ScoreSystem>();
    }

    void Start()
    {

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeDestroyed)
        {
            return;
        }

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && !bullet.isEnemy)
        {
            if (scoreSystem != null)
            {
                int points = baseScore * threatCost;
                scoreSystem.AddScore(points);
            }

            Destroy(gameObject);
            Destroy(bullet.gameObject);
            }
        }
    }





