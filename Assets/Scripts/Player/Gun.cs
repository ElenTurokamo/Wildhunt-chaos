using UnityEngine;

public class Gun: MonoBehaviour
{

    public Bullet bullet;

    void Start()
    {
        
    }

    void Update()
    {

    }
    
    public void Shoot()
    {
        GameObject go = Instantiate(bullet.gameObject, transform.position, Quaternion.identity);
    }
}
