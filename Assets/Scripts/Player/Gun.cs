using UnityEngine;

public class Gun: MonoBehaviour
{

    public Bullet bullet;
    Vector2 direction;

    void Start()
    {

    }

    void Update()
    {
        direction = (transform.localRotation * Vector2.up).normalized;
    }
    
    public void Shoot()
    {
        GameObject go = Instantiate(bullet.gameObject, transform.position, Quaternion.identity);
        Bullet goBullet = go.GetComponent<Bullet>();
        goBullet.direction = direction;
    }
}
