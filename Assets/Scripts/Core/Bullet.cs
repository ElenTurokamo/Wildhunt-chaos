using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Vector2 direction = new Vector2(0, 1);
    public float speed = 2;
    public Vector2 velocity;
    public bool isEnemy = false;
    
    public bool isLaser = false; 

    void Start()
    {
        if (!isLaser) 
        {
            Destroy(gameObject, 3);
        }
    }

    void Update()
    {
        velocity = direction * speed;
    }
    
    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        pos += velocity * Time.fixedDeltaTime;

        transform.position = pos;
    }

    public void FadeOutAndDestroy()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float duration = 1f; 
            float time = 0;
            float startAlpha = sr.color.a;

            while (time < duration)
            {
                time += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startAlpha, 0f, time / duration);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
                yield return null;
            }
        }
        Destroy(gameObject);
    }

    public void RemoveColliderDelayed(float delay)
    {
        StartCoroutine(RemoveColliderRoutine(delay));
    }

    private IEnumerator RemoveColliderRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            Destroy(coll);
        }
    }
}