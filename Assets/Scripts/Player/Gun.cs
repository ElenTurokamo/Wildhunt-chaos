using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet bullet;
    Vector2 direction;

    public bool autoShoot = false;
    public float shootIntervalSeconds = 0.5f;
    public float shootDelaySeconds = 0.0f;
    float shootTimer = 0f;
    float delayTimer = 0f;

    public bool isActive = false;

    private Bullet currentFiredBullet; 

    void Update()
    {
        if (!isActive) return;
        
        direction = (transform.localRotation * Vector2.up).normalized;

        if (autoShoot)
        {
            if (delayTimer >= shootDelaySeconds)
            {
                if (shootTimer >= shootIntervalSeconds)
                {
                    Shoot();
                    shootTimer = 0;
                }
                else
                {
                    shootTimer += Time.deltaTime;
                }
            }
            else
            {
                delayTimer += Time.deltaTime;
            }
        }
    }

    public void Shoot()
    {
        SpriteRenderer bulletRenderer = bullet.GetComponent<SpriteRenderer>();
        float bulletHeight = 0f;

        if (bulletRenderer != null && bulletRenderer.sprite != null)
        {
            bulletHeight = bulletRenderer.bounds.size.y;
        }

        float offset = bulletHeight / 2f; 

        Vector3 spawnOffset = direction * offset;
        
        Vector3 spawnPosition = transform.position + spawnOffset;

        GameObject go = Instantiate(bullet.gameObject, spawnPosition, transform.rotation);
        
        Bullet goBullet = go.GetComponent<Bullet>();
        goBullet.direction = direction;

        currentFiredBullet = goBullet; 
        
        if (goBullet.isLaser)
        {
            goBullet.RemoveColliderDelayed(0.6f);
        }

        DespawnBulletIfLaser(currentFiredBullet);
    }
    private void DespawnBulletIfLaser(Bullet bullet)
    {
        if (bullet != null && bullet.isLaser)
        {
            bullet.FadeOutAndDestroy(); 
            currentFiredBullet = null; 
        }
    }
}