using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5;

    void Start()
    {
        
    }

private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        pos.y -= moveSpeed * Time.fixedDeltaTime;
        transform.position = pos;
    }
}
