using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Настройки владельца")]
    [Tooltip("Если true, это оружие принадлежит игроку, иначе - врагу.")]
    public bool isPlayer = false; 

    [Header("Звуки выстрела")]
    public string playerShootSoundName = "PlayerShoot-1";
    public string enemyShootSoundName = "EnemyShoot-1";

    public Bullet bullet;
    public Bullet AltBullet;
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

        if (delayTimer < shootDelaySeconds)
        {
            delayTimer += Time.deltaTime;
        }

        if (autoShoot && delayTimer >= shootDelaySeconds)
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
    }

    public void Shoot()
    {
        if (delayTimer < shootDelaySeconds) return;

        PlayShootSound();

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
        goBullet.isEnemy = !isPlayer; 
        currentFiredBullet = goBullet; 

        DespawnBulletIfLaser(currentFiredBullet);
    }

    public void AltShoot()
    {
        if (delayTimer < shootDelaySeconds) return;

        PlayShootSound();

        SpriteRenderer AltBulletRenderer = AltBullet.GetComponent<SpriteRenderer>();
        float bulletHeight = 0f;

        if (AltBulletRenderer != null && AltBulletRenderer.sprite != null)
        {
            bulletHeight = AltBulletRenderer.bounds.size.y;
        }

        float offset = bulletHeight / 2f; 
        Vector3 spawnOffset = direction * offset;
        Vector3 spawnPosition = transform.position + spawnOffset;

        GameObject go = Instantiate(AltBullet.gameObject, spawnPosition, transform.rotation);
        
        Bullet goBullet = go.GetComponent<Bullet>();
        goBullet.direction = direction;
        goBullet.isEnemy = !isPlayer; 
        currentFiredBullet = goBullet; 

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
    private void PlayShootSound()
    {
        if (AudioManager.instance != null)
        {
            string soundName = isPlayer ? playerShootSoundName : enemyShootSoundName;
            AudioManager.instance.PlaySFXOneShot(soundName);
        }
    }
}