using UnityEngine;

public class ShieldProtection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        
        if (bullet != null && bullet.isEnemy && !bullet.isLaser)
        {
            Destroy(bullet.gameObject);
        }
    }
}