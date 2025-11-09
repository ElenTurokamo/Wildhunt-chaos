using Unity.VisualScripting;
using UnityEngine;

public class EnemyDestructable : MonoBehaviour
{

    bool canBeDestroyed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 5.0f)
        {
            canBeDestroyed = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeDestroyed)
        {
            return;
        }

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet != null)
        {
            Destroy(gameObject);
            Destroy(bullet.gameObject);
        }
    }
}




