using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Vector2 direction = new Vector2(0, 1);
    public float speed = 2;
    public Vector2 velocity;
    public bool isEnemy = false;
    
    public bool isLaser = false; 
    public float destroyDuration = 0.5f;
    public float initialDelay = 1.0f;

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
        StartCoroutine(ScaleOutRoutine());
        RemoveColliderDelayed(initialDelay);
    }

    private IEnumerator ScaleOutRoutine()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return new WaitForSeconds(initialDelay);

            Vector3 startScale = transform.localScale;
            Vector3 startPos = transform.position;
            float height = sr.bounds.size.y; 

            float time = 0;

            while (time < destroyDuration)
            {
                time += Time.deltaTime;
                float progress = time / destroyDuration;
                
                float newScaleY = Mathf.Lerp(startScale.y, 0f, progress);
                transform.localScale = new Vector3(startScale.x, newScaleY, startScale.z);


                float lostHeight = height * startScale.y * progress; 
                transform.position = startPos + Vector3.down * (lostHeight / 2f);
                
                yield return null;
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