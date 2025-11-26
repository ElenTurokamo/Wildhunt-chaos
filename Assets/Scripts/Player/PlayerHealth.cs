using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameplayUI;  
    [SerializeField] private GameObject gameOverUI; 
    [SerializeField] private GameObject pauseMenu; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null && bullet.isEnemy)
        {
            PlayerDie();
            Destroy(bullet.gameObject);
            return;
        }

        EnemyDestructable destructable = collision.GetComponent<EnemyDestructable>();
        if (destructable != null)
        {
            PlayerDie();
            Destroy(destructable.gameObject);
        }
    }

    private void PlayerDie()
    {
        if (gameplayUI != null)
            gameplayUI.SetActive(false);

        if (pauseMenu != null)
            pauseMenu.SetActive(false);
            Destroy(pauseMenu);

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Time.timeScale = 0f;

        Destroy(gameObject);
    }
}
