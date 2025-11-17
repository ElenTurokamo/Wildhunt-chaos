using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public GameObject gameOverUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (bullet.isEnemy)
            {
                Destroy(gameObject);
                Destroy(bullet.gameObject);
            }
        }

        EnemyDestructable destructable = collision.GetComponent<EnemyDestructable>();
        if (destructable != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0f;
            Destroy(gameObject);
            Destroy(destructable.gameObject);    
        }
    }
}
